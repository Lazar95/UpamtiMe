using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using Data;
using Data.DTOs;

namespace UpamtiMe.Models
{
    public class SessionModel
    {
        public List<Data.DTOs.CardSessionDTO> Cards { get; set; }

        public static SessionModel LoadLearningSession(int userID, int courseID, int? levelID, int? numberOfCards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            if (numberOfCards == null)
                numberOfCards = 6;

            if (levelID == null)
            {
                levelID = (from c in dc.Cards
                    from l in dc.Levels
                    where
                       l.courseID == courseID && c.levelID == l.levelID &&
                        !dc.UsersCards.Any(a => a.cardID == c.cardID && a.userID == userID)
                           select new {id = l.levelID, no = l.number}).OrderBy(a => a.no).First().id;
            }

            SessionModel sm = new SessionModel();
            
            sm.Cards = (from c in dc.Cards
                where
                    c.levelID == levelID.Value &&
                    !dc.UsersCards.Any(a => a.cardID == c.cardID && a.userID == userID)
                select new CardSessionDTO
                {
                    BasicInfo = new CardDTO
                    {
                        Question = c.question,
                        Answer = c.answer,
                        Description = c.description,
                        Image = c.image.ToArray(),
                        Number = c.number,
                    }
                }).OrderBy(a=>a.BasicInfo.Number).Take(numberOfCards.Value).ToList();

            return sm;
        }

        public static SessionModel LoadReviewSession(int userID, int courseID, int? levelID, int? numberOfCards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            if (numberOfCards == null)
                numberOfCards = 20;

            if (levelID == null)
            {
                levelID = (from c in dc.Cards
                           from l in dc.Levels
                           from u in dc.UsersCards
                           where u.cardID == c.cardID && c.levelID == l.levelID && l.courseID == courseID && u.ignore == false && u.nextSee < DateTime.Now
                           select new { id = l.levelID, no = l.number }).OrderBy(a => a.no).First().id;
            }

            SessionModel sm = new SessionModel();

            sm.Cards = (from c in dc.Cards 
                        from u in dc.UsersCards
                        where u.cardID == c.cardID && c.levelID == levelID.Value && u.ignore == false && u.nextSee < DateTime.Now
                        select new CardSessionDTO
                        {
                            UserCardInfo = new UserCardSessionInfo
                            {
                                Combo = u.cardCombo,
                                CorrectAnswers = u.correctAnswers,
                                WrongAnswers = u.wrongAnswers,
                                LastSeen = u.lastSeen,
                                NextSee = u.nextSee,
                                UserCardID = u.usersCardID,
                            },
                            BasicInfo = new CardDTO
                            {
                                Question = c.question,
                                Answer = c.answer,
                                Description = c.description,
                                Image = c.image.ToArray(),
                                Number = c.number,
                            }
                        }).OrderBy(a => a.BasicInfo.Number).Take(numberOfCards.Value).ToList();

            return sm;
        }

        
        public static SessionModel LoadLinkySession(int userID, int courseID, int? levelID, int? numberOfCards)
        {
            int linkyLimit = 5;
            DataClasses1DataContext dc = new DataClasses1DataContext();

            if (numberOfCards == null)
                numberOfCards = 500;

            SessionModel sm = new SessionModel();

            if (levelID != null)
            {
                sm.Cards = (from c in dc.Cards
                            from u in dc.UsersCards
                            where u.cardID == c.cardID && c.levelID == levelID && u.ignore == false && u.nextSee < DateTime.Now
                            select new CardSessionDTO
                            {
                                BasicInfo = new CardDTO
                                {
                                    Question = c.question,
                                    Answer = c.answer,
                                    Description = c.description
                                }
                            }).Take(numberOfCards.Value).ToList();

                if (sm.Cards.Count < 5)
                    return null; //ovde bi mogo i da se baci odgovarajuci exception
            }
            else
            {
                sm.Cards = (from c in dc.Cards
                            from u in dc.UsersCards
                            where u.cardID == c.cardID &&  u.ignore == false && u.nextSee < DateTime.Now
                            select new CardSessionDTO
                            {
                                BasicInfo = new CardDTO
                                {
                                    Question = c.question,
                                Answer = c.answer,
                                Description = c.description
                                }
                            }).Take(numberOfCards.Value).ToList();

                if (sm.Cards.Count < 5)
                    return null; //ovde bi mogo i da se baci odgovarajuci exception
            }
            

            return sm;
        }
    }
}