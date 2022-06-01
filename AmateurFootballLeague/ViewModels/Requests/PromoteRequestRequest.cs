namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum PromoteRequestFieldEnum
    {
        Id,
        IdentityCard,
        PhoneBusiness,
        NameBusiness,
        Tinbusiness,
        Status
    }

    public class PromoteRequestCM
    {
        public string? RequestContent { get; set; }
        public string? IdentityCard { get; set; }
        public DateTime? DateIssuance { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? Tinbusiness { get; set; }
        public int UserId { get; set; }
    }

    public class PromoteRequestUM
    {
        public int Id { get; set; }
        public string? RequestContent { get; set; }
        public string? IdentityCard { get; set; }
        public DateTime? DateIssuance { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? Tinbusiness { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
    }
}
