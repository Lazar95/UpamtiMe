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
       public CardChallangeDTO CardChallange { get; set; }
      
    }

    public class CardChallangeDTO
    {
        public List<string> MultipleChoice { get; set; }
        public string Hangman { get; set; }
        public List<string> Scrabble { get; set; }
        public string Challenges { get; set; }
    }
}
