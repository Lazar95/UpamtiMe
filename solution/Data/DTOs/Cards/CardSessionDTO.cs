using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class CardSessionDTO 
    {
       public CardBasicDTO BasicInfo { get; set; }
       public CardUserDTO UserCardInfo { get; set; }
    }
}
