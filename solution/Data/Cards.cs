using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Cards
    {
        public static int countCards(int levelID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.LevelsCards
                    where a.levelID == levelID
                    select a).Count();
        }

        public static List<Data.CardsDTO> getCardsFor(int levelID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.LevelsCards
                    join b in dc.Cards
                    on a.cardID equals b.cardID
                    where a.levelID == levelID
                    select new CardsDTO()
                    {
                        // Zar ovo ne treba da bude u konstruktoru klase CardsDTO?
                        cardID = b.cardID,
                        Question = b.question,
                        Answer = b.answer,
                        Descrption = b.description,
                        // Image = b.image,
                        // Ovo nece zbog tipa, nebitno sad
                    }).ToList();
        }

    }
}
