using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class CardCourseProfileDTO 
    {
        public UserCardSessionInfo UserCard { get; set; }
        public CardDTO BasicInfo { get; set; }
        public bool Ignore { get; set; }
    }
}
