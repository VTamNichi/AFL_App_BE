using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class ImageCM
    {
        [Required(AllowEmptyStrings = false)]
        public IFormFile File { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int NewsID { get; set; }
    }
    public class ImageUM
    {
        public IFormFile? File { get; set; }
        public int? NewsID { get; set; }
    }
}
