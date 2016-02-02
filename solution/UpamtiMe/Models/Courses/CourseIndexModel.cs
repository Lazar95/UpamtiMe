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
           
            cim.Search = search;
            cim.CategoryID = categoryID;
            cim.CategoryName = categoryID == null ? null : Data.Courses.getCategoryName(cim.CategoryID.Value);
            cim.SubcategoryID = subcategoryID;
            cim.SubcategoryName = subcategoryID == null? null : Data.Courses.getSubcategoryName(cim.SubcategoryID.Value);

            cim.AllSubcategories = Data.Courses.GetAllSubcategories();

            return cim;
        }
        
    }
}