using System.Collections.Generic;

namespace ClickerIn.Models
{
    public struct MousePoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int DelayMs { get; set; }

        public MousePoint(int x, int y, int delayMs)
        {
            X = x; Y = y; DelayMs = delayMs;
        }
    }

    public sealed class MouseMovementData
    {
        public List<MousePoint> Points { get; set; } = new List<MousePoint>();
        public int PointCount => Points?.Count ?? 0;
    }
}