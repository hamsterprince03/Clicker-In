using ClickerIn.Helpers;

namespace ClickerIn.Models
{
    public enum RepeatMode { None, FixedCount, ForDuration, WhileProgramOpen }

    public sealed class RepeatSettings : BindableBase
    {
        private RepeatMode _mode = RepeatMode.None;
        private int _intervalMinutes = 30;
        private int _repeatCount = 1;
        private int _durationHours = 3;
        private string _targetProcessName;

        public RepeatMode Mode { get => _mode; set => Set(ref _mode, value); }
        public int IntervalMinutes { get => _intervalMinutes; set => Set(ref _intervalMinutes, value); }
        public int RepeatCount { get => _repeatCount; set => Set(ref _repeatCount, value); }
        public int DurationHours { get => _durationHours; set => Set(ref _durationHours, value); }
        public string TargetProcessName { get => _targetProcessName; set => Set(ref _targetProcessName, value); }

        public string DisplayInfo
        {
            get
            {
                switch (Mode)
                {
                    case RepeatMode.FixedCount:
                        return $"Каждые {IntervalMinutes} мин, {RepeatCount} раз";
                    case RepeatMode.ForDuration:
                        return $"Каждые {IntervalMinutes} мин в течение {DurationHours} ч";
                    case RepeatMode.WhileProgramOpen:
                        return $"Каждые {IntervalMinutes} мин, пока открыта {TargetProcessName}";
                    default:
                        return "Без повтора";
                }
            }
        }
    }
}