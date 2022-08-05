using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Match
    {
        public Match()
        {
            Comments = new HashSet<Comment>();
            MatchDetails = new HashSet<MatchDetail>();
            ScorePredictions = new HashSet<ScorePrediction>();
            TeamInMatches = new HashSet<TeamInMatch>();
        }

        public int Id { get; set; }
        public DateTime? MatchDate { get; set; }
        public string? Status { get; set; }
        public string? Round { get; set; }
        public string? Fight { get; set; }
        public string? GroupFight { get; set; }
        public string? TokenLivestream { get; set; }
        public int? TournamentId { get; set; }
        public string? IdScreen { get; set; }

        public virtual Tournament? Tournament { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<MatchDetail> MatchDetails { get; set; }
        public virtual ICollection<ScorePrediction> ScorePredictions { get; set; }
        public virtual ICollection<TeamInMatch> TeamInMatches { get; set; }
    }
}
