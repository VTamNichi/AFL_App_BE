﻿using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class PlayerInTournament
    {
        public PlayerInTournament()
        {
            MatchDetails = new HashSet<MatchDetail>();
        }

        public int Id { get; set; }
        public string? Status { get; set; }
        public int? ClothesNumber { get; set; }
        public int? TeamInTournamentId { get; set; }
        public int? PlayerInTeamId { get; set; }

        public virtual PlayerInTeam? PlayerInTeam { get; set; }
        public virtual TeamInTournament? TeamInTournament { get; set; }
        public virtual ICollection<MatchDetail> MatchDetails { get; set; }
    }
}
