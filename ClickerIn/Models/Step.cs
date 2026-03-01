using System.Collections.Generic;
using ClickerIn.Helpers;

namespace ClickerIn.Models
{
    public enum ClickType { Left, Right, Middle, DoubleLeft, DoubleRight }
    public enum CoordinateType { Global, RelativeToWindow }
    public enum TargetType { Desktop, WindowByTitle, WindowByProcess }

    public sealed class Step : BindableBase
    {
        private int _id;
        private StepActionType _actionType = StepActionType.Click;
        private ClickType _clickType;
        private int _x, _y;
        private CoordinateType _coordType;
        private int _delayBeforeMs, _delayAfterMs = 100;
        private bool _randomizeCoords;
        private int _randomCoordsRadius = 5;
        private bool _randomizeDelayBefore, _randomizeDelayAfter;
        private int _randomDelayBeforeRange = 50, _randomDelayAfterRange = 50;
        private TargetType _target;
        private string _targetName, _imagePath;
        private double _imageThreshold = 0.8;
        private bool _useCV;
        private string _recordedProcessName;
        private string _recordedWindowTitle;
        private int _relativeX, _relativeY;
        private KeyAction _keyAction;
        private int _scrollAmount;
        private bool _scrollHorizontal;
        private int _dragToX, _dragToY;
        private string _inputText;
        private bool _waitForImage;
        private int _waitTimeoutMs = 10000;
        private string _comment;
        private bool _enabled = true;
        private int _retryCount;
        private int _retryDelayMs = 500;
        private MouseMovementData _mouseMovement;
        private double _recordedDpiScale = 1.0;
        private bool _isDragStart;
        private bool _isDragEnd;
        private ClickType _mouseButtonHeld = ClickType.Left;

        public int Id { get => _id; set => Set(ref _id, value); }
        public StepActionType ActionType { get => _actionType; set { Set(ref _actionType, value); OnPropertyChanged(nameof(DisplayAction)); } }
        public ClickType ClickType { get => _clickType; set => Set(ref _clickType, value); }
        public int X { get => _x; set => Set(ref _x, value); }
        public int Y { get => _y; set => Set(ref _y, value); }
        public CoordinateType CoordType { get => _coordType; set => Set(ref _coordType, value); }
        public int DelayBeforeMs { get => _delayBeforeMs; set => Set(ref _delayBeforeMs, value); }
        public int DelayAfterMs { get => _delayAfterMs; set => Set(ref _delayAfterMs, value); }
        public bool RandomizeCoords { get => _randomizeCoords; set => Set(ref _randomizeCoords, value); }
        public int RandomCoordsRadius { get => _randomCoordsRadius; set => Set(ref _randomCoordsRadius, value); }
        public bool RandomizeDelayBefore { get => _randomizeDelayBefore; set => Set(ref _randomizeDelayBefore, value); }
        public int RandomDelayBeforeRange { get => _randomDelayBeforeRange; set => Set(ref _randomDelayBeforeRange, value); }
        public bool RandomizeDelayAfter { get => _randomizeDelayAfter; set => Set(ref _randomizeDelayAfter, value); }
        public int RandomDelayAfterRange { get => _randomDelayAfterRange; set => Set(ref _randomDelayAfterRange, value); }
        public TargetType Target { get => _target; set => Set(ref _target, value); }
        public string TargetName { get => _targetName; set => Set(ref _targetName, value); }
        public string ImagePath { get => _imagePath; set => Set(ref _imagePath, value); }
        public double ImageThreshold { get => _imageThreshold; set => Set(ref _imageThreshold, value); }
        public bool UseCV { get => _useCV; set => Set(ref _useCV, value); }
        public string RecordedProcessName { get => _recordedProcessName; set => Set(ref _recordedProcessName, value); }
        public string RecordedWindowTitle { get => _recordedWindowTitle; set => Set(ref _recordedWindowTitle, value); }
        public int RelativeX { get => _relativeX; set => Set(ref _relativeX, value); }
        public int RelativeY { get => _relativeY; set => Set(ref _relativeY, value); }
        public KeyAction KeyAction { get => _keyAction; set => Set(ref _keyAction, value); }
        public int ScrollAmount { get => _scrollAmount; set => Set(ref _scrollAmount, value); }
        public bool ScrollHorizontal { get => _scrollHorizontal; set => Set(ref _scrollHorizontal, value); }
        public int DragToX { get => _dragToX; set => Set(ref _dragToX, value); }
        public int DragToY { get => _dragToY; set => Set(ref _dragToY, value); }
        public string InputText { get => _inputText; set => Set(ref _inputText, value); }
        public bool WaitForImage { get => _waitForImage; set => Set(ref _waitForImage, value); }
        public int WaitTimeoutMs { get => _waitTimeoutMs; set => Set(ref _waitTimeoutMs, value); }
        public string Comment { get => _comment; set => Set(ref _comment, value); }
        public bool Enabled { get => _enabled; set => Set(ref _enabled, value); }
        public int RetryCount { get => _retryCount; set => Set(ref _retryCount, value); }
        public int RetryDelayMs { get => _retryDelayMs; set => Set(ref _retryDelayMs, value); }
        public MouseMovementData MouseMovement { get => _mouseMovement; set => Set(ref _mouseMovement, value); }
        public double RecordedDpiScale { get => _recordedDpiScale; set => Set(ref _recordedDpiScale, value); }
        public bool IsDragStart { get => _isDragStart; set => Set(ref _isDragStart, value); }
        public bool IsDragEnd { get => _isDragEnd; set => Set(ref _isDragEnd, value); }
        public ClickType MouseButtonHeld { get => _mouseButtonHeld; set => Set(ref _mouseButtonHeld, value); }

        public Step Clone()
        {
            var clone = (Step)MemberwiseClone();
            if (MouseMovement != null)
                clone.MouseMovement = new MouseMovementData { Points = new List<MousePoint>(MouseMovement.Points) };
            if (KeyAction != null)
                clone.KeyAction = new KeyAction
                {
                    Key = KeyAction.Key,
                    Modifiers = KeyAction.Modifiers,
                    Text = KeyAction.Text,
                    HoldDurationMs = KeyAction.HoldDurationMs
                };
            return clone;
        }

        public string DisplayAction
        {
            get
            {
                var movement = MouseMovement != null && MouseMovement.PointCount > 0
                    ? $" 🖱{MouseMovement.PointCount}pts" : "";
                switch (ActionType)
                {
                    case StepActionType.Click: return $"{ClickType} ({X},{Y}){movement}";
                    case StepActionType.KeyPress: return $"Клавиша: {KeyAction?.DisplayString}";
                    case StepActionType.KeyCombo: return $"Комбо: {KeyAction?.DisplayString}";
                    case StepActionType.Wait: return $"Ждать {DelayBeforeMs}мс";
                    case StepActionType.MoveMouse: return $"Мышь → ({X},{Y}){movement}";
                    case StepActionType.Scroll:
                        var dir = ScrollHorizontal ? "гориз" : (ScrollAmount > 0 ? "вверх" : "вниз");
                        return $"Скролл {dir}: {System.Math.Abs(ScrollAmount)}";
                    case StepActionType.DragDrop: return $"Тащить ({X},{Y})→({DragToX},{DragToY}){movement}";
                    case StepActionType.TextInput: return $"Текст: \"{InputText}\"";
                    case StepActionType.ScreenCheck: return "Проверка экрана";
                    case StepActionType.MouseDown: return $"Зажать {MouseButtonHeld} ({X},{Y})";
                    case StepActionType.MouseUp: return $"Отпустить {MouseButtonHeld} ({X},{Y})";
                    default: return ActionType.ToString();
                }
            }
        }
    }
}