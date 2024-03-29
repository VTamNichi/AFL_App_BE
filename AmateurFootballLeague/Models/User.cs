﻿using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Notifications = new HashSet<Notification>();
            PromoteRequests = new HashSet<PromoteRequest>();
            Reports = new HashSet<Report>();
            ScorePredictions = new HashSet<ScorePrediction>();
            Tournaments = new HashSet<Tournament>();
        }

        public int Id { get; set; }
        public string? Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Username { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public string? StatusBan { get; set; }
        public DateTime? DateBan { get; set; }
        public DateTime? DateUnban { get; set; }
        public int? FlagReportFootballPlayer { get; set; }
        public int? FlagReportTeam { get; set; }
        public int? FlagReportTournament { get; set; }
        public string? IdentityCard { get; set; }
        public DateTime? DateIssuance { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? Tinbusiness { get; set; }
        public bool? Status { get; set; }
        public int? CountBlock { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual FootballPlayer FootballPlayer { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<PromoteRequest> PromoteRequests { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<ScorePrediction> ScorePredictions { get; set; }
        public virtual ICollection<Tournament> Tournaments { get; set; }
    }
}
