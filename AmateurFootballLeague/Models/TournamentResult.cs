using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class TournamentResult
    {
        public int Id { get; set; }
        public string? Prize { get; set; }
        public string? Description { get; set; }
        public int? TeamInTournamentId { get; set; }
        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }
        public int? FootballPlayerId { get; set; }
        public int? TotalYellowCard { get; set; }
        public int? TotalRedCard { get; set; }
        public int? TotalWinScrore { get; set; }
        public int? TotalWinMatch { get; set; }
        public int? TotalLoseMatch { get; set; }
        public int? TotalDrawMatch { get; set; }
        public int? ClothesNumber { get; set; }

        public virtual FootballPlayer? FootballPlayer { get; set; }
        public virtual Team? Team { get; set; }
        public virtual TeamInTournament? TeamInTournament { get; set; }
        public virtual Tournament? Tournament { get; set; }
    }
}
