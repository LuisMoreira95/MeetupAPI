using System.ComponentModel.DataAnnotations;

namespace MMeetupAPI.Models
{
    public class MeetupDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]
        [MinLength(5)]
        public string Organizer { get; set; }
        public DateTime Date { get; set; }
        public bool IsPrivate { get; set; }
    }
}
