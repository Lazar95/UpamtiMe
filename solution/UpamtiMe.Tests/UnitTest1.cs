using System;
using System.Configuration;
using System.Web.UI.WebControls;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpamtiMe.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Users.resetStatisticsForAllUsers();

            Data.DTOs.StatisctisByDays statisctis1 = Data.Users.GetStatisctisByDays(2, 1,5);
            Data.DTOs.StatisctisByDays statisctis7 = Data.Users.GetStatisctisByDays(2, 7,5);
            Data.DTOs.StatisctisByDays statisctisTotal = Data.Users.GetStatisctisByDays(2,5);

            Data.DTOs.StatisctisByDays aIb = statisctis1.Add(statisctis7);
            Data.DTOs.StatisctisByDays bIc = statisctis7.Add(statisctisTotal);
            int a = 5;
        }

        [TestMethod]
        public void Beautify()
        {
            double dsfa = 3.7;
            int sada = (int) dsfa;

            int sad = int.MaxValue;
            long lonssad = long.MaxValue;
            
            float a = 15.151235f;
            string aS = a.beautify();

            float b = 12151.785165f;
            string bS = b.beautify();

            float c = 1234567328.2125f;
            string cS = c.beautify();

            float d = float.MaxValue;

        }
    }
}
