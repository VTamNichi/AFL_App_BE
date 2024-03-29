﻿using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class MatchDetail
    {
        public int Id { get; set; }
        public int? ActionMatchId { get; set; }
        public string? ActionMinute { get; set; }
        public bool? StatusPen { get; set; }
        public int? MatchId { get; set; }
        public int? PlayerInTournamentId { get; set; }
        public int? FootballPlayerId { get; set; }

        public virtual ActionMatch? ActionMatch { get; set; }
        public virtual FootballPlayer? FootballPlayer { get; set; }
        public virtual Match? Match { get; set; }
        public virtual PlayerInTournament? PlayerInTournament { get; set; }
    }
}
