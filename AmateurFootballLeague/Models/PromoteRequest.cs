using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class PromoteRequest
    {
        public int Id { get; set; }
        public string? RequestContent { get; set; }
        public string? IdentityCard { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? Tinbusiness { get; set; }
        public string? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public int? UserId { get; set; }
        public string? Reason { get; set; }

        public virtual User? User { get; set; }
    }
}
