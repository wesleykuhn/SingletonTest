using SingletonTest.Enums;
using System.Diagnostics;
using System.Text;

namespace SingletonTest.Logging
{
    internal sealed class DataLogging
    {
        #region [ Consts, Props & Fields ]
        
        private const string LogFileFolder = "Logs";

        private string LogCurrentDayFolder => DateTime.Now.ToString("dd_MM_yyyy");
        private string AppBaseDirectory => AppContext.BaseDirectory; //Already ends with '\'
        private string LogFolderFullDirectory => $"{AppBaseDirectory}{LogFileFolder}";
        private string LogCurrentDayFolderFullDirectory => $"{AppBaseDirectory}{LogFileFolder}\\{LogCurrentDayFolder}";

        #endregion

        //Optei pelo Lazy<T> porque ele trabalha usando thread safe, entao nao terei que gerenciar as threads.
        #region [ Lazy ]

        private static readonly Lazy<DataLogging> _dataLoggingLazy = new(() => new DataLogging());

        public static DataLogging CurrentInstance => _dataLoggingLazy.Value;

        #endregion

        public DataLogging()
        {
            CheckDirectoryExistsAndCreate(LogFolderFullDirectory);
        }

        public void OpenLogsFolder()
        {
            ProcessStartInfo startInfo = new()
            {
                Arguments = LogFolderFullDirectory,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

        public async Task<bool> LogException(Exception ex,
            string callerClass,
            string callerMethod,
            string? whileCalling = null,
            Dictionary<string, string>? usedParameters = null)
        {
            try
            {
                StringBuilder fileContent = new();
                fileContent.AppendLine($"DateTime: {DateTime.Now:dd/MM/yyyy HH:mm:ss}.");
                fileContent.AppendLine($"\nException: {ex.GetType().Name ?? "N/A"}.");
                fileContent.AppendLine($"\nException's message: {ex.Message ?? "N/A"}.");
                fileContent.AppendLine($"\nException's source: {ex.Source ?? "N/A"}.");
                fileContent.AppendLine($"\nException's stack trace: {ex.StackTrace ?? "N/A"}.");
                fileContent.AppendLine($"\nException's help link: {ex.HelpLink ?? "N/A"}.");
                fileContent.AppendLine($"\nInner exception: {ex.InnerException?.Message ?? "N/A"}.");
                fileContent.AppendLine($"\nCaller class: {callerClass}.");
                fileContent.AppendLine($"\nCaller method: {callerMethod}.");
                fileContent.AppendLine($"\nWhile calling: {whileCalling ?? "N/A"}.\n");

                if (usedParameters is not null && usedParameters.Any())
                {
                    foreach (var param in usedParameters)
                        fileContent.AppendLine($"Used parameter: {param.Key} - Value: {param.Value}.");
                }

                //../Logs/13_07_2022
                CheckDirectoryExistsAndCreate(LogCurrentDayFolderFullDirectory);

                //../Logs/13_07_2022/Class
                var classNameFullDir = ClassNameToClassFolderFullDirectory(callerClass);
                CheckDirectoryExistsAndCreate(classNameFullDir);

                //../Logs/13_07_2022/Class/120610_Method.txt
                var fileNameFullDir = GenerateLogFileNameFullDirectory(classNameFullDir, callerMethod, LogType.Exception);

                using (FileStream fs = new(fileNameFullDir, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter tw = new(fs);
                    await tw.WriteAsync(fileContent.ToString());
                    tw.Flush();
                }

                return true;
            }
            catch (Exception methodEx)
            {
                Debug.WriteLine(methodEx.Message);

                return false;
            }
        }

        public async Task<bool> LogCustomMessage(string message,
            string callerClass,
            string callerMethod,
            string? whileCalling = null,
            Dictionary<string, string>? usedParameters = null)
        {
            try
            {
                StringBuilder fileContent = new();
                fileContent.AppendLine($"DateTime: {DateTime.Now:dd/MM/yyyy HH:mm:ss}.");
                fileContent.AppendLine($"\nMessage: {message}.");
                fileContent.AppendLine($"\nCaller class: {callerClass}.");
                fileContent.AppendLine($"\nCaller method: {callerMethod}.");
                fileContent.AppendLine($"\nWhile calling: {whileCalling ?? "N/A"}.\n");

                if (usedParameters is not null && usedParameters.Any())
                {
                    foreach (var param in usedParameters)
                        fileContent.AppendLine($"Used parameter: {param.Key} - Value: {param.Value}.");
                }

                //../Logs/13_07_2022
                CheckDirectoryExistsAndCreate(LogCurrentDayFolderFullDirectory);

                //../Logs/13_07_2022/Class
                var classNameFullDir = ClassNameToClassFolderFullDirectory(callerClass);
                CheckDirectoryExistsAndCreate(classNameFullDir);

                //../Logs/13_07_2022/Class/120610_Method.txt
                var fileNameFullDir = GenerateLogFileNameFullDirectory(classNameFullDir, callerMethod, LogType.Warning);

                using (FileStream fs = new(fileNameFullDir, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter tw = new(fs);
                    await tw.WriteAsync(fileContent.ToString());
                    tw.Flush();
                }

                return true;
            }
            catch (Exception methodEx)
            {
                Debug.WriteLine(methodEx.Message);

                return false;
            }
        }

        private void CheckDirectoryExistsAndCreate(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private string ClassNameToClassFolderFullDirectory(string className) =>
            $"{LogCurrentDayFolderFullDirectory}\\{className}";

        private string GenerateLogFileNameFullDirectory(string classNameFullDir, string methodName, LogType type) =>
            $"{classNameFullDir}\\{DateTime.Now:HHmmss}{DateTime.Now.Millisecond}_{type}_{methodName}.txt";
    }
}
