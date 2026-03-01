using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using ClickerIn.Helpers;

namespace ClickerIn.Services
{
    public enum MouseEventType
    {
        Move, LeftDown, LeftUp, RightDown, RightUp, MiddleDown, MiddleUp, DoubleLeft, DoubleRight, Scroll
    }

    public sealed class MouseEventArgs
    {
        public MouseEventType Type { get; set; }
        public Point Position { get; set; }
        public int ScrollDelta { get; set; }
        public long Timestamp { get; set; }
    }

    public sealed class MouseHookService : IDisposable
    {
        private IntPtr _hook;
        private readonly LowLevelMouseProc _proc;

        public event Action<Point> MouseMoved;
        public event Action<Point> LeftClickRecorded;
        public event Action<Point> RightClickRecorded;
        public event Action<Point> MiddleClickRecorded;
        public event Action<Point> DoubleClickRecorded;
        public event Action<MouseEventArgs> MouseEvent;

        public MouseHookService()
        {
            _proc = HookCallback;
            using (var p = Process.GetCurrentProcess())
            using (var m = p.MainModule)
                _hook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, _proc,
                    NativeMethods.GetModuleHandle(m.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var info = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                var pt = new Point(info.pt.X, info.pt.Y);
                int msg = wParam.ToInt32();

                switch (msg)
                {
                    case NativeMethods.WM_MOUSEMOVE:
                        MouseMoved?.Invoke(pt);
                        RaiseEvent(MouseEventType.Move, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_LBUTTONDOWN:
                        LeftClickRecorded?.Invoke(pt);
                        RaiseEvent(MouseEventType.LeftDown, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_LBUTTONUP:
                        RaiseEvent(MouseEventType.LeftUp, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_LBUTTONDBLCLK:
                        DoubleClickRecorded?.Invoke(pt);
                        RaiseEvent(MouseEventType.DoubleLeft, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_RBUTTONDOWN:
                        RightClickRecorded?.Invoke(pt);
                        RaiseEvent(MouseEventType.RightDown, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_RBUTTONUP:
                        RaiseEvent(MouseEventType.RightUp, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_RBUTTONDBLCLK:
                        RaiseEvent(MouseEventType.DoubleRight, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_MBUTTONDOWN:
                        MiddleClickRecorded?.Invoke(pt);
                        RaiseEvent(MouseEventType.MiddleDown, pt, 0, info.time);
                        break;

                    case NativeMethods.WM_MBUTTONUP:
                        RaiseEvent(MouseEventType.MiddleUp, pt, 0, info.time);
                        break;

                    case 0x020A: // WM_MOUSEWHEEL
                        short delta = (short)(info.mouseData >> 16);
                        RaiseEvent(MouseEventType.Scroll, pt, delta, info.time);
                        break;
                }
            }
            return NativeMethods.CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        private void RaiseEvent(MouseEventType type, Point pos, int scrollDelta, uint timestamp)
        {
            MouseEvent?.Invoke(new MouseEventArgs
            {
                Type = type,
                Position = pos,
                ScrollDelta = scrollDelta,
                Timestamp = timestamp
            });
        }

        public void Dispose()
        {
            if (_hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hook);
                _hook = IntPtr.Zero;
            }
        }
    }
}