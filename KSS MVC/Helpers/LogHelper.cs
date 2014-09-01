using System;
using System.IO;
using System.Text;

namespace KSS.Helpers
{
    public static class LogHelper
    {
        private const string Path = @"C:\KSS_Site";
        private const string BaseFileName = "DBLog_";
        private const string BaseExtension = ".log";

        public static void WriteLog(string fileName, string source)
        {
            try
            {
                fileName = fileName.Replace("\\", ".");
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

                using (var fs = new FileStream(Path + @"\" + fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Position = fs.Length;
                    var sw = new StreamWriter(fs, Encoding.Default);
                    sw.WriteLine(source);
                    sw.WriteLine();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                WriteLog("ShellLogger.log", "Ошибка записи в файл", ex);
            }
        }

        public static void WriteLog(string fileName, string russionErrorText, Exception ex)
        {
            WriteLog(fileName, string.Format("{0}{2}:    {1}{0}", Environment.NewLine, russionErrorText, DateTime.Now));
            WriteLog(fileName,
                "Ошибка:" + Environment.NewLine + ex.Message + Environment.NewLine + "StackTrace:" +
                Environment.NewLine + ex.StackTrace);            
            WriteLogAdvanced(fileName, 0, ex.InnerException);
        }

        private static void WriteLogAdvanced(string fileName, int level, Exception ex)
        {
            if (ex != null)
            {
                WriteLog(fileName,
                    "Внутренняя ошибка #" + level + Environment.NewLine + ex.Message + Environment.NewLine +
                    "StackTrace:" + Environment.NewLine + ex.StackTrace);
                WriteLogAdvanced(fileName, level + 1, ex.InnerException);
            }
        }

        public static void WriteLog(string russianText, Exception ex)
        {
            WriteLog(BaseFileName + DateTime.Today.ToShortDateString() + BaseExtension, russianText, ex);
        }

        public static void WriteLog(string text)
        {
            WriteLog(BaseFileName + DateTime.Today.ToString("dd.mm.yyyy") + BaseExtension, text);
        }
    }
}