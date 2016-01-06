using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DTOs;

namespace Data
{
    public class Users
    {
        //TODO pazi ne setujemo avatar
        public static User addUser(string name, string username, string password, string email, string bio = null,
            string location = null, string surname = null)
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

        public static string getUsername(int userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Users where a.userID == userID select a.username).First();
        }

        public static User checkUsernameAndPassword(string username, string pass)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            var find = from a in dc.Users where a.username == username && a.password == pass select a;
            if (find.Any())
            {
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
                           join b in dc.Users
                               on a equals b.userID
                           select new LeaderboardEntryDTO()
                           {
                               UserID = b.userID,
                               Username = b.username,
                               FristName = b.name,
                               LastName = b.surname,
                               WeekScore = b.thisWeekScore,
                               MonthScore = b.thisMonthScore,
                               AllTimeScore = b.score
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

    }
}
