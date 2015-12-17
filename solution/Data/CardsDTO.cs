using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CardsDTO
    {
        public int cardID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Descrption { get; set; }
        public byte[] Image { get; set; }

        public static Card GetCard(int cardID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a
                    in dc.Cards
                    where a.cardID == cardID
                    select a).First();
        }
    }
}
