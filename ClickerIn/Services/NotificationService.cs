using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ClickerIn.Services
{
    public enum NotificationType { Info, Warning, Error, Schedule }

    public interface INotificationService
    {
        void Show(string title, string message, int durationMs = 5000);
        void ShowWarning(string title, string message, int durationMs = 7000);
        void ShowError(string title, string message, int durationMs = 10000);
        Task<NotificationResult> ShowScheduleAlert(string title, string message,
            DateTime targetTime);
    }

    public sealed class NotificationService : INotificationService
    {
        public void Show(string title, string message, int durationMs = 5000)
        {
            ShowSimple(title, message, durationMs, NotificationType.Info);
        }

        public void ShowWarning(string title, string message, int durationMs = 7000)
        {
            ShowSimple(title, message, durationMs, NotificationType.Warning);
        }

        public void ShowError(string title, string message, int durationMs = 10000)
        {
            ShowSimple(title, message, durationMs, NotificationType.Error);
        }

        public Task<NotificationResult> ShowScheduleAlert(string title, string message,
            DateTime targetTime)
        {
            var tcs = new TaskCompletionSource<NotificationResult>();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var win = new NotificationWindow(title, message,
                        NotificationType.Schedule, targetTime, true);
                    win.Closed += (s, e) => tcs.TrySetResult(win.Result);
                    win.Show();
                }
                catch (Exception ex)
                {
                    tcs.TrySetResult(NotificationResult.Dismissed);
                }
            }));

            return tcs.Task;
        }

        private void ShowSimple(string title, string message, int durationMs, NotificationType type)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var win = new NotificationWindow(title, message, type);
                    win.Show();

                    var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(durationMs) };
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        try { win.Close(); } catch { }
                    };
                    timer.Start();
                }
                catch { }
            }));
        }
    }
}