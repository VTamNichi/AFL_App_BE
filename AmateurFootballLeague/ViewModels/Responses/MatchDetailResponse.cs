using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class MatchDetailVM
    {
        public int Id { get; set; }
        public int MatchScore { get; set; }
        public int YellowCardNumber { get; set; }
        public int RedCardNumber { get; set; }
        public int MatchId { get; set; }
        public int PlayerInTeamId { get; set; }

    }

    public class MatchDetailFVM
    {
        public int Id { get; set; }
        public int MatchScore { get; set; }
        public int YellowCardNumber { get; set; }
        public int RedCardNumber { get; set; }
        public int MatchId { get; set; }
        public int PlayerInTeamId { get; set; }

        public virtual Match Match { get; set; }
        public virtual PlayerInTeam PlayerInTeam { get; set; }
    }

    public class MatchDetailFLV
    {
        public List<MatchDetailFVM> MatchDetails { get; set; } = new List<MatchDetailFVM> ();
    }
}
