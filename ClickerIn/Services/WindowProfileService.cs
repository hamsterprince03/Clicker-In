using System;
using ClickerIn.Helpers;
using ClickerIn.Models;

namespace ClickerIn.Services
{
    public interface IWindowProfileService
    {
        WindowProfile CaptureProfile(string processName);
        bool RestoreProfile(WindowProfile profile);
    }

    public sealed class WindowProfileService : IWindowProfileService
    {
        private readonly IWindowManager _win;
        private readonly IDpiService _dpi;

        public WindowProfileService(IWindowManager win, IDpiService dpi)
        {
            _win = win;
            _dpi = dpi;
        }

        public WindowProfile CaptureProfile(string processName)
        {
            var hwnd = _win.FindByProcess(processName);
            if (hwnd == IntPtr.Zero) return null;

            NativeMethods.RECT rect;
            if (!_win.GetWindowRect(hwnd, out rect)) return null;

            return new WindowProfile
            {
                ProcessName = processName,
                WindowTitle = _win.GetTitle(hwnd),
                X = rect.Left,
                Y = rect.Top,
                Width = rect.Right - rect.Left,
                Height = rect.Bottom - rect.Top,
                DpiScale = _dpi.GetSystemDpiScale(),
                ScreenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth,
                ScreenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight
            };
        }

        public bool RestoreProfile(WindowProfile profile)
        {
            if (profile == null) return false;
            var hwnd = _win.FindByProcess(profile.ProcessName);
            if (hwnd == IntPtr.Zero) return false;

            if (NativeMethods.IsIconic(hwnd))
                NativeMethods.ShowWindow(hwnd, NativeMethods.SW_RESTORE);

            if (profile.RestorePosition || profile.RestoreSize)
            {
                uint flags = 0;
                if (!profile.RestorePosition) flags |= NativeMethods.SWP_NOMOVE;
                if (!profile.RestoreSize) flags |= NativeMethods.SWP_NOSIZE;
                flags |= NativeMethods.SWP_SHOWWINDOW;

                NativeMethods.SetWindowPos(hwnd, IntPtr.Zero,
                    profile.X, profile.Y, profile.Width, profile.Height, flags);
            }

            _win.BringToFront(hwnd);
            return true;
        }
    }
}