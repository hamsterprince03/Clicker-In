using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ClickerIn.Helpers;

namespace ClickerIn.Services
{
    public interface IWindowManager
    {
        IntPtr GetRootWindow(IntPtr hwnd);
        string GetTitle(IntPtr hwnd);
        string GetProcessName(IntPtr hwnd);
        string GetClassName(IntPtr hwnd);
        bool BringToFront(IntPtr hwnd);
        void ForceForeground(IntPtr hwnd);
        bool IsWindowValid(IntPtr hwnd);
        IntPtr FindByTitle(string title);
        IntPtr FindByProcess(string processName);
        IntPtr FindWindowByProcess(string processName);
        bool GetWindowRect(IntPtr hwnd, out NativeMethods.RECT rect);
        List<IntPtr> GetAllWindows();
    }

    public sealed class WindowManagerService : IWindowManager
    {
        public IntPtr GetRootWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return IntPtr.Zero;
            return NativeMethods.GetAncestor(hwnd, NativeMethods.GA_ROOTOWNER);
        }

        public string GetTitle(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return string.Empty;
            int len = NativeMethods.GetWindowTextLength(hwnd);
            if (len <= 0) return string.Empty;
            var sb = new StringBuilder(len + 1);
            NativeMethods.GetWindowText(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public string GetProcessName(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return string.Empty;
            try
            {
                NativeMethods.GetWindowThreadProcessId(hwnd, out uint pid);
                if (pid == 0) return string.Empty;
                using (var p = Process.GetProcessById((int)pid))
                    return p.ProcessName;
            }
            catch { return string.Empty; }
        }

        public string GetClassName(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return string.Empty;
            var sb = new StringBuilder(256);
            NativeMethods.GetClassName(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public bool BringToFront(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return false;
            try
            {
                if (NativeMethods.IsIconic(hwnd))
                    NativeMethods.ShowWindow(hwnd, NativeMethods.SW_RESTORE);

                uint currentThread = NativeMethods.GetCurrentThreadId();
                uint targetThread = NativeMethods.GetWindowThreadProcessId(hwnd, out _);

                bool attached = false;
                if (currentThread != targetThread)
                    attached = NativeMethods.AttachThreadInput(currentThread, targetThread, true);

                NativeMethods.AllowSetForegroundWindow(NativeMethods.ASFW_ANY);
                NativeMethods.SetWindowPos(hwnd, NativeMethods.HWND_TOPMOST,
                    0, 0, 0, 0,
                    NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);
                NativeMethods.SetWindowPos(hwnd, NativeMethods.HWND_NOTOPMOST,
                    0, 0, 0, 0,
                    NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);

                NativeMethods.BringWindowToTop(hwnd);
                NativeMethods.SetForegroundWindow(hwnd);
                NativeMethods.SetFocus(hwnd);

                if (attached)
                    NativeMethods.AttachThreadInput(currentThread, targetThread, false);

                return true;
            }
            catch { return false; }
        }

        public void ForceForeground(IntPtr hwnd)
        {
            BringToFront(hwnd);
        }

        public bool IsWindowValid(IntPtr hwnd)
        {
            return hwnd != IntPtr.Zero && NativeMethods.IsWindow(hwnd);
        }

        public IntPtr FindByTitle(string title)
        {
            if (string.IsNullOrEmpty(title)) return IntPtr.Zero;
            IntPtr found = IntPtr.Zero;
            string lowerTitle = title.ToLowerInvariant();
            NativeMethods.EnumWindows((hwnd, _) =>
            {
                if (!NativeMethods.IsWindowVisible(hwnd)) return true;
                string wTitle = GetTitle(hwnd);
                if (!string.IsNullOrEmpty(wTitle) && wTitle.ToLowerInvariant().Contains(lowerTitle))
                {
                    found = hwnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return found;
        }

        public IntPtr FindByProcess(string processName)
        {
            if (string.IsNullOrEmpty(processName)) return IntPtr.Zero;
            string lowerName = processName.ToLowerInvariant().Replace(".exe", "");
            IntPtr found = IntPtr.Zero;
            NativeMethods.EnumWindows((hwnd, _) =>
            {
                if (!NativeMethods.IsWindowVisible(hwnd)) return true;
                string title = GetTitle(hwnd);
                if (string.IsNullOrEmpty(title)) return true;
                string proc = GetProcessName(hwnd);
                if (!string.IsNullOrEmpty(proc) && proc.ToLowerInvariant().Replace(".exe", "") == lowerName)
                {
                    found = hwnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return found;
        }

        public IntPtr FindWindowByProcess(string processName)
        {
            return FindByProcess(processName);
        }

        public bool GetWindowRect(IntPtr hwnd, out NativeMethods.RECT rect)
        {
            rect = default;
            if (hwnd == IntPtr.Zero) return false;
            return NativeMethods.GetWindowRect(hwnd, out rect);
        }

        public List<IntPtr> GetAllWindows()
        {
            var list = new List<IntPtr>();
            NativeMethods.EnumWindows((hwnd, _) =>
            {
                if (NativeMethods.IsWindowVisible(hwnd) && !string.IsNullOrEmpty(GetTitle(hwnd)))
                    list.Add(hwnd);
                return true;
            }, IntPtr.Zero);
            return list;
        }
    }
}