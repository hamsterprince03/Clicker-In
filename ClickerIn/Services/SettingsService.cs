using System;
using System.IO;
using ClickerIn.Models;
using Newtonsoft.Json;

namespace ClickerIn.Services
{
    public interface ISettingsService
    {
        AppSettings Load();
        void Save(AppSettings settings);
    }

    public sealed class SettingsService : ISettingsService
    {
        private readonly string _path;

        public SettingsService()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClickerIn");
            Directory.CreateDirectory(dir);
            _path = Path.Combine(dir, "settings.json");
        }

        public AppSettings Load()
        {
            try
            {
                return File.Exists(_path)
                    ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(_path)) ?? new AppSettings()
                    : new AppSettings();
            }
            catch { return new AppSettings(); }
        }

        public void Save(AppSettings s)
        {
            try
            {
                File.WriteAllText(_path, JsonConvert.SerializeObject(s, Newtonsoft.Json.Formatting.Indented));
            }
            catch { }
        }
    }
}

