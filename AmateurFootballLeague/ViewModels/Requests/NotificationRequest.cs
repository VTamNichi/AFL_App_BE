﻿namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum NotificationFieldEnum
    {
        Id,
        Content,
        DateCreate,
        UserId,
        TournamentId,
        TeamId
    }

    public class NotificationCM
    {
        public string? Content { get; set; }
        public int UserId { get; set; }
        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }
    }
    public class NotificationUM
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public bool? IsSeen { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }
    }
}
