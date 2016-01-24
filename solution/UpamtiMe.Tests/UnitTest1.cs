using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpamtiMe.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Data.DTOs.StatisctisByDays statisctis = Data.Users.GetStatisctisByDays(2, 1);
            int a = 5;
        }
    }
}
