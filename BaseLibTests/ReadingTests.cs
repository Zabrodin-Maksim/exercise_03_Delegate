// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// !!! OBSAH TOHOTO SOUBORU NENÍ DOVOLENO MĚNIT !!!
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fei.BaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace Fei.BaseLib.Tests
{
    [TestClass()]
    public class ReadingTests
    {
        [TestInitialize]
        public void InitializeCulture()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
        
        [TestMethod()]
        public void ReadIntTest()
        {
            Reading.ReadLine = () => "12345";
            Reading.WriteLine = s => { };

            int actual = Reading.ReadInt("Enter an integer value: ");

            Assert.AreEqual(12345, actual);
        }

        [TestMethod()]
        public void ReadDoubleTest()
        {
            Reading.ReadLine = () => "123.45";
            Reading.WriteLine = s => { };

            double actual = Reading.ReadDouble("Enter a double value: ");

            Assert.AreEqual(123.45, actual);
        }

        [TestMethod()]
        public void ReadCharTest()
        {
            Reading.ReadLine = () => "z";
            Reading.WriteLine = s => { };

            char actual = Reading.ReadChar("Enter a char value: ");

            Assert.AreEqual('z', actual);
        }

        [TestMethod()]
        public void ReadStringTest()
        {
            Reading.ReadLine = () => "test string";
            Reading.WriteLine = s => { };

            string actual = Reading.ReadString("Enter a string value: ");

            Assert.AreEqual("test string", actual);
        }
    }
}
