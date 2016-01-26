﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Data.Objects;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DTOs;

namespace Data
{
    public class Users
    {
        public static User addUser(string name, string username, string password, string email, string bio = null,
            string location = null, string surname = null, byte[] avatar = null)
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
                avatar = avatar?.ToArray(),
                lastLogin = DateTime.Now,
                dateRegistered = DateTime.Now,
                totalCardsSeen = 0,
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

        public static string getUsername(int userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Users where a.userID == userID select a.username).First();
        }

        public static User Login(string username, string pass)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            var find = from a in dc.Users where a.username == username && a.password == pass select a;
            if (find.Any())
            {
                find.First().lastLogin = DateTime.Now;
                dc.SubmitChanges();
                return find.First();
            }
            else
                return null;
        }

        public static bool checkUsername(string username)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Users where a.username == username select a).Any();
        }

        public static bool checkEmail(string email)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Users where a.email == email select a).Any();
        }

        

        public static List<LeaderboardEntryDTO> getLeaderboard(int userID, int? courseID = null)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            List<int> friendIDs;
            if (courseID == null)
            {
                friendIDs = (from a in dc.Users
                    from b in dc.Friendships
                    where a.userID == b.user1ID || a.userID == b.user2ID
                    select a.userID == b.user1ID ? b.user2ID : b.user1ID).ToList();
            }
            else
            {
                friendIDs = (from a in dc.Users
                             from b in dc.Friendships
                             from c in dc.UsersCourses
                             where (a.userID == b.user1ID || a.userID == b.user2ID) && ( c.courseID == courseID && c.userID == b.user1ID && c.userID == b.user2ID)
                             select a.userID == b.user1ID ? b.user2ID : b.user1ID).ToList();
            }

            return (from a in friendIDs
                    from  b in dc.Users
                    where a == b.userID
                        select new LeaderboardEntryDTO()
                        {
                            UserID = b.userID,
                            Username = b.username,
                            FristName = b.name,
                            LastName = b.surname,
                            WeekScore = b.thisWeekScore,
                            MonthScore = b.thisMonthScore,
                            AllTimeScore = b.score,
                        }).ToList();

        }

        public static Boolean enrolled(int userID, int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.UsersCourses where a.userID == userID && a.courseID == courseID select a).Any();
        }

        public static UsersCourse enroll(int userID, int courseID)
        {
            if (enrolled(userID, courseID))
                return null;
            DataClasses1DataContext dc = new DataClasses1DataContext();
            UsersCourse uc = new UsersCourse
            {
                userID = userID,
                courseID = courseID,
                startDate = DateTime.Now,
                score = 0,
                lastPlayed = null,
                thisWeekScore = 0,
                thisMonthScore = 0
            };

            Course c = Courses.getCourse(courseID, dc);
            c.participantCount++;

            dc.UsersCourses.InsertOnSubmit(uc);
            dc.SubmitChanges();

            return uc;
        }

        public static void follow(int aID, int bID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            Friendship f = new Friendship
            {
                user1ID = aID,
                user2ID = bID
            };
            dc.Friendships.InsertOnSubmit(f);
            dc.SubmitChanges();
        }

        public static void unfollow(int aID, int bID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            Friendship f = (from a in dc.Friendships where a.user1ID == aID && a.user2ID == bID select a).First();
            dc.Friendships.DeleteOnSubmit(f);
            dc.SubmitChanges();
        }

        public static bool follows(int aID, int bID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Friendships where a.user1ID == aID && a.user2ID == bID select a).Any();
        }
       

        public static void editAvatar(int userID, byte[] file)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            User usr = GetUser(userID, dc);
            usr.avatar = new System.Data.Linq.Binary(file);
            dc.SubmitChanges();
        }

        public static void updateStatisctics(int userID, float score, int cardsLearned, bool streak)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            User usr = GetUser(userID, dc);
            usr.score += score;
            usr.thisWeekScore += score;
            usr.thisMonthScore += score;
            usr.totalCardsSeen += cardsLearned;

            if (streak)
                usr.streak++;
            dc.SubmitChanges();
        }

        public static void resetStatisticsForAllUsers()
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (User user in dc.Users)
            {
                float last7days = 0;
                float last30days = 0;
                bool streak = true;

                foreach (UsersCourse uc in (from a in dc.UsersCourses where a.userID == user.userID select a))
                {

                    DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);

                    var query = (from a in dc.UserCourseStatistics
                        where a.userCourseID == uc.usersCoursesID && a.date > thirtyDaysAgo
                        select new {date = a.date, score = a.score}).ToList();

                    
                    if (!(from a in query where DateTime.Today == a.date select a).Any())
                        streak = false;

                    DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);

                    float week = (from a in query where a.date > sevenDaysAgo select a.score).Sum();
                    float month = (from a in query select a.score).Sum();

                    uc.thisWeekScore = week;
                    uc.thisMonthScore = month;

                    last7days += week;
                    last30days += month;

                }

                if (streak)
                    user.streak = 0;

                user.thisWeekScore = last7days;
                user.thisMonthScore = last30days;
            }
            dc.SubmitChanges();
        }
         

        public static List<int> getUserCourses(int userID, int? courseID = null)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return
                (from a in dc.UsersCourses
                    where a.userID == userID && (courseID == null || a.courseID == courseID)
                    select a.usersCoursesID).ToList();
        } 

        public static StatisctisByDays GetStatisctisByDays(int userID, int? courseID = null, int timeSpan = 30)
        {
            return GetStatisctisByDays(getUserCourses(userID, courseID), timeSpan);
        }
      

        public static StatisctisByDays GetStatisctisByDays(List<int> userCourseID, int timeSpan = 30)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            StatisctisByDays returnValue = new StatisctisByDays();

            DateTime prev = DateTime.Today.Subtract(TimeSpan.FromDays(timeSpan));
            returnValue.SetDates(timeSpan);

            List<UserCourseStatistic> stats =
                (from a in dc.UserCourseStatistics where userCourseID.Contains(a.userCourseID)  && a.date > prev select a).OrderBy(a=>a.date).ToList();

           

            foreach (UserCourseStatistic stat in stats)
            {
                int zeroDays = stat.date.Subtract(prev).Days -1;
                returnValue.SetZeros(zeroDays);

                returnValue.AddValues(stat.score, stat.learnedCards, stat.reviewedCards, stat.sessionNo, stat.timeSpent, stat.learnedCorrectAnswers, stat.learnedWrongAnswers, stat.reviewCorrectAnswers, stat.reviewWrongAnswers);
                
                prev = stat.date;
            }

            int zeroDaysAfter = DateTime.Today.Subtract(prev).Days;
            returnValue.SetZeros(zeroDaysAfter);
          
            returnValue.TrimStrings();

            return returnValue;
        }

    }
}
