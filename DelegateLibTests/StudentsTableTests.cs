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
using System.Reflection.Emit;
using System.Reflection;

namespace DelegateLib.Tests;

[TestClass()]
public class StudentsTableTests
{
    public static IComparable ExtractNumber(object o)
    {
        return (int)GetProperty(o, "Number");
    }

    public Delegate GetStudentNumber = null;

    public void ConstructGetStudentNumber()
    {
        DynamicMethod handler = new DynamicMethod("", typeof(IComparable), new Type[] { GetTestedType("Student") });
        ILGenerator ilgen = handler.GetILGenerator();
        ilgen.Emit(OpCodes.Ldarg_0);
        ilgen.Emit(OpCodes.Call, GetType().GetMethod(nameof(ExtractNumber), BindingFlags.Static | BindingFlags.Public));
        ilgen.Emit(OpCodes.Ret);
        Delegate d = handler.CreateDelegate(GetTestedType("GetKeyCallback"));

        GetStudentNumber = d;
    }

    private object MakeStudent(string name, int id)
    {
        Type studentType = GetTestedType("Student");
        object student = New(studentType);
        SetProperty(student, "Number", id);
        SetProperty(student, "Name", name);

        return student;
    }

    [TestMethod()]
    public void AddAndGetTest()
    {
        ConstructGetStudentNumber();

        object table = New(GetTestedType("StudentsTable"), 20, GetStudentNumber);
        Invoke(table, "Add", MakeStudent("Peter", 1));
        Invoke(table, "Add", MakeStudent("John", 2));

        Assert.IsNull(Invoke(table, "Get", 0));
        Assert.IsNotNull(Invoke(table, "Get", 1));
        Assert.IsNotNull(Invoke(table, "Get", 2));
        Assert.IsNull(Invoke(table, "Get", 3));
    }


    [TestMethod()]
    public void DeleteAndGetTest()
    {
        ConstructGetStudentNumber();

        object table = New(GetTestedType("StudentsTable"), 20, GetStudentNumber);
        Invoke(table, "Add", MakeStudent("Peter", 1));
        Invoke(table, "Add", MakeStudent("John", 2));
        Invoke(table, "Add", MakeStudent("John", 3));

        object removed = Invoke(table, "Delete", 2);

        Assert.IsNotNull(removed);
        Assert.IsNull(Invoke(table, "Get", 0));
        Assert.IsNotNull(Invoke(table, "Get", 1));
        Assert.IsNull(Invoke(table, "Get", 2));
        Assert.IsNotNull(Invoke(table, "Get", 3));
        Assert.IsNull(Invoke(table, "Get", 4));
    }
}