using System.ComponentModel.DataAnnotations;

namespace SamletInfo.Models

{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd")]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }
    }
}
