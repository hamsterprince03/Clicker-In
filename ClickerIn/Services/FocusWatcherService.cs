using System;
using System.Windows.Threading;

namespace ClickerIn.Services
{
    public interface IFocusWatcherService : IDisposable
    {
        void Watch(string processName, Action onFocusLost, Action onFocusRestored);
        void Stop();
        bool IsTargetFocused { get; }
    }

    public sealed class FocusWatcherService : IFocusWatcherService
    {
        private readonly IWindowManager _win;
        private readonly DispatcherTimer _timer;
        private string _processName;
        private Action _onLost, _onRestored;
        private bool _wasFocused;

        public bool IsTargetFocused { get; private set; }

        public FocusWatcherService(IWindowManager win)
        {
            _win = win;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Check;
        }

        public void Watch(string processName, Action onFocusLost, Action onFocusRestored)
        {
            _processName = processName;
            _onLost = onFocusLost;
            _onRestored = onFocusRestored;
            _wasFocused = false;
            _timer.Start();
        }

        public void Stop() => _timer.Stop();

        private void Check(object s, EventArgs e)
        {
            if (string.IsNullOrEmpty(_processName)) return;

            var fg = Helpers.NativeMethods.GetForegroundWindow();
            var proc = _win.GetProcessName(fg);
            bool focused = !string.IsNullOrEmpty(proc) &&
                proc.Equals(_processName, StringComparison.OrdinalIgnoreCase);

            IsTargetFocused = focused;

            if (_wasFocused && !focused)
                _onLost?.Invoke();
            else if (!_wasFocused && focused)
                _onRestored?.Invoke();

            _wasFocused = focused;
        }

        public void Dispose() => _timer.Stop();
    }
}