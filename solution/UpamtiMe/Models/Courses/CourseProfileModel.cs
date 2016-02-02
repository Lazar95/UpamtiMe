using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data;
using Data.DTOs;

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
        public byte[] Image { get; set; }
        public int LevelNumber { get; set; }
        public int? Favorite { get; set; }
        public List<Data.DTOs.LevelWithStatisticsDTO> Levels { get; set; }
        public List<Data.DTOs.LeaderboardEntryDTO> Leaderboard { get; set; }
        public Data.DTOs.CourseUsersStatisticsDTO Statistics { get; set; }

        public string CreatorUsername { get; set; }
        public int CreatorID { get; set; }

        public Options LearnOptions { get; set; }
        public Options ReviewOptions { get; set; }

        public static CourseProfileModel Load(int courseID, int? userID = null)
        {
            int numOpt = ConfigurationParameters.numOptions;
            int firstLearn = ConfigurationParameters.firstOptionLearn;
            int firstReview = ConfigurationParameters.firstOptionReview;
            int stepLearn = ConfigurationParameters.stepLearn;
            int stepReview = ConfigurationParameters.stepReview;

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
                Levels = Data.Levels.getLevels(courseID, userID, numOpt, firstLearn, firstReview, stepLearn, stepReview),
                Leaderboard = Data.Courses.getLeaderboard(courseID),
                LevelNumber = Data.Courses.countLevels(courseID),
                Statistics = null,
                CreatorID = course.creatorID,
                CreatorUsername = Data.Users.getUsername(course.creatorID),
                Description = course.description,
                Image = course.image == null ? null : course.image.ToArray()
            };

            if (userID != null)
            {
                cim.Statistics = Data.Courses.getUserCourseStatistics(courseID, userID.Value, course.numberOfCards);
                cim.Favorite = Data.Courses.getFavorite(courseID, userID.Value);
            }

            if (cim.Image == null || cim.Image.Length == 0)
            {
                cim.Image = Data.DefaultPictures.getAt(course.defaultImageID);
            }

            cim.LearnOptions = new Options
            {
                List = Data.Levels.getOptions(cim.Statistics.LearningStatistics.Unseen, numOpt, firstLearn, stepLearn),
                Default = 6
            };

            cim.ReviewOptions = new Options
            {
                List = Data.Levels.getOptions(cim.Statistics.LearningStatistics.Review, numOpt, firstReview, stepReview),
                Default = 20
            };

            return cim;

        }

    }
}