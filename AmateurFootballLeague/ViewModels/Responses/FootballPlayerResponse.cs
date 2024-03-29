﻿using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class FootballPlayerVM
    {
        public int Id { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerAvatar { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public string? DateCreate { get; set; }
        public string? DateUpdate { get; set; }
        public string? DateDelete { get; set; }
        public UserVM? UserVM { get; set; }
    }

    public class FootballPlayerFVM
    {
        public int Id { get; set; }
        public string? PlayerName { get; set; }

        public string? PlayerAvatar { get; set; }

        public string? Position { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public string? DateCreate { get; set; }
        public string? DateUpdate { get; set; }
        public string? DateDelete { get; set; }
        public UserVM? UserVM { get; set; }
        public virtual ICollection<PlayerInTeam>? PlayerInTeams { get; set; }
    }

    public class FootballPlayerListVM
    {
        public List<FootballPlayerVM> FootballPlayers { get; set; } = new List<FootballPlayerVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

    public class FootballPlayerListFVM
    {
        public List<FootballPlayerFVM> FootballPlayers { get; set; } = new List<FootballPlayerFVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

    public class FootballPlayerReportVM
    {
        public int Id { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerAvatar { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public string? DateCreate { get; set; }
        public string? DateUpdate { get; set; }
        public string? DateDelete { get; set; }
        public int? CountReport { get; set; }
    }
}
