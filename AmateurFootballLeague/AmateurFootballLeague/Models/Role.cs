namespace AmateurFootballLeague.Models
{
    public class Role
    {
        public int ID { get; set; }
        public string RoleName { get; set; }
        public List<User> Users { get; set; }
    }
}
