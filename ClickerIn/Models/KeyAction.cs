using System.Windows.Input;
using ClickerIn.Helpers;

namespace ClickerIn.Models
{
    public sealed class KeyAction : BindableBase
    {
        private Key _key;
        private ModifierKeys _modifiers;
        private string _text;
        private int _holdDurationMs;

        public Key Key { get => _key; set => Set(ref _key, value); }
        public ModifierKeys Modifiers { get => _modifiers; set => Set(ref _modifiers, value); }
        public string Text { get => _text; set => Set(ref _text, value); }
        public int HoldDurationMs { get => _holdDurationMs; set => Set(ref _holdDurationMs, value); }

        public string DisplayString
        {
            get
            {
                if (!string.IsNullOrEmpty(Text)) return $"\"{Text}\"";
                var mod = Modifiers != ModifierKeys.None ? $"{Modifiers}+" : "";
                return $"{mod}{Key}" + (HoldDurationMs > 0 ? $" ({HoldDurationMs}мс)" : "");
            }
        }
    }
}