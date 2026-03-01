using System.Windows.Input;

namespace ClickerIn.Models
{
    public sealed class HotkeySettings
    {
        public Key Key { get; set; } = Key.F8;
        public ModifierKeys Modifiers { get; set; } = ModifierKeys.Control;
        public bool IsGlobal { get; set; } = true;
        public string DisplayString => (Modifiers != ModifierKeys.None ? $"{Modifiers}+" : "") + Key;
    }
}