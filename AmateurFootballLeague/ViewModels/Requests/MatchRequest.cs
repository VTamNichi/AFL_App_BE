using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum MatchStatusEnum
    {
        NotStart,
        Processing,
        Finished
    }
    public enum MatchFieldEnum
    {
        Id,
        MatchDate,
        Status
    }
}
