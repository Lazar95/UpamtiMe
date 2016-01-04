using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe.Models
{
    public class UserIndexModel
    {
        public List<Data.DTOs.LeaderboardEntryDTO> WeekLeaderboard { get; set; }
        public List<Data.DTOs.LeaderboardEntryDTO> MonthLeaderboard { get; set; }
        public List<Data.DTOs.LeaderboardEntryDTO> AllTimeLeaderboard { get; set; }

        
    }

    
}