using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    public static class CommandService
    {
        // https://stackoverflow.com/questions/470256/process-waitforexit-asynchronously
        public static Task WaitForExitAsync(this Process process,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default)
                cancellationToken.Register(tcs.SetCanceled);

            return tcs.Task;
        }

        public static Process Execute(string[] commands, CommandInfo commandOptions = null)
        {
            if (commands == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            if (commands.Length >= 1)
            {
                sb.Append(commands[0]);
            }

            for (int i = 1; i < commands.Length; i++)
            {
                sb.Append($"& {commands[i]}");
            }

            var s = sb.ToString();
            return Execute(s, commandOptions);
        }

        public static Process Execute(string command, CommandInfo commandOptions = null)
        {
            var c = $"/c \"{command}\"";
            var info = new ProcessStartInfo("cmd.exe")
            {
                Arguments = c,
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            if (commandOptions != null)
            {
                info.WorkingDirectory = commandOptions.WorkingDirectory;
            }

            try
            {
                var p = Process.Start(info);
                return p;
            }
            catch (Exception e)
            {
                Logger.LogStatic($"Error: {e.Message}");
            }


            return null;
        }
    }
}