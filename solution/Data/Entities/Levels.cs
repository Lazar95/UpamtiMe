using Data.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Levels
    {

        public static int countCards(int levelID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Cards where a.levelID == levelID select a).Count();
        }

        public static List<int> getAllCards(int levelID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Cards where a.levelID == levelID select a.cardID).ToList();
        } 
     

        public static void deleteLevels(List<int> levelIDs)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            foreach (int level in levelIDs)
            {
                List<int> cards = getAllCards(level);
                Cards.deleteCards(cards);
                Level l = (from a in dc.Levels where a.levelID == level select a).First();
                dc.Levels.DeleteOnSubmit(l);
            }
            dc.SubmitChanges();
        }

        //proveri da li u istom kursu postoji nivo sa istim imenom a da nije bas taj nivo
        public static bool checkName(string name, int courseID, int levelID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Levels
                    where a.courseID == courseID && a.name == name && a.levelID != levelID
                    select a).Any();
        }

        public static bool checkName(Level level)
        {
            return checkName(level.name, level.courseID, level.levelID);
        }

        public static void addLevel(int courseID, LevelWithCardsDTO level)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            if (checkName(level.Name, courseID, -1))
                return;
            if(level.Cards == null)
                return;
            

            Level newLevel = new Level
            {
                name = level.Name,
                type = level.Type,
                number = level.Number,
                courseID = courseID,
                icon = level.Icon,
                color = level.Color
            };
            dc.Levels.InsertOnSubmit(newLevel);
            dc.SubmitChanges();
            

            foreach (CardBasicDTO card in level.Cards)
                card.LevelID = newLevel.levelID;

            Cards.addCards(level.Cards);
        
            
        }

        public static void addLevels(int courseID, List<LevelWithCardsDTO> levels)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (LevelWithCardsDTO level in levels)
            {
                addLevel(courseID, level);
            }
        }

        public static void editLevels(int courseID, List<LevelBasicDTO> levels)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (LevelBasicDTO level in levels)
            {
                if (checkName(level.Name, courseID, level.LevelID))
                    continue;

                //opet ovo besmisleno
                Level l = GetLevel(level.LevelID, dc);
                l.name = level.Name;
                l.type = level.Type;
                l.number = level.Number;
                l.icon = level.Icon;
                l.color = level.Color;
                dc.SubmitChanges();
            }
        }


        public static Data.Level GetLevel(int levelID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();
            return (from a in dc.Levels where a.levelID == levelID select a).First();
        }

        public static int getCourse(int levelID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Levels where a.levelID == levelID select a.courseID).First();
        }


        public static LearningStatisticsDTO getLevelStatistics(int levelID, int userID, DataClasses1DataContext dc = null)
        {
            dc = dc ?? new DataClasses1DataContext();

            var lastNext = (from c in dc.Cards
                            from l in dc.Levels
                            from u in dc.UsersCards
                            where c.cardID == u.cardID && c.levelID == l.levelID && l.levelID == levelID && u.userID == userID
                            select new
                            {
                                last = u.lastSeen,
                                next = u.nextSee,
                            }).ToList();

            LearningStatisticsDTO returnValue = new LearningStatisticsDTO();

            returnValue.Total = (from a in dc.Cards where a.levelID == levelID select a).Count();
            returnValue.Learned = 0;
            returnValue.Review = 0;

            foreach (var a in lastNext)
            {
                if (a.last != null)
                {
                    if (a.next > DateTime.Now)
                    {
                        returnValue.Learned++;
                    }
                    else
                    {
                        returnValue.Review++;
                    }
                }
            }

            returnValue.Unseen = returnValue.Total - returnValue.Learned - returnValue.Review;

            return returnValue;
        }
       

        public static List<LevelWithStatisticsDTO> getLevels(int courseID, int? userID, int numOptions, int firstOptLearn, int firstOptReview, int stepLearn, int stepReview)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<LevelWithStatisticsDTO> returnValue = (
                    from l in dc.Levels
                    where l.courseID == courseID
                    select new LevelWithStatisticsDTO
                    {
                        LevelID = l.levelID,
                        Name = l.name,
                        Number = l.number,
                        Type = l.type,
                        Icon = l.icon,
                        Color = l.color,
                        CardNumber = (from a in dc.Cards where a.levelID == l.levelID select a).Count(),
                        LearningStatistics = null
                    }).OrderBy(a=>a.Number).ToList();

            if (userID != null)
            {
                foreach (LevelWithStatisticsDTO level in returnValue)
                {
                    level.LearningStatistics = getLevelStatistics(level.LevelID, userID.Value, dc);
                    level.LearnOptions = new Options
                    {
                        List = Levels.getOptions(level.LearningStatistics.Unseen, numOptions, firstOptLearn, stepLearn),
                        Default = 6
                    };

                    level.ReviewOptions = new Options
                    {
                        List = getOptions(level.LearningStatistics.Review, numOptions, firstOptReview, stepReview),
                        Default = 20
                    };
                }

            }
            
            

            return returnValue;
        }



        // parametri (broj nenaucenih kartica u nivou, broj opcija u dropdown, prva opcija, korak)
        public static List<int> getOptions(int totalCards, int optionNum, int firstOption, int step)
        {
            //ja ovde napravim listu sa optionNum nula, nzm kako ti radi f-ja, ako hoces da lista inicijalno bude prazna mozes da koristis options.Add(nesto) da bi dodao nesto na kraj
            List<int> options = new List<int>(new int[optionNum]); // niz opcija koji ce funkcija da vrati
            int  maxOptionsNum = optionNum; // vracamo najvise 4 opcije
            int temp = totalCards;
            int i;
            int minimumCards = firstOption; // prva opcija, nikad ne sme manje od ovoga da ostane, samo ako ih nije ni bilo u nivou
            int stepSize = step; // uvecava se za ovoliko po opciji
            for (i = 0; i < maxOptionsNum && temp >= minimumCards; i++)
            {
                options[i] = minimumCards + i * stepSize;
                if (i == 0)
                    temp -= minimumCards;
                else
                    temp -= stepSize;
            }
            if (i > 0)
            {
                if (temp <= minimumCards)
                {
                    options[i - 1] += temp;
                }
            }
            else {
                options[i] = temp;
            }

            if (options.Count > 0)
                options.RemoveAll(m => m == 0);

            return options;
        }

        public static List<Data.LevelWithCardsDTO> getLevelsAndCardsFor(int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<Data.LevelWithCardsDTO> returnValue = (from a in dc.Levels
                                                where a.courseID == courseID
                                                select new LevelWithCardsDTO()
                                                {
                                                    LevelID = a.levelID,
                                                    Type = a.type,
                                                    Name = a.name,
                                                    Number = a.number,
                                                    Color = a.color,
                                                    Icon = a.icon
                                                }).OrderBy(a=>a.Number).ToList();

            // Za svaki preuzeti nivo preuzmi sve kartice za taj nivo
            foreach (Data.LevelWithCardsDTO level in returnValue)
            {
                level.Cards = Data.Cards.getCardsFor(level.LevelID, dc);
            }

            return returnValue;
        }


       
        
    }
}
