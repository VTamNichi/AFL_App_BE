using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Tournament
    {
        public Tournament()
        {
            Images = new HashSet<Image>();
            Matches = new HashSet<Match>();
            News = new HashSet<News>();
            TeamInTournaments = new HashSet<TeamInTournament>();
            TournamentResults = new HashSet<TournamentResult>();
        }

        public int Id { get; set; }
        public string? TournamentName { get; set; }
        public string? Mode { get; set; }
        public string? TournamentPhone { get; set; }
        public string? TournamentGender { get; set; }
        public DateTime? RegisterEndDate { get; set; }
        public DateTime? TournamentStartDate { get; set; }
        public DateTime? TournamentEndDate { get; set; }
        public string? FootballFieldAddress { get; set; }
        public string? TournamentAvatar { get; set; }
        public string? Description { get; set; }
        public string? StatusTnm { get; set; }
        public int? MatchMinutes { get; set; }
        public int? FootballTeamNumber { get; set; }
        public int? FootballPlayerMaxNumber { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }
        public int? UserId { get; set; }
        public int? TournamentTypeId { get; set; }
        public int? FootballFieldTypeId { get; set; }

        public virtual FootballFieldType? FootballFieldType { get; set; }
        public virtual TournamentType? TournamentType { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<TeamInTournament> TeamInTournaments { get; set; }
        public virtual ICollection<TournamentResult> TournamentResults { get; set; }
    }
}
