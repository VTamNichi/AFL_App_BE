﻿using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }
        public int? TournamentId { get; set; }

        public virtual Tournament? Tournament { get; set; }
    }
}
