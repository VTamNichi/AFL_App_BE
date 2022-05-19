using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class VerifyCode
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Code { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateExpire { get; set; }
    }
}
