using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ClickerIn.Services
{
    public interface IScenarioStorage
    {
        List<Models.Scenario> LoadAll();
        void SaveAll(List<Models.Scenario> scenarios);
        void SaveFile(Models.Scenario scenario, string path);
        Models.Scenario LoadFile(string path);
        string DefaultDir { get; }

        List<Models.ScheduleEntry> LoadSchedules();
        void SaveSchedules(List<Models.ScheduleEntry> entries);

        List<Models.ScenarioChain> LoadChains();
        void SaveChains(List<Models.ScenarioChain> chains);
    }

    public sealed class ScenarioStorageService : IScenarioStorage
    {
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None
        };

        public string DefaultDir { get; }
        private readonly string _scenariosFile;
        private readonly string _schedulesFile;
        private readonly string _chainsFile;

        public ScenarioStorageService()
        {
            DefaultDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ClickerIn");
            Directory.CreateDirectory(DefaultDir);

            _scenariosFile = Path.Combine(DefaultDir, "scenarios.json");
            _schedulesFile = Path.Combine(DefaultDir, "schedules.json");
            _chainsFile = Path.Combine(DefaultDir, "chains.json");
        }

        public List<Models.Scenario> LoadAll()
        {
            try
            {
                if (File.Exists(_scenariosFile))
                {
                    string json = File.ReadAllText(_scenariosFile);
                    var list = JsonConvert.DeserializeObject<List<Models.Scenario>>(json, _jsonSettings);
                    if (list != null && list.Count > 0) return list;
                }
            }
            catch { }
            return new List<Models.Scenario>();
        }

        public void SaveAll(List<Models.Scenario> scenarios)
        {
            try
            {
                string json = JsonConvert.SerializeObject(scenarios, _jsonSettings);
                File.WriteAllText(_scenariosFile, json);
            }
            catch { }
        }

        public void SaveFile(Models.Scenario scenario, string path)
        {
            string json = JsonConvert.SerializeObject(scenario, _jsonSettings);
            File.WriteAllText(path, json);
        }

        public Models.Scenario LoadFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Models.Scenario>(json, _jsonSettings);
        }

        public List<Models.ScheduleEntry> LoadSchedules()
        {
            try
            {
                if (File.Exists(_schedulesFile))
                {
                    string json = File.ReadAllText(_schedulesFile);
                    var list = JsonConvert.DeserializeObject<List<Models.ScheduleEntry>>(json, _jsonSettings);
                    if (list != null) return list;
                }
            }
            catch { }
            return new List<Models.ScheduleEntry>();
        }

        public void SaveSchedules(List<Models.ScheduleEntry> entries)
        {
            try
            {
                string json = JsonConvert.SerializeObject(entries, _jsonSettings);
                File.WriteAllText(_schedulesFile, json);
            }
            catch { }
        }

        public List<Models.ScenarioChain> LoadChains()
        {
            try
            {
                if (File.Exists(_chainsFile))
                {
                    string json = File.ReadAllText(_chainsFile);
                    var list = JsonConvert.DeserializeObject<List<Models.ScenarioChain>>(json, _jsonSettings);
                    if (list != null) return list;
                }
            }
            catch { }
            return new List<Models.ScenarioChain>();
        }

        public void SaveChains(List<Models.ScenarioChain> chains)
        {
            try
            {
                string json = JsonConvert.SerializeObject(chains, _jsonSettings);
                File.WriteAllText(_chainsFile, json);
            }
            catch { }
        }
    }
}