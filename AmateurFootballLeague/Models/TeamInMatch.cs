using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class TeamInMatch
    {
        public TeamInMatch()
        {
            ScorePredictionTeamInMatchAs = new HashSet<ScorePrediction>();
            ScorePredictionTeamInMatchBs = new HashSet<ScorePrediction>();
        }

        public int Id { get; set; }
        public int? TeamScore { get; set; }
        public int? TeamScoreLose { get; set; }
        public int? YellowCardNumber { get; set; }
        public int? RedCardNumber { get; set; }
        public int? ScorePenalty { get; set; }
        public int? Result { get; set; }
        public string? NextTeam { get; set; }
        public string? TeamName { get; set; }
        public int? TeamInTournamentId { get; set; }
        public int? MatchId { get; set; }
        public int? WinTieBreak { get; set; }
        public int? ScoreTieBreak { get; set; }

        public virtual Match? Match { get; set; }
        public virtual TeamInTournament? TeamInTournament { get; set; }
        public virtual ICollection<ScorePrediction> ScorePredictionTeamInMatchAs { get; set; }
        public virtual ICollection<ScorePrediction> ScorePredictionTeamInMatchBs { get; set; }
    }
}
