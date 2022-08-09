using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime? DateCreate { get; set; }
        public bool? IsSeen { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsOld { get; set; }
        public bool? ForAdmin { get; set; }
        public int? UserId { get; set; }
        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }
        public int? FootballPlayerId { get; set; }

        public virtual FootballPlayer? FootballPlayer { get; set; }
        public virtual Team? Team { get; set; }
        public virtual Tournament? Tournament { get; set; }
        public virtual User? User { get; set; }
    }
}
