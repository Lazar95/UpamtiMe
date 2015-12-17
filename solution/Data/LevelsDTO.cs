using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class LevelsDTO
    {
        public int LevelID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public List<Data.CardsDTO> Cards { get; set; }
    }
}
