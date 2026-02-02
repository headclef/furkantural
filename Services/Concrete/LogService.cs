using furkantural.Services.Abstract;
using System.Collections.Concurrent;

namespace furkantural.Services.Concrete
{
    public class LogService : ILogService
    {
        #region Properties
        private readonly string informationLogDirectory = "InformationLogs";
        private readonly string successLogDirectory = "SuccessLogs";
        private readonly string errorLogDirectory = "ErrorLogs";
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks = new();
        #endregion

        #region Constructors
        public LogService()
        {
            EnsureFolderExists(informationLogDirectory).Wait();
            EnsureFolderExists(successLogDirectory).Wait();
            EnsureFolderExists(errorLogDirectory).Wait();
        }
        #endregion

        #region Methods
        public async Task Log(LogLevel logLevel, string message)
        {
            // Loglanacak dosyanın adı.
            string fileName = $"{DateTime.UtcNow:yyyy-MM-dd}_{logLevel}.txt";

            // Bilgilendirme logu mu?
            bool isInformation = logLevel == LogLevel.Information;

            // Başarılı işlem logu mu?
            bool isSuccess = logLevel == LogLevel.Success;

            // Başarısız işlem logu mu?
            bool isError = logLevel == LogLevel.Error;

            // Klasör + Dosyanın yol birleşimi. Örneğin: "InformationLogs/2025-01-01_Information.txt".
            string path = Path.Combine(
                isInformation ?
                informationLogDirectory :
                    isSuccess ? successLogDirectory :
                    errorLogDirectory,
                fileName
            );

            // İlgili dosya ile işlem yapmak için toplam işlem limiti.
            var gate = _fileLocks.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync();
            try
            {
                // Dosya yoksa kendisi oluşturur; paylaşım çakışmalarını azaltır
                await File.AppendAllTextAsync(path, $"{DateTime.UtcNow:HH-mm} | {logLevel} | {message}{Environment.NewLine}");
            }
            finally
            {
                gate.Release();
            }
        }

        private async Task EnsureFolderExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }
        #endregion
    }

    #region Enums
    public enum LogLevel
    {
        Information = 0,
        Success = 1,
        Error = 2
    }
    #endregion
}