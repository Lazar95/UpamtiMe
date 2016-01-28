using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data;
using Data.DTOs;

namespace UpamtiMe.Models
{
 public class UserIndexModel
    {
        public List<Data.DTOs.UserCourseDTO> Courses { get; set; }
        public Data.DTOs.StatisctisByDays Statistics { get; set; }
        public LearningStatisticsDTO LearningStatistics { get; set; }
        public bool More { get; set; }


        public static UserIndexModel Load(int userID)
        {
            int courseStartNo = ConfigurationParameters.UserIndexStartCourseNumber;

            List<Course> allCourses = Data.Courses.getSortedCoursesOf(userID);
            //smesti ih sve u sesiju da bi mogla da ih citas na more
            UserSession.SetUserCourses(allCourses);

            //pocetnih x smesti u model
            UserIndexModel uim = new UserIndexModel();
            List<Course> modelList = allCourses.Take(courseStartNo).ToList();
            uim.Courses = Data.Courses.CreateUserCourseDTOs(userID, modelList);

            if (courseStartNo >= allCourses.Count)
            {
                uim.More = false;
            }
            else
            {
                uim.More = true;
            }


            // kad smo vadili sve podatke odjednom
            //uim.Statistics = new StatisctisByDays().AllZeros();
            //uim.LearningStatistics = new LearningStatisticsDTO();
            //foreach (UserCourseDTO course in uim.Courses)
            //{
            //    uim.Statistics = uim.Statistics.Add(course.StatisctisByDays);
            //    uim.LearningStatistics = uim.LearningStatistics.Add(course.LearningStatistics.LearningStatistics);
            //}

            //mogu i ovo da izvucem iz sidebara ali je bolje da tamo ucitvam 7 a ovde 30, ili mozda nije?
            //uim.Statistics = Data.Users.GetStatisctisByDays(userID);

            uim.Statistics = UserSession.GetSidebar().Statistics;
            uim.LearningStatistics = UserSession.GetSidebar().LearningStatistics;

            return uim;
        }
        
        
    }

    
}