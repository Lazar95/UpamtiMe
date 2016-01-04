using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class UserCourseDTO
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int? SubcategoryID { get; set; }
        public int TotalCards { get; set; }
        public int CardsLearned { get; set; }
        public int ReviewCount { get; set; }
    }
}
