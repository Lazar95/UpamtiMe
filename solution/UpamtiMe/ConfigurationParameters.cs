using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe
{
    public static class ConfigurationParameters
    {
        public static int CoursesSearchInfiniteScrollBlockSize = 2;
        public static int CourseIndexStartCourseNumber = 1;

        public static int CoursesUserIndexInfiniteScrollBlockSize = 2;
        public static int UserIndexStartCourseNumber = 1;

        public static int LearningSessionCardNumber = 6;
        public static int ReviewSessionCardNumber = 20;
        public static int LinkySessionCardNumber = 100;
        public static int LinkyLimit = 5;

        //dropdown
        public static int numOptions = 4;
        public static List<int> desiredLearningOptions = new List<int> { 3, 6, 9, 12, 15 };
        public static int minimumLeftToLearn = 3;
        public static List<int> desiredReviewOptions = new List<int> { 10, 15, 20, 30, 50 };
        public static int minimumLeftToReview = 5;


        //ovo nisam stavila da ne bih prenatrpavala kod, tesko da ce ikad da se promeni
        // static int StatisticsTimeSpan = 30;
    }
}