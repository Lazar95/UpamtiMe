using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

        [TestMethod]
        public void RegexTest()
        {
            bool a =
                Regex.IsMatch(
                    "A public action method 'ksndkjasn' was not found on controller 'UpamtiMe.Controllers.UsersController'.",
                    @"A public action method .+ was not found on controller .+");
        }

        [TestMethod]
        public void Jajceva()
        {
            List<int> list = Levels.getOptions(21, new List<int> { 10, 15, 20, 30 }, 5);
        }

        [TestMethod]
        public void Unique()
        {
            List<CardBasicDTO> sm = new List<CardBasicDTO>();
            sm.Add(new CardBasicDTO
            {
                Answer = "1"
            });
            sm.Add(new CardBasicDTO
            {
                Answer = "1"
            });
            sm.Add(new CardBasicDTO
            {
                Answer = "2"
            });
            sm.Add(new CardBasicDTO
            {
                Answer = "1"
            });
            sm.Add(new CardBasicDTO
            {
                Answer = "3"
            });

            sm = sm.GroupBy(a => a.Answer).Select(a => a.First()).ToList();
        }
    }
}
