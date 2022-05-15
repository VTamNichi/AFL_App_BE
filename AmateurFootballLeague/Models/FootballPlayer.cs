using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class FootballPlayer
    {
        public FootballPlayer()
        {
            PlayerInTeams = new HashSet<PlayerInTeam>();
        }

        public int Id { get; set; }
        public string? Email { get; set; }
        public string? PlayerName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlayerAvatar { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }
        public string? Phone { get; set; }

        public virtual ICollection<PlayerInTeam> PlayerInTeams { get; set; }
    }
}
