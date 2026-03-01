using ClickerIn.Helpers;

namespace ClickerIn.Models
{
    public sealed class WindowProfile : BindableBase
    {
        private string _processName;
        private string _windowTitle;
        private int _x, _y, _width, _height;
        private double _dpiScale = 1.0;
        private int _screenWidth, _screenHeight;
        private bool _restorePosition = true;
        private bool _restoreSize = true;

        public string ProcessName { get => _processName; set => Set(ref _processName, value); }
        public string WindowTitle { get => _windowTitle; set => Set(ref _windowTitle, value); }
        public int X { get => _x; set => Set(ref _x, value); }
        public int Y { get => _y; set => Set(ref _y, value); }
        public int Width { get => _width; set => Set(ref _width, value); }
        public int Height { get => _height; set => Set(ref _height, value); }
        public double DpiScale { get => _dpiScale; set => Set(ref _dpiScale, value); }
        public int ScreenWidth { get => _screenWidth; set => Set(ref _screenWidth, value); }
        public int ScreenHeight { get => _screenHeight; set => Set(ref _screenHeight, value); }
        public bool RestorePosition { get => _restorePosition; set => Set(ref _restorePosition, value); }
        public bool RestoreSize { get => _restoreSize; set => Set(ref _restoreSize, value); }
        public string DisplayInfo => $"{ProcessName} ({Width}x{Height} @ {X},{Y}) DPI:{DpiScale:F2}";
    }
}