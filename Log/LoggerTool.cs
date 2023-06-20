using System;
using System.IO;

namespace AcDoNetTool.Log
{
    public class LoggerTool
    {
        public string LogPath { get; }

        public static readonly LoggerTool Instance = new LoggerTool();

        private LoggerTool()
        {
            var dir = Environment.CurrentDirectory;
            if (!Directory.Exists(dir + "\\Logs"))
            {
                Directory.CreateDirectory(dir + "\\Logs");
            }
            string logName = @"Logs\Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            LogPath = Path.Combine(dir, logName);
            if (!File.Exists(LogPath))
            {
                FileStream fileStream = File.Create(LogPath);
                fileStream.Close();
            }
        }

        public void Log(string txt)
        {
            if (txt != null)
            {
                WriteLine("[LOG]" + txt, true);
            }
        }

        public void Warnning(string warnning)
        {
            if (warnning != null)
            {
                WriteLine("[WARNNING]" + warnning, true);
            }
        }

        public void Error(string error)
        {
            if (error != null)
            {
                WriteLine("[ERROR]" + error, true);
            }
        }

        private void WriteLine(string line, bool writeToFile)
        {
            var temp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + (line ?? string.Empty) + Environment.NewLine;
            System.Diagnostics.Trace.Write(temp);

            if (writeToFile)
            {
                WriteToFile(temp);
            }
        }

        private void WriteToFile(string line)
        {
            // 如果日志文件超过3M，删除之
            if (File.Exists(LogPath))
            {
                var info = new FileInfo(LogPath);
                var m = info.Length / 1024 / 1024;
                if (m > 3)
                {
                    File.Delete(LogPath);
                }
            }
            File.AppendAllText(LogPath, line);
        }
    }
}