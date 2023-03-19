﻿// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// !!! OBSAH TOHOTO SOUBORU NENÍ DOVOLENO MĚNIT !!!
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
global using static RoDiTestExtensions.Tests.TestUtils;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using DelegateLib;

namespace RoDiTestExtensions.Tests;

public static class TestUtils
{
    private static int dispatchCounter = 0;
    private static readonly List<Action<object, EventArgs>> dispatchs = new();

    public static Type GetTestedType(string typename)
    {
        foreach (var item in typeof(_TestHook).Assembly.ExportedTypes)
        {
            if (item.Name == typename)
                return item;
        }
        //foreach (var item in ... ExportedTypes) // Assembly.GetCallingAssembly().GetReferencedAssemblies())
        //{
        //    foreach (var exportedType in Assembly.Load(item).ExportedTypes)
        //    {
        //        if (exportedType.Name == typename)
        //        {
        //            return exportedType;
        //        }
        //    }
        //}

        Assert.Fail($"This test requires implementation of type '{typename}'");
        throw new SystemException();
    }

    private static object New(Type? type)
    {
        Assert.IsNotNull(type);

        var constructor = type?.GetConstructor(new Type[0]);
        Assert.IsNotNull(constructor, $"Class {type?.Name} is missing nonparametric constructor");

        object? obj = constructor?.Invoke(new object?[0]);
        Assert.IsNotNull(obj, $"Class {type?.Name} failed to construct object using nonparametric constructor");
        return obj;
    }

    public static object New(Type? type, params object[] values)
    {
        Assert.IsNotNull(type, "Unable to construct object of unknown type");

        if (values.Length == 0)
            return New(type);

        Type[] types = new Type[values.Length];
        for (int i = 0; i < values.Length; i++)
            types[i] = values[i].GetType();

        var constructor = type?.GetConstructor(types);
        Assert.IsNotNull(constructor, $"Class {type?.Name} is missing parametric constructor");

        object? obj = constructor?.Invoke(values);
        Assert.IsNotNull(obj, $"Class {type?.Name} failed to construct object using parametric constructor");
        return obj;
    }

    public static void RegisterEvent(object obj, string evt, Action<object, EventArgs> method)
    {
        var ev = obj.GetType().GetEvent(evt);
        Assert.IsNotNull(ev);

        var addMethod = ev.GetAddMethod();
        Assert.IsNotNull(addMethod);

        Type tDelegate = addMethod.GetParameters()[0].ParameterType;

        Type returnType = GetDelegateReturnType(tDelegate);
        if (returnType != typeof(void))
            throw new ArgumentException("Delegate has a return type.");

        DynamicMethod handler = new DynamicMethod("", null, GetDelegateParameterTypes(tDelegate));

        MethodInfo? helper = typeof(TestUtils).GetMethod("HelperDispatcher");
        Assert.IsNotNull(helper);

        ILGenerator ilgen = handler.GetILGenerator();
        ilgen.Emit(OpCodes.Ldarg_0);
        ilgen.Emit(OpCodes.Ldarg_1);
        ilgen.Emit(OpCodes.Ldc_I4, dispatchCounter);
        ilgen.Emit(OpCodes.Call, helper);
        ilgen.Emit(OpCodes.Ret);

        dispatchs.Add(method);
        dispatchCounter++;

        Delegate del = handler.CreateDelegate(tDelegate);

        addMethod.Invoke(obj, new object?[] { del });
    }

    public static void HelperDispatcher(object a, EventArgs e, int c)
    {
        dispatchs[c](a, e);
    }

    public static Type[] GetDelegateParameterTypes(Type d)
    {
        if (d.BaseType != typeof(MulticastDelegate))
            throw new ArgumentException("Not a delegate.", nameof(d));

        MethodInfo? invoke = d.GetMethod("Invoke");
        if (invoke is null)
            throw new ArgumentException("Not a delegate.", nameof(d));

        ParameterInfo[] parameters = invoke.GetParameters();
        Type[] typeParameters = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            typeParameters[i] = parameters[i].ParameterType;
        }
        return typeParameters;
    }

    private static Type GetDelegateReturnType(Type d)
    {
        if (d.BaseType != typeof(MulticastDelegate))
            throw new ArgumentException("Not a delegate.", nameof(d));

        MethodInfo? invoke = d.GetMethod("Invoke");
        if (invoke is null)
            throw new ArgumentException("Not a delegate.", nameof(d));

        return invoke.ReturnType;
    }

    public static object? GetProperty(object obj, string prop)
    {
        var p = obj.GetType().GetProperty(prop);
        Assert.IsNotNull(p);

        var get = p.GetGetMethod();
        Assert.IsNotNull(get);

        try
        {
            return get.Invoke(obj, new object?[0]);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? new SystemException();
        }
    }

    public static void SetProperty(object obj, string prop, object? value)
    {
        var p = obj.GetType().GetProperty(prop);
        Assert.IsNotNull(p);

        var set = p.GetSetMethod();
        Assert.IsNotNull(set);

        try
        {
            set.Invoke(obj, new object?[1] { value });
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? new SystemException();
        }
    }

    public static object? Invoke(object obj, string method, params object?[] values)
    {
        var m = obj.GetType().GetMethod(method);
        Assert.IsNotNull(m);

        try
        {
            return m.Invoke(obj, values);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? new SystemException();
        }
    }

    public static T? Invoke<T>(object obj, string method, params object?[] values)
    {
        var m = obj.GetType().GetMethod(method);
        Assert.IsNotNull(m);

        try
        {
            return (T?)m.Invoke(obj, values);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? new SystemException();
        }
    }

    public static object? InvokePrivateStatic(object obj, string method, params object?[] values)
    {
        var m = obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(m);

        return m.Invoke(obj, values);
    }

    public static object? InvokePublicStatic(Type type, string method, params object?[] values)
    {
        var m = type.GetMethod(method, BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(m);

        return m.Invoke(null, values);
    }

    public static T? InvokePublicStatic<T>(Type type, string method, params object?[] values)
    {
        var m = type.GetMethod(method, BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(m);

        return (T?)m.Invoke(null, values);
    }

    public static object? IndexBy(object obj, object index)
    {
        var indexer = obj.GetType().GetProperty("Item");
        Assert.IsNotNull(indexer);

        var get = indexer.GetGetMethod();
        Assert.IsNotNull(get);

        return get.Invoke(obj, new object?[] { index });
    }

    public static object? SetIndexBy(object obj, object index, object value)
    {
        var indexer = obj.GetType().GetProperty("Item");
        Assert.IsNotNull(indexer);

        var set = indexer.GetSetMethod();
        Assert.IsNotNull(set);

        return set.Invoke(obj, new object?[] { index, value });
    }

    public static void AssertByEnumerator(object obj, params object[] values)
    {
        IEnumerator? enumerator = (IEnumerator?)Invoke(obj, "GetEnumerator");
        Assert.IsNotNull(enumerator);

        List<object?> actual = new();
        while (enumerator.MoveNext())
            actual.Add(enumerator.Current);

        CollectionAssert.AreEqual(values, actual);
    }
}
