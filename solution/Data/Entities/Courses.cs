using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DTOs;

namespace Data
{
    public class Courses
    {
        public static Course getCourse(int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a
                in dc.Courses
                where a.courseID == courseID
                select a).First();
        }

        public static List<Subcategory> GetAllSubcategories()
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Subcategories select a).ToList();
        }


        public static Course addCourse(
            string name, int categoryID,
            int subcategoryID, int numberOfCards,
            int creatorID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();



            Course course = new Course
            {
                name = name,
                categoryID = categoryID,
                participantCount = 1, // creator is the only participant
                numberOfCards = numberOfCards,
                creatorID = creatorID,
            };

            if (subcategoryID != 0)
            {
                course.subcategoryID = subcategoryID;
            }


            dc.Courses.InsertOnSubmit(course);
            dc.SubmitChanges();

            return course;
        }


        //TODO lazo nzm sta ce ti ovo, podaci u CoursesDTO su nepotrebni za kurseve konkretnog coveka
        public static List<Data.CoursesDTO> getCoursesFor(int userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<Data.CoursesDTO>
                returnValue = (from a in dc.UsersCourses
                    join b in dc.Courses
                        on a.courseID equals b.courseID
                    where a.userID == userID
                    select new CoursesDTO()
                    {
                        CategoryID = b.categoryID,
                        CourseID = b.courseID,
                        CreatorID = b.creatorID,
                        Name = b.name,
                        ParticipantCount = b.participantCount,
                        SubcategoryID = b.subcategoryID ?? 0, // ako je null da upise nulu
                    }).ToList();

            // Za svaki preuzeti kurs preuzmi sve nivoe za taj kurs
            foreach (Data.CoursesDTO course in returnValue)
                course.Levels = Levels.getLevelsAndCardsFor(course.CourseID);

            return returnValue;
        }

        public static CourseUsersStatisticsDTO getUserCourseStatistics(Course course, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();

            var lastNext = (from cl in dc.CoursesLevels
                            from lc in dc.LevelsCards
                            from cu in dc.UsersCards
                            where
                                cl.courseID == course.courseID && cl.levelID == lc.levelID && lc.cardID == cu.userID &&
                                cu.ignore == false
                            select new
                            {
                                last = cu.lastSeen,
                                next = cu.nextSee,

                            }).ToList();

            CourseUsersStatisticsDTO returnValue = new CourseUsersStatisticsDTO();

            returnValue.Total = 0;
            returnValue.Learned = 0;
            returnValue.Review = 0;

            foreach (var a in lastNext)
            {
                returnValue.Total++;

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

            return returnValue;
        }

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
                CourseUsersStatisticsDTO cus = getUserCourseStatistics(course, dc);

                UserCourseDTO ucd = new UserCourseDTO
                {
                    CourseID = course.courseID,
                    CategoryID = course.categoryID,
                    SubcategoryID = course.subcategoryID,
                    Name = course.name,
                    CardsLearned = cus.Learned,
                    TotalCards = cus.Total,
                    ReviewCount = cus.Review
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

        public static List<LeaderboardEntryDTO> getLeaderboard(int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

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
    }

    
}
