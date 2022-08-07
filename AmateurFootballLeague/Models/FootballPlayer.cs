using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class FootballPlayer
    {
        public FootballPlayer()
        {
            MatchDetails = new HashSet<MatchDetail>();
            Notifications = new HashSet<Notification>();
            PlayerInTeams = new HashSet<PlayerInTeam>();
            Reports = new HashSet<Report>();
            TournamentResults = new HashSet<TournamentResult>();
        }

        public int Id { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerAvatar { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }

        public virtual User IdNavigation { get; set; } = null!;
        public virtual ICollection<MatchDetail> MatchDetails { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<PlayerInTeam> PlayerInTeams { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<TournamentResult> TournamentResults { get; set; }
    }
}
