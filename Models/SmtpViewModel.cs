namespace furkantural.Models
{
    public class SmtpViewModel
    {
        #region Properties
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ListenerEmail { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = false;
        public int PerHourLimit { get; set; } = 50;
        #endregion
    }
}