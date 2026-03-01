using System;
using System.Windows;
using System.Windows.Threading;
using ClickerIn.Services;

namespace ClickerIn
{
    public partial class NotificationWindow : Window
    {
        private readonly DispatcherTimer _countdownTimer;
        private DateTime _targetTime;
        private bool _hasCountdown;

        public NotificationResult Result { get; private set; } = NotificationResult.Dismissed;

        public NotificationWindow(string title, string message,
            NotificationType type = NotificationType.Info)
        {
            InitializeComponent();
            Setup(title, message, type);
            PositionWindow();
        }

        public NotificationWindow(string title, string message,
            NotificationType type, DateTime targetTime, bool showActions)
        {
            InitializeComponent();
            Setup(title, message, type);

            if (showActions)
            {
                PanelActions.Visibility = Visibility.Visible;
                TxtCountdown.Visibility = Visibility.Visible;
                _hasCountdown = true;
                _targetTime = targetTime;

                _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _countdownTimer.Tick += UpdateCountdown;
                _countdownTimer.Start();
                UpdateCountdownText();
            }

            PositionWindow();
        }

        private void Setup(string title, string message, NotificationType type)
        {
            TxtTitle.Text = title;
            TxtMessage.Text = message;

            switch (type)
            {
                case NotificationType.Warning:
                    TxtIcon.Text = "⚠";
                    MainBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(255, 180, 0));
                    break;
                case NotificationType.Error:
                    TxtIcon.Text = "❌";
                    MainBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(255, 80, 80));
                    break;
                case NotificationType.Schedule:
                    TxtIcon.Text = "⏰";
                    MainBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(142, 161, 230));
                    break;
                default:
                    TxtIcon.Text = "ℹ";
                    break;
            }
        }

        private void UpdateCountdown(object s, EventArgs e)
        {
            UpdateCountdownText();
        }

        private void UpdateCountdownText()
        {
            if (!_hasCountdown) return;
            var remaining = _targetTime - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
            {
                TxtCountdown.Text = "⏰ Время запуска наступило!";
                _countdownTimer?.Stop();
            }
            else if (remaining.TotalMinutes >= 1)
            {
                TxtCountdown.Text = "⏳ До запуска: " + (int)remaining.TotalMinutes + " мин " +
                    remaining.Seconds + " сек";
            }
            else
            {
                TxtCountdown.Text = "⏳ До запуска: " + remaining.Seconds + " сек";
            }
        }

        private void PositionWindow()
        {
            var area = SystemParameters.WorkArea;
            Left = area.Right - 440;
            Top = area.Bottom - 180;
        }

        private void Accept_Click(object s, RoutedEventArgs e)
        {
            Result = NotificationResult.Accepted;
            _countdownTimer?.Stop();
            Close();
        }

        private void Postpone_Click(object s, RoutedEventArgs e)
        {
            Result = NotificationResult.Postponed;
            _countdownTimer?.Stop();
            Close();
        }

        private void Close_Click(object s, RoutedEventArgs e)
        {
            Result = NotificationResult.Dismissed;
            _countdownTimer?.Stop();
            Close();
        }
    }

    public enum NotificationResult
    {
        Dismissed,
        Accepted,
        Postponed
    }
}