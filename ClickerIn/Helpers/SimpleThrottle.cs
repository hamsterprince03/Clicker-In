using System;
using System.Windows.Threading;

namespace ClickerIn.Helpers
{
    public sealed class SimpleThrottle<T> : IDisposable
    {
        private readonly Action<T> _handler;
        private readonly DispatcherTimer _timer;
        private T _latest;
        private bool _hasValue;

        public SimpleThrottle(TimeSpan interval, Action<T> handler)
        {
            _handler = handler;
            _timer = new DispatcherTimer { Interval = interval };
            _timer.Tick += OnTick;
            _timer.Start();
        }

        public void Push(T value)
        {
            _latest = value;
            _hasValue = true;
        }

        private void OnTick(object s, EventArgs e)
        {
            if (!_hasValue) return;
            _hasValue = false;
            _handler(_latest);
        }

        public void Dispose() => _timer.Stop();
    }
}