using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Match
    {
        public Match()
        {
            MatchDetails = new HashSet<MatchDetail>();
            ScorePredictions = new HashSet<ScorePrediction>();
            TeamInMatches = new HashSet<TeamInMatch>();
        }

        public int Id { get; set; }
        public DateTime? MatchDate { get; set; }
        public string? Status { get; set; }
        public int? TournamentId { get; set; }

        public virtual Tournament? Tournament { get; set; }
        public virtual ICollection<MatchDetail> MatchDetails { get; set; }
        public virtual ICollection<ScorePrediction> ScorePredictions { get; set; }
        public virtual ICollection<TeamInMatch> TeamInMatches { get; set; }
    }
}
