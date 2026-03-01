using System;
using System.Collections.Generic;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;

namespace ClickerIn.Services
{
    public interface IHotkeyService : IDisposable
    {
        void Register(string name, Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler);
        void Unregister(string name);
        void UnregisterAll();
    }

    public sealed class HotkeyService : IHotkeyService
    {
        private readonly HashSet<string> _names = new HashSet<string>();

        public void Register(string name, Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler)
        {
            try
            {
                HotkeyManager.Current.AddOrReplace(name, key, modifiers, handler);
                _names.Add(name);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hotkey fail [{name}]: {ex.Message}");
            }
        }

        public void Unregister(string name)
        {
            try { HotkeyManager.Current.Remove(name); } catch { }
            _names.Remove(name);
        }

        public void UnregisterAll()
        {
            foreach (var n in _names)
                try { HotkeyManager.Current.Remove(n); } catch { }
            _names.Clear();
        }

        public void Dispose() => UnregisterAll();
    }
}

