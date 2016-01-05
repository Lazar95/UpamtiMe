using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Levels
    {
        //// Lazino staro
        ////////////////////////
        //public static Data.Level GetLevel(int levelID)
        //{
        //    DataClasses1DataContext dc = new DataClasses1DataContext();
        //    return (from a
        //            in dc.Levels
        //            where a.levelID == levelID
        //            select a).First();
        //}

        //public static int countLevels(int courseID)
        //{
        //    DataClasses1DataContext dc = new DataClasses1DataContext();
        //    return (from a
        //            in dc.CoursesLevels
        //            where a.courseID == courseID
        //            select a).Count();
        //}

        public static List<Data.LevelsDTO> getLevelsAndCardsFor(int courseID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            List<Data.LevelsDTO> returnValue = (from a in dc.CoursesLevels
                                                join b in dc.Levels
                                                on a.LevelID equals b.LevelID
                                                where a.courseID == courseID
                                                select new LevelsDTO()
                                                {
                                                    LevelID = b.LevelID,
                                                    Type = b.type,
                                                    Name = b.name,
                                                }).ToList();

            // Za svaki preuzeti nivo preuzmi sve kartice za taj nivo
            foreach (Data.LevelsDTO level in returnValue)
            {
                level.Cards = Data.Cards.getCardsFor(level.LevelID);
            }

            return returnValue;
        }

        //public static Level addLevel(int courseID, string name, int type)
        //{
        //    DataClasses1DataContext dc = new DataClasses1DataContext();

        //    Level level = new Level
        //    {
        //        type = type,
        //        name = name,
        //    };

        //    dc.Levels.InsertOnSubmit(level);
        //    dc.SubmitChanges();

        //    CoursesLevel courseLevel = new CoursesLevel
        //    {
        //        courseID = courseID,
        //        levelID = level.levelID,
        //        number = countLevels(courseID) + 1,
        //    };

        //    return level;
        //}
    }
}
