using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data;

namespace UpamtiMe.Models
{
    public class CourseIndexModel
    {
        public List<CourseDTO> Courses { get; set; }
        
        //search
        public string Search { get; set; }
        public int CategoryID { get; set; }
        public int SubcategoryID { get; set; } 
    }
}