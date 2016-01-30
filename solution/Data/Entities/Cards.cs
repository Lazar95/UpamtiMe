using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Data.DTOs;

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

        public static bool checkJustQustion(string question, int courseID, int? cardID =  null)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Cards
                    from b in dc.Levels
                    from c in dc.Courses
                    where  c.courseID == courseID && a.levelID == b.levelID && b.courseID == c.courseID  && a.question == question && (cardID == null || a.cardID != cardID.Value) 
                    select a).Any();
        }

        public static bool checkJustQustion(Card card)
        {
            return checkJustQustion(card.question, getCourseOfCard(card.cardID), card.cardID);
        }

        public static bool checkJustQustion(CardBasicDTO cardBasic)
        {
            return checkJustQustion(cardBasic.Question, getCourseOfCard(cardBasic.CardID), cardBasic.CardID);
        }

      

        public static bool checkEmptyFields(Card card)
        {
            return ((card.question == String.Empty && card.image == null) || card.answer == String.Empty);
        }

        public static bool checkEmptyFields(CardBasicDTO cardBasic)
        {
            return ((cardBasic.Question == String.Empty && cardBasic.Image == null) || cardBasic.Answer == String.Empty);
        }

        public static void editCards(List<CardBasicDTO> cards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            
            foreach (CardBasicDTO card in cards)
            {
                if (checkJustQustion(card))
                    continue;
                if (checkEmptyFields(card))
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

        public static void addCards(List<CardBasicDTO> cards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (CardBasicDTO card in cards)
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

        public static List<Data.CardBasicDTO> getCardsFor(int levelID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Cards
                    where a.levelID == levelID
                    select new CardBasicDTO()
                    {
                        CardID = a.cardID,
                        Question = a.question,
                        Answer = a.answer,
                        Description = a.description,
                        Number = a.number,
                        Image = a.image == null ? null : a.image.ToArray(),
                        }).OrderBy(a=>a.Number).ToList();
        }

        public static List<Data.DTOs.CardCourseProfileDTO> getCards(int levelID, int? userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<Data.DTOs.CardCourseProfileDTO> returnValue = new List<CardCourseProfileDTO>();

            if (userID == null)
            {
                List<CardBasicDTO> l = getCardsFor(levelID, dc);
                foreach (CardBasicDTO card in l)
                {
                    returnValue.Add(new CardCourseProfileDTO {BasicInfo = card});
                }
            }
            else
            {
                returnValue = (from c in dc.Cards.Where(a=>a.levelID == levelID)
                    join u in dc.UsersCards.Where(a=>a.userID == userID.Value) on c.cardID equals u.cardID into uc
                    from u in uc.DefaultIfEmpty()
                    select new CardCourseProfileDTO
                    {
                        BasicInfo = new CardBasicDTO()
                        {
                            CardID = c.cardID,
                            Question = c.question,
                            Answer = c.answer,
                            Description = c.description,
                            Number = c.number,
                            Image = c.image == null ? null : c.image.ToArray(),
                        },
                        Ignore = u == null ? false : u.ignore,
                        UserCardInfo = u == null ? null : new CardUserDTO
                        {
                            UserCardID = u.usersCardID,
                            CorrectAnswers = u.correctAnswers,
                            WrongAnswers = u.wrongAnswers,
                            Combo = u.cardCombo,
                            LastSeen = u.lastSeen,
                            LastSeenMinutes = Convert.ToInt32(DateTime.Now.Subtract(u.lastSeen).TotalMinutes),
                            NextSee = u.nextSee,
                            NextSeeMinutes = Convert.ToInt32(DateTime.Now.Subtract(u.nextSee).TotalMinutes),
                            Goodness = u.goodness
                        }
                    }).OrderBy(a=>a.BasicInfo.Number).ToList();
            }

            return returnValue;
        }

        public static CorrectWrong CreateUserCard(List<CardUserDTO> cards, int userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            CorrectWrong cw = new CorrectWrong();
            cw.Correct = 0;
            cw.Wrong = 0;
            foreach (CardUserDTO card in cards)
            {
                cw.Correct += card.CorrectAnswers;
                cw.Wrong += card.WrongAnswers;
                UsersCard uc = new UsersCard
                {
                    userID = userID,
                    cardID = card.CardID,
                    lastSeen = DateTime.Now,
                    nextSee = DateTime.Now.AddMinutes(card.NextSeeMinutes),
                    cardCombo = card.Combo,
                    correctAnswers = card.CorrectAnswers,
                    wrongAnswers = card.WrongAnswers,
                    goodness = card.Goodness
                };
                dc.UsersCards.InsertOnSubmit(uc);
            }
            dc.SubmitChanges();
            return cw;
        }

        public static CorrectWrong UpdateUserCards(List<CardUserDTO> cards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            CorrectWrong cw = new CorrectWrong();
            cw.Correct = 0;
            cw.Wrong = 0;
            foreach (CardUserDTO card in cards)
            {
                cw.Correct += card.CorrectAnswers;
                cw.Wrong += card.WrongAnswers;
                UsersCard uc = (from a in dc.UsersCards where a.usersCardID == card.UserCardID select a).First();

                uc.cardCombo = card.Combo;
                uc.lastSeen = DateTime.Now;
                uc.nextSee = DateTime.Now.AddMinutes(card.NextSeeMinutes);
                uc.correctAnswers += card.CorrectAnswers;
                uc.wrongAnswers += card.WrongAnswers;
                uc.goodness = card.Goodness; //trenutno laza racuna to
            }
            dc.SubmitChanges();
            return cw;
        }

        public static void setIgnore(bool ignore, int cardID , int userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            //ignorise karticu koju je vec video
            var uc = (from a in dc.UsersCards where a.cardID == cardID && a.userID == userID select a);
            if (uc.Any())
            {
               uc.First().ignore = ignore;
            }
            else 
            {
                UsersCard newUC = new UsersCard
                {
                    ignore = ignore,
                    userID = userID,
                    cardID = cardID,
                    lastSeen = DateTime.Now,
                    nextSee = new DateTime(2300, 0, 0),
                    cardCombo = 0,
                    correctAnswers = 0,
                    wrongAnswers = 0,
                    goodness = 0
                };
                dc.UsersCards.InsertOnSubmit(newUC);
            }

            dc.SubmitChanges();
        }

    }
}
