﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CardBasicDTO
    {
        public int CardID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public int LevelID { get; set; }
        public int Number { get; set; }

       
    }
}
