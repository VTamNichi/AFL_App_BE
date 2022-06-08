using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class PlayerInTeam
    {
        public PlayerInTeam()
        {
            PlayerInTournaments = new HashSet<PlayerInTournament>();
        }

        public int Id { get; set; }
        public string? Status { get; set; }
        public int? TeamId { get; set; }
        public int? FootballPlayerId { get; set; }

        public virtual FootballPlayer? FootballPlayer { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<PlayerInTournament> PlayerInTournaments { get; set; }
    }
}
