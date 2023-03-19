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
using RoDiTestExtensions.Tests;

namespace DelegateLib.Tests
{
    [TestClass()]
    public class StudentTests
    {

        [TestMethod()]
        public void ToStringTest1()
        {
            Type studentType = GetTestedType("Student");
            object student = New(studentType);
            SetProperty(student, "Number", 1);
            SetProperty(student, "Name", "Peter");

            string? actual = Invoke<string>(student, "ToString");
            Assert.IsTrue(actual is not null && actual.StartsWith("Peter (1,"));
        }

        [TestMethod()]
        public void ToStringTest2()
        {
            Type studentType = GetTestedType("Student");
            object student = New(studentType);
            SetProperty(student, "Number", 2);
            SetProperty(student, "Name", "Lucy");

            string? actual = Invoke<string>(student, "ToString");
            Assert.IsTrue(actual is not null && actual.StartsWith("Lucy (2,"));
        }
    }
}