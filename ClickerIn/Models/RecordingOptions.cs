using ClickerIn.Helpers;

namespace ClickerIn.Models
{
    public sealed class RecordingOptions : BindableBase
    {
        private bool _recordMouseMovement;
        private int _movementSampleIntervalMs = 50;
        private bool _simplifyPath = true;
        private double _simplifyTolerance = 5.0;
        private bool _recordKeyboard;

        public bool RecordMouseMovement { get => _recordMouseMovement; set => Set(ref _recordMouseMovement, value); }
        public int MovementSampleIntervalMs { get => _movementSampleIntervalMs; set => Set(ref _movementSampleIntervalMs, value); }
        public bool SimplifyPath { get => _simplifyPath; set => Set(ref _simplifyPath, value); }
        public double SimplifyTolerance { get => _simplifyTolerance; set => Set(ref _simplifyTolerance, value); }
        public bool RecordKeyboard { get => _recordKeyboard; set => Set(ref _recordKeyboard, value); }
    }
}