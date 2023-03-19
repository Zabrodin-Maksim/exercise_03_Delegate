// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// !!! OBSAH TOHOTO SOUBORU NENÍ DOVOLENO MĚNIT !!!
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DelegateLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Dynamic;
using System.Linq.Expressions;
using RoDiTestExtensions.Tests;
using System.Reflection.Emit;

namespace DelegateLib.Tests;

[TestClass()]
public class StudentsArrayTests
{
    [TestMethod()]
    public void TestThatSortHasADelegateParameter()
    {
        Type? classType = GetTestedType("StudentsArray"); // Assembly.Load("DelegateLib").GetType("DelegateLib.StudentsArray");
        Assert.IsNotNull(classType);
        MethodInfo? sortMethodInfo = classType.GetMethod("Sort");
        Assert.IsNotNull(sortMethodInfo);

        ParameterInfo[] sortMethodParameters = sortMethodInfo.GetParameters();
        Assert.IsNotNull(sortMethodParameters);
        Assert.AreEqual(1, sortMethodParameters.Length);

        ParameterInfo delegateParameter = sortMethodParameters[0];
        Assert.IsTrue(delegateParameter.ParameterType.IsSubclassOf(typeof(Delegate)));

        Type delegateType = delegateParameter.ParameterType;

        MethodInfo? invokeMethod = delegateType.GetMethod("Invoke");
        Assert.IsNotNull(invokeMethod);

        int studentParamsCount = 0;
        foreach (var item in invokeMethod.GetParameters())
            if (item.ParameterType.Name == "Student")
                studentParamsCount++;

        Assert.AreEqual(2, studentParamsCount);
    }

    public static bool CompareByNumber(object a, object b)
    {
        object? anum = GetProperty(a, "Number");
        object? bnum = GetProperty(b, "Number");
        return ((int)anum) < ((int)bnum);
    }

    public static bool CompareByName(object a, object b)
    {
        object? anum = GetProperty(a, "Name");
        object? bnum = GetProperty(b, "Name");
        return ((string)anum).CompareTo((string)bnum) < 0;
    }
    public static int Compare2ByNumber(object a, object b)
    {
        object? anum = GetProperty(a, "Number");
        object? bnum = GetProperty(b, "Number");
        return ((int)anum) < ((int)bnum) ? -1 : (((int)anum) > ((int)bnum) ? 1 : 0);
    }

    public static int Compare2ByName(object a, object b)
    {
        object? anum = GetProperty(a, "Name");
        object? bnum = GetProperty(b, "Name");
        return ((string)anum).CompareTo((string)bnum);
    }

    [TestMethod()]
    public void TestSorting()
    {
        Type? classType = GetTestedType("StudentsArray");
        Assert.IsNotNull(classType);

        object ary = New(classType, 4);

        SetIndexBy(ary, 0, MakeStudent("Aaa", 4));
        SetIndexBy(ary, 1, MakeStudent("Bbb", 2));
        SetIndexBy(ary, 2, MakeStudent("Ccc", 3));
        SetIndexBy(ary, 3, MakeStudent("Ddd", 1));

        Type? callbackType = GetTestedType("CompareStudentsCallback");
        Type retType = callbackType.GetMethod("Invoke").ReturnType;
        Delegate d = null;

        if (retType == typeof(bool))
            d = CreateComparisonMethodOfBool(callbackType, retType, nameof(CompareByNumber));
        else if (retType == typeof(int))
            d = CreateComparisonMethodOfBool(callbackType, retType, nameof(Compare2ByNumber));
        else
            Assert.Fail("Unsupported return type of CompareStudentsCallback");

        Invoke(ary, "Sort", d);

        Assert.AreEqual(1, GetProperty(IndexBy(ary, 0), "Number"));
        Assert.AreEqual(2, GetProperty(IndexBy(ary, 1), "Number"));
        Assert.AreEqual(3, GetProperty(IndexBy(ary, 2), "Number"));
        Assert.AreEqual(4, GetProperty(IndexBy(ary, 3), "Number"));

        if (retType == typeof(bool))
            d = CreateComparisonMethodOfBool(callbackType, retType, nameof(CompareByName));
        else if (retType == typeof(int))
            d = CreateComparisonMethodOfBool(callbackType, retType, nameof(Compare2ByName));

        Invoke(ary, "Sort", d);

        Assert.AreEqual(4, GetProperty(IndexBy(ary, 0), "Number"));
        Assert.AreEqual(2, GetProperty(IndexBy(ary, 1), "Number"));
        Assert.AreEqual(3, GetProperty(IndexBy(ary, 2), "Number"));
        Assert.AreEqual(1, GetProperty(IndexBy(ary, 3), "Number"));
    }

    private Delegate CreateComparisonMethodOfBool(Type callbackType, Type returnType, string methodName)
    {
        DynamicMethod handler = new DynamicMethod("", returnType, GetDelegateParameterTypes(callbackType));
        ILGenerator ilgen = handler.GetILGenerator();
        ilgen.Emit(OpCodes.Ldarg_0);
        ilgen.Emit(OpCodes.Ldarg_1);
        ilgen.Emit(OpCodes.Call, GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.Public));
        ilgen.Emit(OpCodes.Ret);
        Delegate d = handler.CreateDelegate(callbackType);
        return d;
    }

    private object MakeStudent(string name, int id)
    {
        Type studentType = GetTestedType("Student");
        object student = New(studentType);
        SetProperty(student, "Number", id);
        SetProperty(student, "Name", name);

        return student;
    }
}
