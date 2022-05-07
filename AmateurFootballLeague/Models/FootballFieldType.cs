using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class FootballFieldType
    {
        public FootballFieldType()
        {
            Tournaments = new HashSet<Tournament>();
        }

        public int Id { get; set; }
        public string? FootballFieldTypeName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Tournament> Tournaments { get; set; }
    }
}
