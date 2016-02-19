using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;
using Data;
using Data.DTOs;

namespace UpamtiMe.Models
{
    public class SessionModel
    {
        public List<CardSessionDTO> Cards { get; set; }
        public int CourseID { get; set; }
        public string Link { get; set; }

        public static SessionModel LoadLearningSession(int userID, int courseID, int? levelID, int? numberOfCards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            //ovo je zapravo defaultna vrenost, mora ovako jer inace mora da bude compile-time constant
            if (numberOfCards == null)
                numberOfCards = ConfigurationParameters.LearningSessionCardNumber;

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
            sm.CourseID = courseID;
            sm.Cards = (from c in dc.Cards
                where
                    c.levelID == levelID.Value &&
                    !dc.UsersCards.Any(a => a.cardID == c.cardID && a.userID == userID)
                select new CardSessionDTO
                {
                    BasicInfo = new CardBasicDTO
                    {
                        CardID = c.cardID,
                        Question = c.question,
                        Answer = c.answer,
                        Description = c.description,
                        Image = c.image == null ? null : c.image.ToArray(),
                        Number = c.number,
                    },
                    CardChallange = new CardChallangeDTO
                    {
                        MultipleChoice = new List<string>(new string[] { c.answer, c.answer, c.answer, c.answer }),
                        Hangman = c.answer,
                        Scrabble = Regex.Split(c.answer, string.Empty).ToList(),
                        Challenges = ConfigurationParameters.ChallengesLearn,
                    }
                }).OrderBy(a=>a.BasicInfo.Number).Take(numberOfCards.Value).ToList();
            
            return sm;
        }

        public static SessionModel LoadReviewSession(int userID, int courseID, int? levelID, int? numberOfCards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            if (numberOfCards == null)
                numberOfCards = ConfigurationParameters.ReviewSessionCardNumber;


            SessionModel sm = new SessionModel();
            sm.CourseID = courseID;

            sm.Cards = (from c in dc.Cards
                        from l in dc.Levels
                        from u in dc.UsersCards
                        where u.userID == userID && u.cardID == c.cardID && ((levelID != null && c.levelID == levelID ) || (levelID == null && c.levelID == l.levelID)) && u.ignore == false && u.nextSee < DateTime.Now && (levelID != null || l.courseID == courseID)
                        select new CardSessionDTO
                        {
                            UserCardInfo = new CardUserDTO()
                            {
                                Combo = u.cardCombo,
                                CorrectAnswers = u.correctAnswers,
                                WrongAnswers = u.wrongAnswers,
                                LastSeen = u.lastSeen,
                                SinceSeen = Convert.ToInt32(DateTime.Now.Subtract(u.lastSeen).TotalMinutes),
                                NextSee = u.nextSee,
                                SincePlan = Convert.ToInt32(DateTime.Now.Subtract(u.nextSee).TotalMinutes),
                                UserCardID = u.usersCardID,
                            },
                            BasicInfo = new CardBasicDTO
                            {
                                Question = c.question,
                                Answer = c.answer,
                                Description = c.description,
                                Image = c.image == null ? null : c.image.ToArray(),
                                Number = c.number,
                            },
                            CardChallange = new CardChallangeDTO()
                            {
                                MultipleChoice = new List<string>(new string[] { c.answer, c.answer, c.answer, c.answer }),
                                Hangman = c.answer,
                                Scrabble = Regex.Split(c.answer, string.Empty).ToList(),
                                Challenges = (u.goodness > 0.6 ? "" : "mutiple;")  +  ConfigurationParameters.ChallengesReview 
                            }
                        }).OrderBy(a => a.UserCardInfo.NextSee).Take(numberOfCards.Value).ToList();
            
            return sm;
        }

        
        public static SessionModel LoadLinkySession(int userID, int courseID, int? levelID, int? numberOfCards)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            if (numberOfCards == null)
                numberOfCards = ConfigurationParameters.LinkySessionCardNumber;

            SessionModel sm = new SessionModel();
            sm.CourseID = courseID;
            if (levelID != null)
            {
                sm.Cards = (from c in dc.Cards
                            from u in dc.UsersCards
                            where u.userID == userID &&  u.cardID == c.cardID && c.levelID == levelID && u.ignore == false && u.nextSee > DateTime.Now 
                            select new CardSessionDTO
                            {
                                BasicInfo = new CardBasicDTO
                                {
                                    Question = c.question,
                                    Answer = c.answer,
                                    Description = c.description
                                }
                            }).Take(numberOfCards.Value).ToList();
            }
            else
            {
                sm.Cards = (from c in dc.Cards
                            from l in dc.Levels
                            from u in dc.UsersCards
                            where u.userID == userID && u.cardID == c.cardID && c.levelID == l.levelID && l.courseID == courseID && u.ignore == false && u.nextSee > DateTime.Now 
                            select new CardSessionDTO
                            {
                                BasicInfo = new CardBasicDTO
                                {
                                    Question = c.question,
                                    Answer = c.answer,
                                    Description = c.description
                                }
                            }).Take(numberOfCards.Value).ToList();
            }

            sm.Cards = sm.Cards.GroupBy(a => a.BasicInfo.Answer).Select(a => a.First()).ToList();

            return sm;
        }
    }
}