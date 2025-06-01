using System.ComponentModel.DataAnnotations;

namespace LinkBioAPI.Models
{
    public class LinkItem
    {
        public int Id { get; set; }

        [Required]
        public string Label { get; set; } = "";

        [Required]
        public string Url { get; set; } = "";

        public int Order { get; set; } = 0;

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
