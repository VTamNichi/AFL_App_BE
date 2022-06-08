﻿using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public DateTime? DateReport { get; set; }
        public int? UserId { get; set; }
        public int? CommentId { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }

        public virtual Comment? Comment { get; set; }
        public virtual Team? Team { get; set; }
        public virtual Tournament? Tournament { get; set; }
        public virtual User? User { get; set; }
    }
}
