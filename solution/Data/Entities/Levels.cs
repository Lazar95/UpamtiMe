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
       

        public static List<LevelWithStatisticsDTO> getLevels(int courseID, int? userID, List<int> desiredOptLearn, List<int> desiredOptReview, int minimumLearn, int minimumReview)
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
                        List = Levels.getOptions(level.LearningStatistics.Unseen, desiredOptLearn, minimumLearn),
                        Default = 6
                    };

                    level.ReviewOptions = new Options
                    {
                        List = getOptions(level.LearningStatistics.Review, desiredOptReview, minimumReview),
                        Default = 20
                    };
                }

            }
            
            

            return returnValue;
        }



        // parametri (broj nenaucenih/neobnovljenih kartica u nivou, lista zeljenih opcija, tipa {10, 15, 20, 30, 50}, koliko najmanje kartica sme da ostane)
        public static List<int> getOptions(int totalCards, List<int> desiredOptions, int minimumLeft)
        {
            int optionNum = desiredOptions.Count; // koliko cemo opcija da ponudimo
            List<int> options = new List<int>(new int[optionNum]); // niz opcija koji ce funkcija da vrati
            int temp = totalCards; // temp je pomocna promenljiva u kojoj se pamti jos koliko kartica ima za rasporedjivanje po opcijama
            int i;
            int currentOption = 0;
            for (i = 0; i < optionNum && minimumLeft <= temp; i++)
            {
                if (temp < (desiredOptions[i] - (i > 0 ? desiredOptions[i-1] : 0))) // gledamo da li je temp manje od onoliko koliko nam treba za sledecu opciju
                    currentOption += temp;  // ako jeste, npr opcije su 30 pa 40 a nama je temp = 8, sledeca opcija ce biti 38
                else
                    currentOption = desiredOptions[i]; // a ako temp nije manje, onda je sledeca opcija 40

                if (i == 0)  // smanjume temp za onoliko koliko je nova opcija veca od prethodne
                    temp -= currentOption;
                else
                    temp -= currentOption - desiredOptions[i - 1];
                options[i] = currentOption;
            }

            if (temp < minimumLeft) // ako je ostalo jos nesto sitno (manje od minimumLeft), da se doda u poslednju opciju, ili naravno u prvu ako ima samo jedna
            {
                if (i == 0)
                    options[i] += temp;
                else
                    options[i - 1] += temp;
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
