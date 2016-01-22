using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class CardSessionDTO 
    {
       public CardDTO BasicInfo { get; set; }
        public UserCardSessionInfo UserCardInfo { get; set; }
    }
}
