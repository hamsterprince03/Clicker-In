using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ClickerIn.Models;
using ClickerIn.Services;

namespace ClickerIn
{
    public partial class SettingsWindow : Window
    {
        public AppSettings Settings { get; private set; }
        private readonly IThemeService _theme;
        private Key _k1, _k2;
        private ModifierKeys _m1, _m2;
        private readonly AppSettings _backup;

        public SettingsWindow(AppSettings s, IThemeService theme)
        {
            InitializeComponent();
            Settings = s; _theme = theme;
            _backup = new AppSettings
            {
                ThemeName = s.ThemeName,
                AccentColor = s.AccentColor,
                FontSize = s.FontSize,
                AlwaysOnTop = s.AlwaysOnTop,
                MinimizeToTray = s.MinimizeToTray,
                ConfirmBeforeRun = s.ConfirmBeforeRun,
                LogEnabled = s.LogEnabled,
                MaxLogEntries = s.MaxLogEntries
            };

            CmbTheme.ItemsSource = theme.Themes; CmbTheme.SelectedItem = s.ThemeName;
            TxtAccent.Text = s.AccentColor; UpdatePreview();
            SldFont.Value = s.FontSize; TxtFont.Text = s.FontSize.ToString("F0");
            ChkOnTop.IsChecked = s.AlwaysOnTop; ChkTray.IsChecked = s.MinimizeToTray;
            ChkConfirm.IsChecked = s.ConfirmBeforeRun; ChkLogOn.IsChecked = s.LogEnabled;
            TxtMaxLog.Text = s.MaxLogEntries.ToString();

            _k1 = s.StartStopKey; _m1 = s.StartStopModifiers;
            _k2 = s.RecordKey; _m2 = s.RecordModifiers;
            TxtHk1.Text = Fmt(_m1, _k1); TxtHk2.Text = Fmt(_m2, _k2);
        }

        private static string Fmt(ModifierKeys m, Key k)
        {
            var p = new List<string>();
            if (m.HasFlag(ModifierKeys.Control)) p.Add("Ctrl");
            if (m.HasFlag(ModifierKeys.Alt)) p.Add("Alt");
            if (m.HasFlag(ModifierKeys.Shift)) p.Add("Shift");
            p.Add(k.ToString());
            return string.Join("+", p);
        }

        private void Theme_Changed(object s, System.Windows.Controls.SelectionChangedEventArgs e)
        { if (CmbTheme.SelectedItem is string t) _theme.Apply(t); }

        private void ApplyAccent_Click(object s, RoutedEventArgs e) { _theme.SetAccent(TxtAccent.Text); UpdatePreview(); }
        private void UpdatePreview()
        { try { AccPreview.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TxtAccent.Text)); } catch { AccPreview.Background = Brushes.Gray; } }

        private void Font_Changed(object s, RoutedPropertyChangedEventArgs<double> e)
        { if (TxtFont != null) { TxtFont.Text = SldFont.Value.ToString("F0"); _theme.SetFontSize(SldFont.Value); } }

        private bool IsModifier(Key k) => k == Key.LeftCtrl || k == Key.RightCtrl || k == Key.LeftAlt || k == Key.RightAlt || k == Key.LeftShift || k == Key.RightShift || k == Key.LWin || k == Key.RWin;

        private void Hk1_Key(object s, KeyEventArgs e)
        { e.Handled = true; var k = e.Key == Key.System ? e.SystemKey : e.Key; if (IsModifier(k)) return; _m1 = Keyboard.Modifiers; _k1 = k; TxtHk1.Text = Fmt(_m1, _k1); }

        private void Hk2_Key(object s, KeyEventArgs e)
        { e.Handled = true; var k = e.Key == Key.System ? e.SystemKey : e.Key; if (IsModifier(k)) return; _m2 = Keyboard.Modifiers; _k2 = k; TxtHk2.Text = Fmt(_m2, _k2); }

        private void Save_Click(object s, RoutedEventArgs e)
        {
            Settings.ThemeName = CmbTheme.SelectedItem as string ?? "Dark";
            Settings.AccentColor = TxtAccent.Text; Settings.FontSize = SldFont.Value;
            Settings.AlwaysOnTop = ChkOnTop.IsChecked == true; Settings.MinimizeToTray = ChkTray.IsChecked == true;
            Settings.ConfirmBeforeRun = ChkConfirm.IsChecked == true; Settings.LogEnabled = ChkLogOn.IsChecked == true;
            if (int.TryParse(TxtMaxLog.Text, out int ml)) Settings.MaxLogEntries = ml;
            Settings.StartStopKey = _k1; Settings.StartStopModifiers = _m1;
            Settings.RecordKey = _k2; Settings.RecordModifiers = _m2;
            DialogResult = true; Close();
        }

        private void Cancel_Click(object s, RoutedEventArgs e)
        {
            _theme.Apply(_backup.ThemeName); _theme.SetFontSize(_backup.FontSize);
            if (!string.IsNullOrEmpty(_backup.AccentColor)) _theme.SetAccent(_backup.AccentColor);
            DialogResult = false; Close();
        }
    }
}

