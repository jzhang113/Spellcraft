using GoRogue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spellcraft
{
    internal class SpellResolver
    {
        public Action<IEnumerable<Coord>> Effect { get; }
        public Coord Target { get; set; }
        public int Radius { get; set; }
        public ICollection<Direction> MoveRecord { get; }

        private readonly AreaType _shape;

        public SpellResolver(Coord target, AreaType shape, Action<IEnumerable<Coord>> effect)
        {
            _shape = shape;

            Target = target;
            Effect = effect;
            MoveRecord = new List<Direction>();
        }

        public void Resolve()
        {

            ICollection<Coord> targets = new List<Coord>();

            switch (_shape)
            {
                case AreaType.Move:
                    targets = MoveRecord.Select(dir => new Coord(dir.DeltaX, dir.DeltaY)).ToList();
                    break;
                case AreaType.Area:
                    foreach (Direction dir in MoveRecord)
                    {
                        Target += dir;
                    }

                    Target = Clamp(Target);

                    for (int i = Target.X - Radius; i <= Target.X + Radius; i++)
                    {
                        for (int j = Target.Y - Radius; j <= Target.Y + Radius; j++)
                        {
                            if (i >= 0 && i < Game.Map.Width && j >= 0 && j < Game.Map.Height)
                                targets.Add(new Coord(i, j));
                        }
                    }
                    break;
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

    internal enum AreaType
    {
        Area,
        Move
    }
}
