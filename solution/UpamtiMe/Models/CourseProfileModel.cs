using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data;

namespace UpamtiMe.Models
{
    public class CourseProfileModel
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? SubcategoryID { get; set; }
        public string SubcategoryName { get; set; }
        public int NumberOfCards { get; set; }
        public int ParticipantCount { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public int LevelNumber { get; set; }
        public List<Data.DTOs.SimpleLevelDTO> Levels { get; set; }
        public List<Data.DTOs.LeaderboardEntryDTO> Leaderboard { get; set; }
        public Data.DTOs.CourseUsersStatisticsDTO Statistics { get; set; }

        public string CreatorUsername { get; set; }
        public int CreatorID { get; set; }

        public static CourseProfileModel Load(int courseID, int? userID = null)
        {
            Data.Course course = Data.Courses.getCourse(courseID);
            CourseProfileModel cim = new CourseProfileModel
            {
                CourseID = course.courseID,
                Name = course.name,
                NumberOfCards = course.numberOfCards,
                ParticipantCount = course.participantCount,
                CategoryID = course.categoryID,
                CategoryName = Data.Courses.getCategoryName(course.categoryID),
                SubcategoryID = course.subcategoryID,
                SubcategoryName = course.subcategoryID == null ? null : Data.Courses.getSubcategoryName(course.subcategoryID.Value),
                Levels = Data.Levels.getLevels(courseID, userID),
                Leaderboard = Data.Courses.getLeaderboard(courseID),
                LevelNumber = Data.Courses.countLevels(courseID),
                Statistics = null,
                CreatorID = course.creatorID,
                CreatorUsername = Data.Users.getUsername(course.creatorID),
                Description = course.description
            };

            if (userID != null)
            {
                cim.Statistics = Data.Courses.getUserCourseStatistics(courseID, userID.Value);
            }

            return cim;

        }

    }
}