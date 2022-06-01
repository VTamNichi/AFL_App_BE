using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class News
    {
        public News()
        {
            Images = new HashSet<Image>();
        }

        public int Id { get; set; }
        public string? Content { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }
        public int? TournamentId { get; set; }
        public string? NewsImage { get; set; }

        public virtual Tournament? Tournament { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
