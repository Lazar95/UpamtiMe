using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Data.DTOs;

namespace UpamtiMe.Models
{
    public class UserProfileModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public float Score { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime DateRegistered { get; set; }
        public int totalCardsSeen { get; set; }
        public string Bio { get; set; }
        public int Streak { get; set; }
        public string Location { get; set; }
        public byte[] Image { get; set; }
        public List<Data.Achievement> Achievements { get; set; }

        public List<CourseDTO> Courses { get; set; }
        public List<FollowerDTO> Followers { get; set; }
        public List<FollowerDTO> Following { get; set; }

        public int numCourses { get; set; }


        public static UserProfileModel Load(int profileID, int? userID = null)
        {
            Data.User u = Data.Users.GetUser(profileID);
            UserProfileModel returnValue = new UserProfileModel
            {
                UserID = u.userID,
                Username = u.username,
                Name = u.name,
                Surname = u.surname,
                Location = u.location,
                Score = u.score,
                Bio = u.bio,
                LastLogin = u.lastLogin,
                DateRegistered = u.dateRegistered,
                totalCardsSeen = u.totalCardsSeen,
                Streak = u.streak,
                Image =  u.avatar?.ToArray(),
                Achievements = null
            };

            if (returnValue.Image == null || returnValue.Image.Length == 0)
            {
                returnValue.Image = Data.DefaultPictures.getAt(u.defaultAvatarID);
            }

            returnValue.Courses = Data.Courses.GetCoursesOfUser(profileID, userID);
            returnValue.Following = Data.Users.GetFollowing(profileID, userID);
            returnValue.Followers = Data.Users.GetFollowers(profileID, userID);

            returnValue.numCourses = Data.Users.getUserCourses(profileID).Count;

            return returnValue;
        }
    }
}