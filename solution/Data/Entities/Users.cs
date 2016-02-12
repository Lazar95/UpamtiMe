using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Data.Objects;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Data.DTOs;

namespace Data
{
    public static class HashPassword
    {
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string SaltedHashPassword(string password, string salt)
        {
            byte[] saltBytes = GetBytes(salt);
            var passwordAndSaltBytes = Concat(password, saltBytes);
            return ComputeHash(passwordAndSaltBytes);
        }

        public static bool Verify(string salt, string password, string hash)
        {
            var saltBytes = GetBytes(salt);
            var passwordAndSaltBytes = Concat(password, saltBytes);
            var hashAttempt = ComputeHash(passwordAndSaltBytes);
            return hash == hashAttempt;
        }

        static private string ComputeHash(byte[] bytes)
        {
            using (var sha512 = SHA512.Create())
            {
                return Convert.ToBase64String(sha512.ComputeHash(bytes));
            }
        }

        static private byte[] Concat(string password, byte[] saltBytes)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return passwordBytes.Concat(saltBytes).ToArray();
        }
    }

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
                defaultAvatarID = Data.DefaultPictures.getRandom((int)Data.Enumerations.DefaultPicture.Avatar)
            };

            dc.Users.InsertOnSubmit(u);
            dc.SubmitChanges();

            return u;
        }


        public static void encryptAllPasswords()
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<User> usrs = (from a in dc.Users select a).ToList();
            foreach (User usr in usrs)
            {
                usr.password = HashPassword.SaltedHashPassword(usr.password, usr.username);
            }
           
            dc.SubmitChanges();
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
                friendIDs = (from a in dc.Friendships
                    where a.user1ID == userID 
                    select a.user2ID).ToList();
            }
            else
            {
                friendIDs = (from a in dc.Friendships
                             from c in dc.UsersCourses
                             where a.user1ID == userID && ( c.courseID == courseID && c.userID == a.user1ID && c.userID == a.user2ID)
                             select a.user2ID ).ToList();
            }

            friendIDs.Add(userID);

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

        public static void editProfile(int userID, string name, string surname, string location,string bio)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            User usr = GetUser(userID, dc);
            
            usr.name = name ?? usr.name;
            usr.surname = surname ?? usr.surname;
            usr.location = location ?? usr.location;
            usr.bio = bio ?? usr.bio;

            dc.SubmitChanges();
        }

        public static void editAvatar(int userID, byte[] file)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            User usr = GetUser(userID, dc);
            usr.avatar = new System.Data.Linq.Binary(file);
            dc.SubmitChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="score"></param>
        /// <param name="cardsLearned"></param>
        public static void updateStatisctics(int userID, float score, int cardsLearned)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            User usr = GetUser(userID, dc);
            usr.score += score;
            usr.thisWeekScore += score;
            usr.thisMonthScore += score;
            usr.totalCardsSeen += cardsLearned;

            if (!usr.doneToday)
            {
                usr.streak++;
                usr.doneToday = true;
            }
                
            dc.SubmitChanges();
        }

        public static void resetStatisticsForAllUsers()
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (User user in dc.Users)
            {
                float last7days = 0;
                float last30days = 0;

                foreach (UsersCourse uc in (from a in dc.UsersCourses where a.userID == user.userID select a))
                {

                    DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);

                    var query = (from a in dc.UserCourseStatistics
                        where a.userCourseID == uc.usersCoursesID && a.date > thirtyDaysAgo
                        select new {date = a.date, score = a.score}).ToList();

                    DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);

                    float week = (from a in query where a.date > sevenDaysAgo select a.score).Sum();
                    float month = (from a in query select a.score).Sum();

                    uc.thisWeekScore = week;
                    uc.thisMonthScore = month;

                    last7days += week;
                    last30days += month;

                }

                if (!user.doneToday)
                    user.streak = 0;

                user.doneToday = false;
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

           
            if (userCourseID.Count > 1)
            {
                stats = (from o in stats
                            group o by o.date into grouping
                            select new UserCourseStatistic()
                            {
                                score = grouping.Sum(a => a.score),
                                learnedCards = grouping.Sum(a => a.learnedCards),
                                reviewedCards = grouping.Sum(a => a.reviewedCards),
                                sessionNo = grouping.Sum(a => a.sessionNo),
                                timeSpent = grouping.Sum(a => a.timeSpent),
                                learnedCorrectAnswers = grouping.Sum(a => a.sessionNo),
                                learnedWrongAnswers = grouping.Sum(a => a.learnedWrongAnswers),
                                reviewCorrectAnswers = grouping.Sum(a => a.reviewCorrectAnswers),
                                reviewWrongAnswers = grouping.Sum(a => a.reviewWrongAnswers),
                                date = grouping.Key
                            }).ToList();
            }
           

            foreach (UserCourseStatistic stat in stats)
            {
                int zeroDays = (int)stat.date.Subtract(prev).TotalDays -1;
                returnValue.SetZeros(zeroDays);

                returnValue.AddValues(stat.score, stat.learnedCards, stat.reviewedCards, stat.sessionNo, stat.timeSpent, stat.learnedCorrectAnswers, stat.learnedWrongAnswers, stat.reviewCorrectAnswers, stat.reviewWrongAnswers);
                
                prev = stat.date;
            }

            int zeroDaysAfter = (int)DateTime.Today.Subtract(prev).TotalDays;
            returnValue.SetZeros(zeroDaysAfter);
          
            returnValue.TrimStrings();

            return returnValue;
        }

        //oni koji prate njega
        public static List<FollowerDTO> GetFollowers(int profileID, int? userID = null, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Users.AsEnumerable()
                from f in dc.Friendships
                where f.user2ID == profileID && f.user1ID == a.userID
                select new FollowerDTO
                {
                    UserID = a.userID,
                    FirstName = a.name,
                    LastName = a.surname,
                    Username = a.username,
                    Avatar =
                        (a.avatar == null || a.avatar.ToArray().Length == 0)
                            ? Data.DefaultPictures.getAt(a.defaultAvatarID)
                            : a.avatar.ToArray(),
                    Follow = userID == null ? (bool?)null : Users.follows(userID.Value, a.userID),
                }).ToList();

        }


        //oni koje on prati
        public static List<FollowerDTO> GetFollowing(int profileID, int? userID = null, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Users.AsEnumerable()
                    from f in dc.Friendships
                    where f.user1ID == profileID && f.user2ID == a.userID
                    select new FollowerDTO
                    {
                        UserID = a.userID,
                        FirstName = a.name,
                        LastName = a.surname,
                        Username = a.username,
                        Avatar =
                            (a.avatar == null || a.avatar.ToArray().Length == 0)
                                ? Data.DefaultPictures.getAt(a.defaultAvatarID)
                                : a.avatar.ToArray(),
                        Follow = userID == null ? (bool?)null : Users.follows(userID.Value, a.userID),
                    }).ToList();

        }

    }
}
