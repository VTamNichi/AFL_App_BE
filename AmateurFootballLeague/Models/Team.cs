using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Team
    {
        public Team()
        {
            Comments = new HashSet<Comment>();
            Notifications = new HashSet<Notification>();
            PlayerInTeams = new HashSet<PlayerInTeam>();
            Reports = new HashSet<Report>();
            TeamInTournaments = new HashSet<TeamInTournament>();
        }

        public int Id { get; set; }
        public string? TeamName { get; set; }
        public string? TeamAvatar { get; set; }
        public string? TeamArea { get; set; }
        public string? TeamPhone { get; set; }
        public string? TeamGender { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }

        public virtual User IdNavigation { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<PlayerInTeam> PlayerInTeams { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<TeamInTournament> TeamInTournaments { get; set; }
    }
}
