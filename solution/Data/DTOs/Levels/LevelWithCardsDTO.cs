using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DTOs;

namespace Data
{
    public class LevelWithCardsDTO : LevelBasicDTO
    {
        public List<Data.CardBasicDTO> Cards { get; set; }
    }
}
