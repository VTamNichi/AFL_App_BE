using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class PlayerInTournament
    {
        public int Id { get; set; }
        public int? TeamInTournamentId { get; set; }
        public int? PlayerInTeamId { get; set; }

        public virtual PlayerInTeam? PlayerInTeam { get; set; }
        public virtual TeamInTournament? TeamInTournament { get; set; }
    }
}
