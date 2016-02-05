using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class FollowerDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string  FirstName { get; set; }
        public string  LastName { get; set; }
        public byte[] Avatar { get; set; }
        public bool? Follow { get; set; }
    }
}
