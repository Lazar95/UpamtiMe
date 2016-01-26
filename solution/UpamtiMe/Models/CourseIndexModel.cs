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

        public static int brojKursevaKojiSePrikazuUStartu = ConfigurationParameters.CourseIndexStartCourseNumber;
        //search
        public string Search { get; set; }
        public int? CategoryID { get; set; }
        public int? SubcategoryID { get; set; }

        public static CourseIndexModel Load(string search, int? categoryID = null, int? subcategoryID = null)
        {
            LoginDTO user = UserSession.GetUser();
            int? userID = null;
            if (user != null)
                userID = user.UserID;

            CourseIndexModel cim = new CourseIndexModel();
           
            List<CourseDTO> allCourses = Data.Courses.Search(search, categoryID, subcategoryID, userID);
            //upise u model prvih x
            cim.Courses = allCourses.Take(brojKursevaKojiSePrikazuUStartu).ToList();

            if (brojKursevaKojiSePrikazuUStartu >= allCourses.Count)
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
            cim.SubcategoryID = subcategoryID;

            return cim;
        }
        
    }
}