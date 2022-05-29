﻿using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class TeamInTournament
    {
        public TeamInTournament()
        {
            PlayerInTournaments = new HashSet<PlayerInTournament>();
            TournamentResults = new HashSet<TournamentResult>();
        }

        public int Id { get; set; }
        public int? Point { get; set; }
        public int? DifferentPoint { get; set; }
        public string? Status { get; set; }
        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }

        public virtual Team? Team { get; set; }
        public virtual Tournament? Tournament { get; set; }
        public virtual ICollection<PlayerInTournament> PlayerInTournaments { get; set; }
        public virtual ICollection<TournamentResult> TournamentResults { get; set; }

        public static implicit operator TeamInTournament(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
