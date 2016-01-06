using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe.Models
{
    public class CourseProfileModel
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int? SubcategoryID { get; set; }
        public int NumberOfCards { get; set; }
        public int ParticipantCount { get; set; }
        public List<Data.LevelsDTO> Levels { get; set; }
        public List<Data.DTOs.LeaderboardEntryDTO> Leaderboard { get; set; }
        public Data.DTOs.CourseUsersStatisticsDTO Statistics { get; set; }

        public static CourseProfileModel Load(int courseID, int? userID = null)
        {
            Data.Course course = Data.Courses.getCourse(courseID);
            CourseProfileModel cim = new CourseProfileModel
            {
                CategoryID = course.categoryID,
                CourseID = course.courseID,
                Name = course.name,
                NumberOfCards = course.numberOfCards,
                ParticipantCount = course.participantCount,
                SubcategoryID = course.subcategoryID,
                Levels = Data.Levels.getLevelsAndCardsFor(courseID),
                Leaderboard = Data.Courses.getLeaderboard(courseID),
                Statistics = null
            };

            if (userID != null)
            {
                cim.Statistics = Data.Courses.getUserCourseStatistics(course);
            }

            return cim;

        }

    }
}