using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class MatchDetail
    {
        public int Id { get; set; }
        public int? MatchScore { get; set; }
        public int? YellowCardNumber { get; set; }
        public int? RedCardNumber { get; set; }
        public int? MatchId { get; set; }
        public int? PlayerInTeamId { get; set; }

        public virtual Match? Match { get; set; }
        public virtual PlayerInTeam? PlayerInTeam { get; set; }
    }
}
