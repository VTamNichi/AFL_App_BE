using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class ActionMatch
    {
        public ActionMatch()
        {
            MatchDetails = new HashSet<MatchDetail>();
        }

        public int Id { get; set; }
        public string? ActionMatchName { get; set; }

        public virtual ICollection<MatchDetail> MatchDetails { get; set; }
    }
}
