using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;

namespace Data.DTOs
{
    public class LoginTransporterDTO
    {
       
        public String Username { get; set; }
        //[DataType(DataType.Password)]
        public String Password { get; set; }
        public Boolean RememberMe { get; set; }
    }
}
