using furkantural.Services.Concrete;

namespace furkantural.Services.Abstract
{
    public interface IEmailService
    {
        #region Signatures
        /// <summary>
        /// Kullanıcıya bir bilgilendirme e-postası gönderirken bir yandan da listener 'a kullanıcının e-postasını iletir.
        /// </summary>
        /// <param name="toEmail">Kime gidecek?</param>
        /// <param name="emailType">Hangi tipte e-posta? <seealso cref="EmailType.Contact"/> mı? Yoksa <seealso cref="EmailType.Listener"/> mı?</param>
        /// <param name="placeholders">Şablon üzerindeki alanlar</param>
        /// <returns></returns>
        Task SendTransactionalEmailAsync(string toEmail, EmailType emailType, Dictionary<string, string> placeholders);
        #endregion
    }
}