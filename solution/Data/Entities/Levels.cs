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
                dc.SubmitChanges();
            }
        }

        public static bool checkName(string name, int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.Levels
                    where a.courseID == courseID && a.name == name
                    select a).Any();
        }

        public static bool checkName(Level level)
        {
            return checkName(level.name, level.courseID);
        }

        public static void addLevel(int courseID, LevelsDTO level)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();

            if (checkName(level.Name, courseID))
                return;

            Level newLevel = new Level
            {
                name = level.Name,
                type = level.Type,
                number = level.Number
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
                if (checkName(level.Name, courseID))
                    continue;

                //opet ovo besmisleno
                Level l = GetLevel(level.LevelID, dc);
                l.name = level.Name;
                l.type = level.Type;
                l.number = level.Number;
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
                                                    Number = a.number
                                                }).ToList();

            // Za svaki preuzeti nivo preuzmi sve kartice za taj nivo
            foreach (Data.LevelsDTO level in returnValue)
            {
                level.Cards = Data.Cards.getCardsFor(level.LevelID, dc);
            }

            return returnValue;
        }

        
    }
}
