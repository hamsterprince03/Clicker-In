using System;
using System.Threading;

namespace ClickerIn.Services
{
    public interface IAntiAfkService : IDisposable
    {
        void Start(int intervalSeconds);
        void Stop();
        bool IsRunning { get; }
    }

    public sealed class AntiAfkService : IAntiAfkService
    {
        private readonly IInputSimulatorService _sim;
        private Timer _timer;
        private static readonly Random _rng = new Random();

        public bool IsRunning { get; private set; }

        public AntiAfkService(IInputSimulatorService sim) => _sim = sim;

        public void Start(int intervalSeconds)
        {
            Stop();
            IsRunning = true;
            _timer = new Timer(_ =>
            {
                Helpers.NativeMethods.GetCursorPos(out var pt);
                int dx = _rng.Next(-2, 3);
                int dy = _rng.Next(-2, 3);
                _sim.MoveMouse(pt.X + dx, pt.Y + dy);
                Thread.Sleep(50);
                _sim.MoveMouse(pt.X, pt.Y);
            }, null, TimeSpan.FromSeconds(intervalSeconds), TimeSpan.FromSeconds(intervalSeconds + _rng.Next(10)));
        }

        public void Stop()
        {
            IsRunning = false;
            _timer?.Dispose();
            _timer = null;
        }

        public void Dispose() => Stop();
    }
}