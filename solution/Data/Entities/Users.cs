using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DTOs;

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
            
            List<LeaderboardEntryDTO> returnValue;
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

            returnValue = (from a in friendIDs
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
    }
}
