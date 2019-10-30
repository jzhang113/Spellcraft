using BearLib;
using GoRogue;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Spellcraft.Animations
{
    internal class FlashAnimation : IAnimation
    {
        public TimeSpan Duration { get; } = Game.FrameRate * 4;
        public TimeSpan CurrentTime { get; private set; }
        public TimeSpan EndTime { get; }

        private readonly IEnumerable<Coord> _pos;
        private readonly Color _color;

        public FlashAnimation(IEnumerable<Coord> pos, in Color color)
        {
            _pos = pos;
            _color = color;

            CurrentTime = TimeSpan.Zero;
            EndTime = CurrentTime + Duration;
        }

        public bool Update(TimeSpan dt)
        {
            CurrentTime += dt;
            return CurrentTime >= EndTime;
        }

        public void Draw()
        {
            double fracPassed = CurrentTime / Duration;
            Color between = _color.Blend(Color.White, fracPassed);
            Terminal.Color(between);
            Terminal.Layer(2);
            foreach (Coord pos in _pos) {
                Terminal.Put(pos.X, pos.Y, '▓');
            }
            Terminal.Layer(1);
        }
    }
}
