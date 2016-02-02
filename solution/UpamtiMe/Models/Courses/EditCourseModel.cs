using Data;
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
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public List<Data.LevelWithCardsDTO> Levels { get; set; }
        public List<Subcategory> AllSubcategories { get; set; }

        public static EditCourseModel Load(int courseID)
        {
            Data.Course course = Data.Courses.getCourse(courseID);

            if(course.creatorID != UserSession.GetUserID())
                throw new Exception("nije on kreator");

            EditCourseModel returnValue =  new EditCourseModel
            {
                CategoryID = course.categoryID,
                CourseID = course.courseID,
                Name = course.name,
                NumberOfCards = course.numberOfCards,
                Description = course.description,
                Image = course.image?.ToArray(),
                SubcategoryID = course.subcategoryID ?? 0,
                Levels = Data.Levels.getLevelsAndCardsFor(courseID),
                AllSubcategories = Data.Courses.GetAllSubcategories()
            };

            if (returnValue.Image == null || returnValue.Image.Length == 0)
            {
                returnValue.Image = Data.DefaultPictures.getAt(course.defaultImageID);
            }

            return returnValue;
            
        }
    }

}