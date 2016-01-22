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
           
            if (levelID != null)
            {
                sm.Cards = (from c in dc.Cards
                    where
                        c.levelID == levelID.Value &&
                        !dc.UsersCards.Any(a => a.cardID == c.cardID && a.userID == userID)
                    select new CardSessionDTO
                    {
                        Question = c.question,
                        Answer = c.answer,
                        Description = c.description,
                        Image = c.image.ToArray(),
                        Number = c.number,
                    }).OrderBy(a=>a.Number).Take(numberOfCards.Value).ToList();
            }
            else
            {
                sm.Cards = (from c in dc.Cards
                            from l in dc.Levels
                            from u in dc.UsersCards
                            where
                                u.userID == userID && l.courseID == courseID && c.levelID == levelID.Value &&
                                u.cardID == c.cardID && u.ignore == false
                            select new CardSessionDTO
                            {
                                Question = c.question,
                                Answer = c.answer,
                                Description = c.description,
                                Image = c.image.ToArray(),
                                Number = l.number*1000 + c.number,
                                LastSeen = u.lastSeen,
                                NextSee = u.nextSee,
                                Combo = u.cardCombo,
                                CorrectAnswers = u.correctAnswers,
                                WrongAnswers = u.wrongAnswers
                            }).OrderBy(a => a.Number).Take(numberOfCards.Value).ToList();

            }

            return sm;
        }

        public SessionModel LoadReviewSession(int userID, int courseID, int? levelID)
        {
            SessionModel sm = new SessionModel();


            return sm;
        }
    }
}