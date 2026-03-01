using System.Windows.Input;
using ClickerIn.Helpers;

namespace ClickerIn.Models
{
    public sealed class AppSettings : BindableBase
    {
        private string _themeName = "Dark";
        private double _windowOpacity = 1.0;
        private bool _alwaysOnTop;
        private bool _minimizeToTray = true;
        private bool _showOverlayOnStart = true;
        private Key _startStopKey = Key.F8;
        private ModifierKeys _startStopModifiers = ModifierKeys.Control;
        private Key _recordKey = Key.F9;
        private ModifierKeys _recordModifiers = ModifierKeys.Control;
        private string _accentColor = "#FF2196F3";
        private double _fontSize = 13;
        private bool _confirmBeforeRun = true;
        private bool _logEnabled = true;
        private int _maxLogEntries = 500;
        private bool _runInBackground = true;

        public string ThemeName { get => _themeName; set => Set(ref _themeName, value); }
        public double WindowOpacity { get => _windowOpacity; set => Set(ref _windowOpacity, value); }
        public bool AlwaysOnTop { get => _alwaysOnTop; set => Set(ref _alwaysOnTop, value); }
        public bool MinimizeToTray { get => _minimizeToTray; set => Set(ref _minimizeToTray, value); }
        public bool ShowOverlayOnStart { get => _showOverlayOnStart; set => Set(ref _showOverlayOnStart, value); }
        public Key StartStopKey { get => _startStopKey; set => Set(ref _startStopKey, value); }
        public ModifierKeys StartStopModifiers { get => _startStopModifiers; set => Set(ref _startStopModifiers, value); }
        public Key RecordKey { get => _recordKey; set => Set(ref _recordKey, value); }
        public ModifierKeys RecordModifiers { get => _recordModifiers; set => Set(ref _recordModifiers, value); }
        public string AccentColor { get => _accentColor; set => Set(ref _accentColor, value); }
        public double FontSize { get => _fontSize; set => Set(ref _fontSize, value); }
        public bool ConfirmBeforeRun { get => _confirmBeforeRun; set => Set(ref _confirmBeforeRun, value); }
        public bool LogEnabled { get => _logEnabled; set => Set(ref _logEnabled, value); }
        public int MaxLogEntries { get => _maxLogEntries; set => Set(ref _maxLogEntries, value); }
        public bool RunInBackground { get => _runInBackground; set => Set(ref _runInBackground, value); }
    }
}