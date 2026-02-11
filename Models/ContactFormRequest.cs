using System.ComponentModel.DataAnnotations;

namespace furkantural.Models
{
    public class ContactFormRequest
    {
        #region Properties
        [Required(ErrorMessage = "E-posta adresinizi girmek zorundasınız.")]
        [Display(Name = "E-posta")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad Soyad girmek zorundasınız.")]
        [Display(Name = "Ad Soyad")]
        public string NameSurname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj / İhtiyaç girmek zorundasınız.")]
        [Display(Name = "Mesaj / İhtiyaç")]
        public string MessageNeed { get; set; } = string.Empty;

        // Cloudflare Turnstile için token
        [Required(ErrorMessage = "Güvenlik doğrulaması gerekli")]
        public string TurnstileResponse { get; set; } = string.Empty;
        #endregion
    }
}