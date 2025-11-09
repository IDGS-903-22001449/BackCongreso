using System.ComponentModel.DataAnnotations;

namespace app_congreso.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Apellidos { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Twitter { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Ocupacion { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Avatar { get; set; } = string.Empty;

        public bool AceptaTerminos { get; set; }
    }
}
