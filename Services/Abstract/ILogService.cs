using LogLevel = furkantural.Services.Concrete.LogLevel;

namespace furkantural.Services.Abstract
{
    public interface ILogService
    {
        #region Signatures
        /// <summary>
        /// Loglama yapmak için kullanılan metot. Verilecek olan <see cref="LogLevel"/> değerine göre
        /// (Bu değerler: <seealso cref="LogLevel.Information"/>, <seealso cref="LogLevel.Success"/> ve <seealso cref="LogLevel.Error"/>)
        /// loglama yapar. Yani bir metin belgesine (ilgili günün tarihi ile kaydedilmiş {ilgili klasöre}) yeni kayıt ekler.
        /// </summary>
        /// <param name="logLevel">Hangi log seviyesinde olduğu.</param>
        /// <param name="message">Kayıt atılacak olan mesaj</param>
        /// <returns>Geriye bir şey dönmez. Yalnızca çalışır ve işlemi tamamlar.</returns>
        Task Log(LogLevel logLevel, string message);
        #endregion
    }
}