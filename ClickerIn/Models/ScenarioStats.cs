using System;
using ClickerIn.Helpers;

namespace ClickerIn.Models
{
    public sealed class ScenarioStats : BindableBase
    {
        private int _totalRuns;
        private int _successRuns;
        private int _failedRuns;
        private DateTime? _lastRunTime;
        private TimeSpan _totalRunTime;
        private double _avgRunTimeSeconds;

        public int TotalRuns { get => _totalRuns; set => Set(ref _totalRuns, value); }
        public int SuccessRuns { get => _successRuns; set => Set(ref _successRuns, value); }
        public int FailedRuns { get => _failedRuns; set => Set(ref _failedRuns, value); }
        public DateTime? LastRunTime { get => _lastRunTime; set => Set(ref _lastRunTime, value); }
        public TimeSpan TotalRunTime { get => _totalRunTime; set => Set(ref _totalRunTime, value); }
        public double AvgRunTimeSeconds { get => _avgRunTimeSeconds; set => Set(ref _avgRunTimeSeconds, value); }

        public string Summary =>
            $"Запусков: {TotalRuns} (✅{SuccessRuns} ❌{FailedRuns})" +
            (LastRunTime.HasValue ? $" | Последний: {LastRunTime.Value:dd.MM HH:mm}" : "") +
            (AvgRunTimeSeconds > 0 ? $" | ~{AvgRunTimeSeconds:F1}сек" : "");

        public void RecordRun(bool success, TimeSpan duration)
        {
            TotalRuns++;
            if (success) SuccessRuns++; else FailedRuns++;
            LastRunTime = DateTime.Now;
            TotalRunTime += duration;
            AvgRunTimeSeconds = TotalRunTime.TotalSeconds / TotalRuns;
        }
    }
}