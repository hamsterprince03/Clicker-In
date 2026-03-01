using System;
using System.Runtime.InteropServices;
using ClickerIn.Helpers;

namespace ClickerIn.Services
{
    public interface IDpiService
    {
        /// <summary>Получить текущий DPI масштаб системы</summary>
        double GetSystemDpiScale();

        /// <summary>Получить DPI масштаб для конкретного монитора/окна</summary>
        double GetDpiScaleForWindow(IntPtr hWnd);

        /// <summary>Скорректировать координаты из записанного DPI в текущий</summary>
        (int x, int y) AdjustCoordinates(int x, int y, double recordedDpi, double currentDpi);

        /// <summary>Скорректировать размер окна</summary>
        (int width, int height) AdjustSize(int width, int height, double recordedDpi, double currentDpi);
    }

    public sealed class DpiService : IDpiService
    {
        // DPI awareness
        private const int MDT_EFFECTIVE_DPI = 0;
        private const int MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(NativeMethods.POINT pt, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern int GetDpiForWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int GetDpiForSystem();

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private bool _initialized;

        public DpiService()
        {
            EnsureDpiAware();
        }

        private void EnsureDpiAware()
        {
            if (_initialized) return;
            try
            {
                // Для Windows 10 1607+
                SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            }
            catch
            {
                try
                {
                    SetProcessDPIAware();
                }
                catch { }
            }
            _initialized = true;
        }

        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr value);

        public double GetSystemDpiScale()
        {
            try
            {
                int dpi = GetDpiForSystem();
                return dpi / 96.0;
            }
            catch
            {
                // Fallback через Graphics
                try
                {
                    using (var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                    {
                        return g.DpiX / 96.0;
                    }
                }
                catch { return 1.0; }
            }
        }

        public double GetDpiScaleForWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return GetSystemDpiScale();

            try
            {
                // Windows 10 1607+
                int dpi = GetDpiForWindow(hWnd);
                if (dpi > 0) return dpi / 96.0;
            }
            catch { }

            try
            {
                // Fallback: через монитор
                IntPtr monitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    int hr = GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out uint dpiX, out uint dpiY);
                    if (hr == 0 && dpiX > 0) return dpiX / 96.0;
                }
            }
            catch { }

            return GetSystemDpiScale();
        }

        public double GetDpiScaleAtPoint(int x, int y)
        {
            try
            {
                var pt = new NativeMethods.POINT { X = x, Y = y };
                IntPtr monitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    int hr = GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out uint dpiX, out uint dpiY);
                    if (hr == 0 && dpiX > 0) return dpiX / 96.0;
                }
            }
            catch { }

            return GetSystemDpiScale();
        }

        public (int x, int y) AdjustCoordinates(int x, int y, double recordedDpi, double currentDpi)
        {
            if (Math.Abs(recordedDpi - currentDpi) < 0.01) return (x, y);
            if (recordedDpi <= 0) recordedDpi = 1.0;

            double ratio = currentDpi / recordedDpi;
            return ((int)(x * ratio), (int)(y * ratio));
        }

        public (int width, int height) AdjustSize(int width, int height, double recordedDpi, double currentDpi)
        {
            if (Math.Abs(recordedDpi - currentDpi) < 0.01) return (width, height);
            if (recordedDpi <= 0) recordedDpi = 1.0;

            double ratio = currentDpi / recordedDpi;
            return ((int)(width * ratio), (int)(height * ratio));
        }
    }
}