namespace furkantural.Services.Abstract
{
    public interface IEmailRateLimiter
    {
        #region Signatures
        /// <summary>
        /// Kullanıcıya giden "harici" (gerçek alıcıya giden) maili göndermeden önce çağır.
        /// true -> gönderebilirsin
        /// false -> limit dolu, gönderme
        /// </summary>
        /// <returns></returns>
        Task<bool> CanSendExternalEmail();
        #endregion
    }
}