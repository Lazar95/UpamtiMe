using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe.Models
{
    public class ProfilePageModel
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public float Score { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime DateRegistered { get; set; }
        public int totalCardsLearned { get; set; }
        public string Bio { get; set; }
        public int Streak { get; set; }
        public string Location { get; set; }
        public List<Data.Achievement> Achievements { get; set; }

        public static ProfilePageModel Load(int userID)
        {
            Data.User u = Data.Users.GetUser(userID);
            return new ProfilePageModel
            {
                UserID = u.userID,
                Name = u.name,
                Surname = u.surname,
                Location = u.location,
                Score = u.score,
                Bio = u.bio,
                LastLogin = u.lastLogin,
                DateRegistered = u.dateRegistered,
                totalCardsLearned = u.totalCardsLearned,
                Streak = u.streak,
                Achievements = null
            };
        }
    }
}