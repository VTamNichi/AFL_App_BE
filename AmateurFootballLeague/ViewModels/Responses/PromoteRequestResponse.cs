namespace AmateurFootballLeague.ViewModels.Responses
{
    public class PromoteRequestVM
    {
        public int Id { get; set; }
        public string RequestContent { get; set; }
        public string IdentityCard { get; set; }
        public string DateIssuance { get; set; }
        public string PhoneBusiness { get; set; }
        public string NameBusiness { get; set; }
        public string Tinbusiness { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime DateCreate { get; set; }
        public int UserId { get; set; }
    }
    public class PromoteRequestListVM
    {
        public List<PromoteRequestVM> PromoteRequests { get; set; } = new List<PromoteRequestVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
