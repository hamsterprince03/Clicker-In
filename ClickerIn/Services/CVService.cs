/*using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ClickerIn.Services
{
    public interface ICVService
    {
        Task<(bool found, int x, int y)> FindOnScreen(string imagePath, double threshold);
        bool IsPythonAvailable { get; }
        Task<string> RunDiagnostics();
        string InstallInstructions { get; }
    }

    public sealed class CVService : ICVService
    {
        private readonly string _pythonPath;
        private readonly string _scriptPath;
        private readonly string _scriptDir;
        private bool? _hasPython;
        private string _cachedDiag;

        public string InstallInstructions =>
            "Для работы CV:\n1. Python 3.8+ (Add to PATH)\n2. pip install opencv-python numpy Pillow\n3. Перезапустить ClickerIn.";

        public bool IsPythonAvailable
        {
            get
            {
                if (!_hasPython.HasValue)
                {
                    try
                    {
                        using (var p = Process.Start(new ProcessStartInfo
                        {
                            FileName = _pythonPath,
                            Arguments = "--version",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }))
                        {
                            p.WaitForExit(5000);
                            _hasPython = p.ExitCode == 0;
                        }
                    }
                    catch { _hasPython = false; }
                }
                return _hasPython.Value;
            }
        }

        public CVService(string pythonPath = "python")
        {
            _pythonPath = pythonPath;
            _scriptDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonScripts");
            _scriptPath = Path.Combine(_scriptDir, "find_image.py");
        }

        public async Task<(bool found, int x, int y)> FindOnScreen(string imagePath, double threshold)
        {
            if (!File.Exists(imagePath) || !File.Exists(_scriptPath) || !IsPythonAvailable)
                return (false, 0, 0);

            return await Task.Run(() =>
            {
                try
                {
                    using (var proc = Process.Start(new ProcessStartInfo
                    {
                        FileName = _pythonPath,
                        Arguments = $"\"{_scriptPath}\" \"{imagePath}\" {threshold.ToString(CultureInfo.InvariantCulture)}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }))
                    {
                        var output = proc.StandardOutput.ReadToEnd();
                        proc.WaitForExit(15000);

                        if (string.IsNullOrWhiteSpace(output)) return (false, 0, 0);

                        var j = JObject.Parse(output);
                        if (j["error"] != null || !j["found"].Value<bool>()) return (false, 0, 0);
                        return (true, j["x"].Value<int>(), j["y"].Value<int>());
                    }
                }
                catch { return (false, 0, 0); }
            });
        }

        public async Task<string> RunDiagnostics()
        {
            if (_cachedDiag != null) return _cachedDiag;
            if (!IsPythonAvailable) return "Python не найден.";
            if (!File.Exists(_scriptPath)) return $"Скрипт не найден: {_scriptPath}";

            return await Task.Run(() =>
            {
                try
                {
                    using (var proc = Process.Start(new ProcessStartInfo
                    {
                        FileName = _pythonPath,
                        Arguments = $"\"{_scriptPath}\" --check",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }))
                    {
                        var output = proc.StandardOutput.ReadToEnd();
                        proc.WaitForExit(10000);
                        var j = JObject.Parse(output);
                        var ready = j["ready"]?.Value<bool>() ?? false;
                        _cachedDiag = ready ? "✅ CV готов" : "❌ Не все пакеты: pip install opencv-python numpy Pillow";
                        return _cachedDiag;
                    }
                }
                catch (Exception ex) { return $"Ошибка: {ex.Message}"; }
            });
        }
    }
}
*/
