using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Data;

namespace UpamtiMe.Models
{
    public class CreateNewCourseModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int CategoryID { get; set; }
        public int SubcategoryID { get; set; }
        public int NumberOfCards { get; set; }
        public int CreatorID { get; set; }
        public List<Subcategory> AllSubcategories { get; set; } 

        public CreateNewCourseModel()
        {
            AllSubcategories = Courses.GetAllSubcategories();
        }
    }
}