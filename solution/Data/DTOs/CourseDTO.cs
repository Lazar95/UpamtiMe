using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class CourseDTO
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? SubcategoryID { get; set; }
        public string SubcategoryName { get; set; }
        public int ParticipantCount { get; set; }
        public int NumberOfCards { get; set; }
        public int CreatorID { get; set; }
        public string CreatorUsername { get; set; }
        public int? Rating { get; set; }
        //description nema 
        public byte[] Image { get; set; }
    }
}
