using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class EditCourseLevelsCards
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int SubcategoryID { get; set; }
        public int NumberOfCards { get; set; }
        public string Description { get; set; }
        public List<int> DeletedCards { get; set; }  
        public List<int> DeletedLevels { get; set; }

        //kada je ID kartice/nivoa 0 to znci da je to nova kartica
        public List<CardDTO> EditedCards { get; set; }   
        public List<EditLevelDTO> EditedLevels { get; set; }

        public List<CardDTO> AddedCards { get; set; }
        public List<LevelsDTO> AddedLevels { get; set; }
    }
}
