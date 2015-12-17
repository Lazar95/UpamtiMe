﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Test
    {
        public int a { get; set; }
        public int lazar { get; set; }
    }
    public class Users
    {
        //TODO pazi ne setujemo avatar
        public static User addUser(string name, string username, string password, string email, string bio = null, string location = null, string surname = null)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            User u = new User
            {
                name = name,
                username = username,
                password = password,
                email = email,
                bio = bio,
                location = location,
                surname = surname,
                score = 0,
                avatar = null,
                lastLogin = DateTime.Now,
                dateRegistered = DateTime.Now,
                totalCardsLearned = 0,
                streak = 0,
            };

            dc.Users.InsertOnSubmit(u);
            dc.SubmitChanges();

            return u;
        }

        public static User GetUser(int userID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Users where a.userID == userID select a).First();
        }
    }
}
