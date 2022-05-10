namespace AmateurFootballLeague.ViewModels.Responses
{
    public class PlayerInTeamVM
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int TeamId { get; set; }
        public int FootballPlayerId { get; set; }
    }

    public class PlayerInTeamLV
    {
        public List<PlayerInTeamVM> PlayerInTeams =  new List<PlayerInTeamVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
