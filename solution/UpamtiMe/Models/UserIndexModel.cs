using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data.DTOs;

namespace UpamtiMe.Models
{
 public class UserIndexModel
    {
        public List<Data.DTOs.UserCourseDTO> Courses { get; set; }
        public Data.DTOs.StatisctisByDays Statistics { get; set; }
        public LearningStatisticsDTO LearningStatistics { get; set; }


        public static UserIndexModel Load(int userID)
        {
            UserIndexModel uim = new UserIndexModel();
            uim.Courses = Data.Courses.getCoursesOf(userID);

            //moze ovako ali je brze ako se samo sabere
            //uim.Statistics = Data.Users.GetStatisctisByDays(userID);

            uim.Statistics = new StatisctisByDays().AllZeros();
            uim.LearningStatistics = new LearningStatisticsDTO();
            foreach (UserCourseDTO course in uim.Courses)
            {
                uim.Statistics = uim.Statistics.Add(course.StatisctisByDays);
                uim.LearningStatistics = uim.LearningStatistics.Add(course.LearningStatistics);
            }

            

            return uim;
        }
        
        
    }

    
}