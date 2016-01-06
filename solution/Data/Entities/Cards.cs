using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Cards
    {

        public static Card getCard(int cardID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Cards where a.cardID == cardID select a).First();
        }
      

        public static int getCourseOfCard(int cardID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Cards
                from b in dc.Levels
                from c in dc.Courses
                where b.courseID == c.courseID && a.levelID == b.levelID && a.cardID == cardID
                select c.courseID).First();
        }

        public static void deleteCards(List<int> cardIDs)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            foreach (int card in cardIDs)
            {
                Card c = Cards.getCard(card, dc);

                var usrs = (from a in dc.UsersCards where a.cardID == card select a);
                foreach (UsersCard uc in usrs)
                {
                    dc.UsersCards.DeleteOnSubmit(uc);
                }
               
                dc.Cards.DeleteOnSubmit(c);
                dc.SubmitChanges();
            }
        }

        public static bool checkJustQustion(string question, int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Cards
                    from b in dc.Levels
                    from c in dc.Courses
                    where  a.levelID == b.levelID && b.courseID == c.courseID  && a.question == question
                    select a).Any();
        }

        public static bool checkJustQustion(Card card)
        {
            return checkJustQustion(card.question, getCourseOfCard(card.cardID));
        }

        public static bool checkJustQustion(CardDTO card)
        {
            return checkJustQustion(card.Question, getCourseOfCard(card.CardID));
        }

        public static bool checkQustion(CardDTO card)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Cards
                    from b in dc.Levels
                    from c in dc.Courses
                    where a.levelID == b.levelID && b.courseID == c.courseID && a.question == card.Question && a.cardID != card.CardID
                    select a).Any();
        }

        public static bool checkEmptyFields(Card card)
        {
            return ((card.question == String.Empty && card.image == null) || card.answer == String.Empty);
        }

        public static bool checkEmptyFields(CardDTO card)
        {
            return ((card.Question == String.Empty && card.Image == null) || card.Answer == String.Empty);
        }

        public static void editCards(List<CardDTO> cards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            
            foreach (CardDTO card in cards)
            {
                if(checkQustion(card))
                    continue;
                if(checkEmptyFields(card))
                    continue;

                //ovo deluje besmislneno ali mislim da bez ovog nece da radi
                Card c = (from a in dc.Cards where a.cardID == card.CardID select a).First();
                c.question = card.Question;
                c.answer = card.Answer;
                c.description = card.Description;
                c.image = card.Image;
                dc.SubmitChanges();
                
            }
        }

        public static void addCards(List<CardDTO> cards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (CardDTO card in cards)
            {
                if (checkJustQustion(card.Question, Levels.getCourse(card.LevelID)))
                    continue;
                if (checkEmptyFields(card))
                    continue;

                Card newCard = new Card
                {
                    question = card.Question,
                    answer = card.Answer,
                    levelID = card.LevelID,
                    number = card.Number,
                    description = card.Description,
                    image = card.Image
                };
                dc.Cards.InsertOnSubmit(newCard);
                dc.SubmitChanges();
            }
        }

        public static List<Data.CardDTO> getCardsFor(int levelID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Cards
                    where a.levelID == levelID
                    select new CardDTO()
                    {
                        CardID = a.cardID,
                        Question = a.question,
                        Answer = a.answer,
                        Description = a.description,
                        Number = a.number,
                        //Image = a.image,
                        // Ovo nece zbog tipa, nebitno sad
                    }).ToList();
        }

    }
}
