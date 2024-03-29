﻿namespace AmateurFootballLeague.ViewModels.Responses
{
    public class NewsVM
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? NewsImage { get; set; }
        public int TournamentId { get; set; }
        public bool Status { get; set; }
        public string? DateCreate { get; set; }
    }
    public class NewsListVM
    {
        public List<NewsVM> News { get; set; } = new List<NewsVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
