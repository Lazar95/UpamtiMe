using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Cards
    {
        //lazino staro nesto
        /////////////////////////
        //public static int countCards(int levelID)
        //{
        //    DataClasses1DataContext dc = new DataClasses1DataContext();
        //    return (from a in dc.LevelsCards
        //            where a.levelID == levelID
        //            select a).Count();
        //}

        public static List<Data.CardDTO> getCardsFor(int levelID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.LevelsCards
                    join b in dc.Cards
                    on a.cardID equals b.cardID
                    where a.levelID == levelID
                    select new CardDTO()
                    {
                        CardID = b.cardID,
                        Question = b.question,
                        Answer = b.answer,
                        Descrption = b.description,
                        Number = a.number
                        // Image = b.image,
                        // Ovo nece zbog tipa, nebitno sad
                    }).ToList();
        }

    }
}
