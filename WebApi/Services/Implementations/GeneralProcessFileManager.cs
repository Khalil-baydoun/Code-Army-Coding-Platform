using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace WebApi.Services.Implementations
{
    public class GeneralProcessFileManager
    {
        public class ProcessInput
        {
            public int TimeLimit { get; set; }

            public int MemoryLimit { get; set; }

            public string Input { get; set; }

            public List<string> ExpectedOutput { get; set; }
        }

        public class ProcessOutput
        {
            public List<string> Output { get; set; }

            public int ExitCode { get; set; }

            public int TimeTaken { get; set; }

            public int MemoryTaken { get; set; }
        }

        public static void CreateFile(string filePath, string content, string directoryName)
        {
            using (FileStream fs = System.IO.File.Create(filePath))
            {
                Byte[] byteContent = new UTF8Encoding(true).GetBytes(content);
                fs.Write(byteContent, 0, byteContent.Length);
            }
        }

        public static ProcessStartInfo CreateProcessStartInfo(string processName, string arguments)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.FileName = processName;
            processStartInfo.Arguments += " " + arguments;
            return processStartInfo;
        }

        public static int GetProcessRunningTime(DateTime endTime, DateTime startTime)
        {
            TimeSpan span = endTime - startTime;
            return (int)span.TotalMilliseconds;
        }

        public static ProcessOutput RunProcess(string processName, string arguments, double timeLimit, string inputText = "")
        {
            Process process = new Process()
            {
                EnableRaisingEvents = false
            };
            process.StartInfo = CreateProcessStartInfo(processName, arguments);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            process.Start();

            var output = new List<string>();
            process.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                output.Add(e.Data);
            });

            process.BeginOutputReadLine();

            long memoryTaken = -1;

            Timer memoryTimer = new Timer(
             delegate
             {
                 try
                 {
                     if (!process.HasExited)
                     {
                         memoryTaken = process.HasExited ? 0 : process.PeakWorkingSet64; //This behaves weird when receiving too manny requests

                     }
                 }
                 catch
                 {
                 }
             },
             null, 5, 1
            );

            if (!string.IsNullOrWhiteSpace(inputText))
            {
                StreamWriter myStreamWriter = process.StandardInput;
                myStreamWriter.Write(inputText);
            }

            process.WaitForExit((int)timeLimit);
            process.Kill();
            sw.Stop();
            output = output.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x = x.Trim()).ToList();
            return new ProcessOutput
            {
                Output = output,
                TimeTaken = (int)sw.ElapsedMilliseconds,
                ExitCode = process.ExitCode,
                MemoryTaken = (int)memoryTaken / (1024)
            };
        }
    }
}