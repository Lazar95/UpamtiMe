using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe.Models
{
 public class UserIndexModel
    {
        public List<Data.DTOs.LeaderboardEntryDTO> Leaderboard { get; set; }
        public List<Data.DTOs.UserCourseDTO> Courses { get; set; }
        public Data.DTOs.StatisctisByDays Statistics { get; set; }


        public static UserIndexModel Load(int userID)
        {
            UserIndexModel uim = new UserIndexModel();
            uim.Leaderboard = Data.Users.getLeaderboard(userID);
            uim.Courses = Data.Courses.getCoursesOf(userID);

            return uim;
        }
        
        
    }

    
}