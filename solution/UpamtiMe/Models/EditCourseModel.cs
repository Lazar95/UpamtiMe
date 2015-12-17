using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe.Models
{
    public class EditCourseModel
    {
        public int CourseID{ get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int SubcategoryID { get; set; }
        public int NumberOfCards { get; set; }
        public List<Data.LevelsDTO> Levels { get; set; }

        public static EditCourseModel Load(int courseID)
        {
            Data.Course course = Data.Courses.getCourse(courseID);
            return new EditCourseModel
            {
                CategoryID = course.categoryID,
                CourseID = course.courseID,
                Name = course.name,
                NumberOfCards = course.numberOfCards,
                SubcategoryID = course.subcategoryID ?? 0,
                Levels = Data.Levels.getLevelsFor(courseID),
            };
        }
    }

}