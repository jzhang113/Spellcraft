using GoRogue;
using System;
using System.Collections.Generic;

namespace Spellcraft
{
    class SpellResolver
    {
        public Action<IEnumerable<Coord>> Effect { get; }
        public Coord Target { get; set; }
        public int Radius { get; set; }

        public SpellResolver(Coord target, Action<IEnumerable<Coord>> effect)
        {
            Target = target;
            Effect = effect;
        }

        public void Resolve()
        {
            ICollection<Coord> targets = new List<Coord>();

            Target = Clamp(Target);

            for (int i = Target.X - Radius; i <= Target.X + Radius; i++)
            {
                for (int j = Target.Y - Radius; j <= Target.Y + Radius; j++)
                {
                    if (i >= 0 && i < Game.Map.Width && j >= 0 && j < Game.Map.Height)
                        targets.Add(new Coord(i, j));
                }
            }

            Effect(targets);
        }

        private Coord Clamp(Coord coord)
        {
            int newX = Math.Min(Game.Map.Width - 1, coord.X);
            newX = Math.Max(0, newX);

            int newY = Math.Min(Game.Map.Height - 1, coord.Y);
            newY = Math.Max(0, newY);

            return (newX, newY);
        }
    }
}
