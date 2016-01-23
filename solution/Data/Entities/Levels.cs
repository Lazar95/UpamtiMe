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

        public static void addLevel(int courseID, LevelsDTO level)
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
            

            foreach (CardDTO card in level.Cards)
                card.LevelID = newLevel.levelID;

            Cards.addCards(level.Cards);
        
            
        }

        public static void addLevels(int courseID, List<LevelsDTO> levels)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (LevelsDTO level in levels)
            {
                addLevel(courseID, level);
            }
        }

        public static void editLevels(int courseID, List<EditLevelDTO> levels)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            foreach (EditLevelDTO level in levels)
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
       

        public static List<SimpleLevelDTO> getLevels(int courseID, int? userID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<SimpleLevelDTO> returnValue = (
                    from l in dc.Levels
                    where l.courseID == courseID
                    select new SimpleLevelDTO
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
                foreach (SimpleLevelDTO level in returnValue)
                {
                    level.LearningStatistics = getLevelStatistics(level.LevelID, userID.Value, dc);
                }
            }
            

            return returnValue;
        } 

        public static List<Data.LevelsDTO> getLevelsAndCardsFor(int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<Data.LevelsDTO> returnValue = (from a in dc.Levels
                                                where a.courseID == courseID
                                                select new LevelsDTO()
                                                {
                                                    LevelID = a.levelID,
                                                    Type = a.type,
                                                    Name = a.name,
                                                    Number = a.number,
                                                    Color = a.color,
                                                    Icon = a.icon
                                                }).OrderBy(a=>a.Number).ToList();

            // Za svaki preuzeti nivo preuzmi sve kartice za taj nivo
            foreach (Data.LevelsDTO level in returnValue)
            {
                level.Cards = Data.Cards.getCardsFor(level.LevelID, dc);
            }

            return returnValue;
        }

       
        
    }
}
