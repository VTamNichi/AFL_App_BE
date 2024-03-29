﻿using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TournamentVM
    {
        public int Id { get; set; }
        public string? TournamentName { get; set; }
        public string? Mode { get; set; }
        public string? TournamentPhone { get; set; }
        public string? TournamentGender { get; set; }
        public string? RegisterEndDate { get; set; }
        public string? TournamentStartDate { get; set; }
        public string? TournamentEndDate { get; set; }
        public string? FootballFieldAddress { get; set; }
        public string? TournamentAvatar { get; set; }
        public string? Description { get; set; }
        public string? StatusTnm { get; set; }
        public int MatchMinutes { get; set; }
        public int FootballTeamNumber { get; set; }
        public int FootballPlayerMaxNumber { get; set; }
        public int GroupNumber { get; set; }
        public string? DateCreate { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public int TournamentTypeId { get; set; }
        public int FootballFieldTypeId { get; set; }
        public int NumberTeamInTournament { get; set; }
    }
    public class TournamentListVM
    {
        public List<TournamentVM> Tournaments { get; set; } = new List<TournamentVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

    public class TournamentReportVM
    {
        public int Id { get; set; }
        public string? TournamentName { get; set; }
        public string? Mode { get; set; }
        public string? TournamentPhone { get; set; }
        public string? TournamentGender { get; set; }
        public string? RegisterEndDate { get; set; }
        public string? TournamentStartDate { get; set; }
        public string? TournamentEndDate { get; set; }
        public string? FootballFieldAddress { get; set; }
        public string? TournamentAvatar { get; set; }
        public string? Description { get; set; }
        public string? StatusTnm { get; set; }
        public int MatchMinutes { get; set; }
        public int FootballTeamNumber { get; set; }
        public int FootballPlayerMaxNumber { get; set; }
        public int GroupNumber { get; set; }
        public string? DateCreate { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public int TournamentTypeId { get; set; }
        public int FootballFieldTypeId { get; set; }
        public int CountReport { get; set; }
    }
}
