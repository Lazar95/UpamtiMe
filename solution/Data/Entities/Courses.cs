using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.DTOs;

namespace Data
{
    public class Courses
    {
        public static Course getCourse(int courseID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Courses where a.courseID == courseID select a).First();
        }

        public static int countLevels(int courseID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Levels where a.courseID == courseID select a).Count();
        }

        public static List<Subcategory> GetAllSubcategories(DataClasses1DataContext dc = null)
        {
            dc = new DataClasses1DataContext();
            return (from a in dc.Subcategories select a).ToList();
        }


        public static Course addCourse(string name, int categoryID, int? subcategoryID, int numberOfCards, int creatorID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            Course course = new Course
            {
                name = name,
                categoryID = categoryID,
                subcategoryID = subcategoryID,
                participantCount = 0,
                numberOfCards = numberOfCards,
                creatorID = creatorID,
            };
            dc.Courses.InsertOnSubmit(course);
            dc.SubmitChanges();
            Users.enroll(creatorID, course.courseID);
            return course;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="userID"></param>
        /// <param name="numberOfCards">broj kartica u tom kursu, sluzi samo da ne bih pristupala bazi opet zbog te informacije</param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static CourseUsersStatisticsDTO getUserCourseStatistics(int courseID, int userID, int numberOfCards,
            DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            CourseUsersStatisticsDTO cus = new CourseUsersStatisticsDTO();
            cus.LearningStatistics = getUserLearningStatistics(courseID, userID, numberOfCards);
            UsersCourse uc =
                (from a in dc.UsersCourses where a.userID == userID && a.courseID == courseID select a).First();
            cus.LastPlayed = uc.lastPlayed;
            cus.StartDate = uc.startDate;

            return cus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="userID"></param>
        /// <param name="numberOfCards">broj kartica u tom kursu, sluzi samo da ne bih pristupala bazi opet zbog te informacije</param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static LearningStatisticsDTO getUserLearningStatistics(int courseID, int userID, int numberOfCards,
            DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();

            var lastNext = (from c in dc.Cards
                from l in dc.Levels
                from co in dc.Courses
                from u in dc.UsersCards
                where
                    c.levelID == l.levelID && l.courseID == co.courseID && u.cardID == c.cardID && u.ignore == false &&
                    co.courseID == courseID && u.userID == userID
                select new
                {
                    last = u.lastSeen,
                    next = u.nextSee,

                }).ToList();

            LearningStatisticsDTO returnValue = new LearningStatisticsDTO();

            returnValue.Total = numberOfCards;
            returnValue.Learned = 0;
            returnValue.Review = 0;

            foreach (var a in lastNext)
            {
                if (a.last != null)
                {
                    if (a.next > DateTime.Now)
                    {
                        returnValue.Learned++;
                    }
                    else
                    {
                        returnValue.Review++;
                    }
                }
            }

            returnValue.Unseen = returnValue.Total - returnValue.Learned - returnValue.Review;

            return returnValue;
        }


        //ovo nista ne valja, trebaju ti podaci kao sto je lastPlayed za kurs
        public static List<UserCourseDTO> getCoursesOf(int userID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();

            List<UserCourseDTO> retrunValue = new List<UserCourseDTO>();

            List<Course> courses = (from a in dc.UsersCourses
                from b in dc.Courses
                where a.userID == userID && a.courseID == b.courseID
                select b).ToList();

            foreach (Course course in courses)
            {
                CourseUsersStatisticsDTO cus = getUserCourseStatistics(course.courseID, userID, course.numberOfCards, dc);

                UserCourseDTO ucd = new UserCourseDTO
                {
                    CourseID = course.courseID,
                    CategoryID = course.categoryID,
                    SubcategoryID = course.subcategoryID,
                    Name = course.name,
                    LearningStatistics = cus.LearningStatistics
                };

                retrunValue.Add(ucd);
            }

            return retrunValue;
        }


        public static String getSubcategoryName(int subID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Subcategories where a.subcategoryID == subID select a.name).First();
        }

        public static String getCategoryName(int catID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Categories where a.categoryID == catID select a.name).First();
        }

        public static DateTime getStartDate(int courseID, int userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return
                (from a in dc.UsersCourses where a.userID == userID && a.courseID == courseID select a.startDate).First();
        }

        public static List<LeaderboardEntryDTO> getLeaderboard(int courseID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();

            List<int> usersList = (from a in dc.UsersCourses where a.courseID == courseID select a.userID).ToList();

            return (from a in usersList
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

        public static Course updateCourseInfo(int courseID, string name, int catID, int subID, int numCards,
            string description)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            Course course = (from a in dc.Courses where a.courseID == courseID select a).First();
            course.name = name;
            course.description = description;
            course.categoryID = catID;
            course.subcategoryID = subID;
            course.numberOfCards = numCards;
            dc.SubmitChanges();
            return course;
        }


        public static int getCardNuber(int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Courses where a.courseID == courseID select a).First().numberOfCards;
        }

        public static void editImage(int courseID, byte[] file)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            Course c = getCourse(courseID, dc);
            c.image = new System.Data.Linq.Binary(file);
            dc.SubmitChanges();
        }


        public static int updateUserCourse(int courseID, int userID, float score)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            UsersCourse uc =
                (from a in dc.UsersCourses where a.courseID == courseID && a.userID == userID select a).First();
            uc.score += score;

            if (uc.lastPlayed == null)
            {
                uc.thisWeekScore = score;
                uc.thisMonthScore = score;
            }
            else
            {
                uc.thisWeekScore += score;
                uc.thisMonthScore += score;
            }

            uc.lastPlayed = DateTime.Now;
            dc.SubmitChanges();
            return uc.usersCoursesID;
        }

        public static UserCourseStatistic findStatistics(int userCourseID, DateTime date,
            DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            var query =
                (from a in dc.UserCourseStatistics where a.userCourseID == userCourseID && a.date == date select a);
            if (query.Any())
                return query.First();
            else
                return null;
        }

        //vraca bool koji govori da li treba da se uveca streak ili ne
        public static bool updateStatistics(int courseID, int userID, float score, int learnedCards, int reviewedCards,
            int learnedCorrectAnswers, int learnedWrongAnswers, int reviewCorrectAnswers, int reviewWrongAnswers,
            int timeSpent)
        {
            bool streak;
            DataClasses1DataContext dc = new DataClasses1DataContext();
            int userCourseID = updateUserCourse(courseID, userID, score);
            UserCourseStatistic ucs = findStatistics(userCourseID, DateTime.Today.Date, dc);
            if (ucs == null)
            {
                streak = true;

                UserCourseStatistic newStat = new UserCourseStatistic
                {
                    userCourseID = userCourseID,
                    date = DateTime.Today.Date,
                    score = score,
                    learnedCards = learnedCards,
                    reviewedCards = reviewedCards,
                    sessionNo = 1, //prva sesija za taj dan
                    timeSpent = timeSpent,
                    learnedCorrectAnswers = learnedCorrectAnswers,
                    learnedWrongAnswers = learnedWrongAnswers,
                    reviewCorrectAnswers = reviewCorrectAnswers,
                    reviewWrongAnswers = reviewWrongAnswers
                };
                dc.UserCourseStatistics.InsertOnSubmit(newStat);

            }
            else
            {
                streak = false;

                ucs.score += score;
                ucs.learnedCards += learnedCards;
                ucs.reviewedCards += reviewedCards;
                ucs.sessionNo ++;
                ucs.timeSpent += timeSpent;
                ucs.learnedCorrectAnswers += learnedCorrectAnswers;
                ucs.learnedWrongAnswers += learnedWrongAnswers;
                ucs.reviewCorrectAnswers += reviewCorrectAnswers;
                ucs.reviewWrongAnswers += reviewWrongAnswers;
            }
            dc.SubmitChanges();

            return streak;
        }
       

        public static List<CourseDTO> Search(string name, int? categoryID = null, int? subcategoryID = null, int? userID = null,
            DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();

            return (from a in dc.Courses
                where // v lazo ovde je poredjenje stringova kad se pretrazuje kurs
                    (name == null || a.name.Contains(name)) && (categoryID == null || a.categoryID == categoryID) &&
                    (subcategoryID == null || a.subcategoryID == subcategoryID)
                select new CourseDTO
                {
                    CourseID = a.courseID,
                    Name = a.name,
                    CategoryID = a.categoryID,
                    CategoryName = getCategoryName(a.categoryID),
                    SubcategoryID = a.subcategoryID,
                    SubcategoryName = a.subcategoryID == null ? null : getSubcategoryName(a.subcategoryID.Value),
                    ParticipantCount = a.participantCount,
                    NumberOfCards = a.numberOfCards,
                    CreatorID = a.creatorID,
                    CreatorUsername = Users.getUsername(a.creatorID),
                    Rating = a.rating,
                    Image = a.image == null ? null : a.image.ToArray(),
                    Erolled = userID == null ? false : Users.enrolled(userID.Value, a.courseID)
                }).OrderByDescending(m => m.Rating).OrderByDescending(m => m.ParticipantCount).ToList();
        }

    }
}
