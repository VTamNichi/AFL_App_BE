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

        public virtual TeamInTournament? TeamInTournament { get; set; }
        public virtual Tournament? Tournament { get; set; }
    }
}
