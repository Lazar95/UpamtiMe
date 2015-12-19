using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return ( from a in dc.Subcategories select a).ToList();
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
                subcategoryID = subcategoryID,
                participantCount = 1, // creator is the only participant
                numberOfCards = numberOfCards,
                creatorID = creatorID,
            };

            dc.Courses.InsertOnSubmit(course);
            dc.SubmitChanges();

            return course;
        }

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
    }
}
