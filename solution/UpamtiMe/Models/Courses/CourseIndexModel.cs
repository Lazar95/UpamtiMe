using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data;
using Data.DTOs;

namespace UpamtiMe.Models
{
    public class CourseIndexModel
    {
        public List<CourseDTO> Courses { get; set; }
        public bool More { get; set; }

        
        //search
        public string Search { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? SubcategoryID { get; set; }
        public string SubcategoryName { get; set; }

        public List<Subcategory> AllSubcategories { get; set; }

        public static CourseIndexModel Load(string search, int? categoryID = null, int? subcategoryID = null)
        {
             int courseStartNo = ConfigurationParameters.CourseIndexStartCourseNumber;

            LoginDTO user = UserSession.GetUser();
            int? userID = null;
            if (user != null)
                userID = user.UserID;

            CourseIndexModel cim = new CourseIndexModel();
           
            List<CourseDTO> allCourses = Data.Courses.Search(search, categoryID, subcategoryID, userID);
            //upise u model prvih x
            cim.Courses = allCourses.Take(courseStartNo).ToList();

            if (courseStartNo >= allCourses.Count)
            {
                cim.More = false;
            }
            else
            {
                cim.More = true;
            }

            //ostale smesti u sessiju (i posle cita na more)
            UserSession.SetSearchCourses(allCourses);

            cim.AllSubcategories = Data.Courses.GetAllSubcategories();
            cim.Search = search;
            
            cim.SubcategoryID = subcategoryID;
            if (subcategoryID != null)
            {
                cim.SubcategoryName = Data.Courses.getSubcategoryName(cim.SubcategoryID.Value);
                categoryID =
                    (from a in cim.AllSubcategories where a.subcategoryID == cim.SubcategoryID.Value select a.categoryID)
                        .First();
            }
            else
            {
                cim.SubcategoryName = null;
            }


            cim.CategoryID = categoryID;
            cim.CategoryName = categoryID == null ? null : Data.Courses.getCategoryName(cim.CategoryID.Value);


            
            

            return cim;
        }
        
    }
}