using GoRogue;
using GoRogue.DiceNotation;
using GoRogue.GameFramework;
using System;
using System.Linq;

namespace Spellcraft.Shards
{
    enum MoveDir
    {
        N, E, S, W
    }

    class Move : IShard
    {
        private static readonly Coord[] _modifiers = new Coord[0];
        private static readonly IDiceExpression _dirChoice = Dice.Parse("1d4-1");

        public string Name { get; }
        public char Symbol { get; }
        public Coord[] Modifiers => _modifiers;

        private readonly Direction _dir;

        public Move() : this((MoveDir) _dirChoice.Roll()) { }

        public Move(MoveDir dir)
        {
            switch (dir)
            {
                case MoveDir.N:
                    _dir = Direction.UP;
                    Name = "North";
                    Symbol = 'N';
                    break;
                case MoveDir.E:
                    _dir = Direction.RIGHT;
                    Name = "East";
                    Symbol = 'E';
                    break;
                case MoveDir.S:
                    _dir = Direction.DOWN;
                    Name = "South";
                    Symbol = 'S';
                    break;
                case MoveDir.W:
                    _dir = Direction.LEFT;
                    Name = "West";
                    Symbol = 'W';
                    break;
            }
        }

        public SpellResolver Primary(IGameObject caster)
        {
            return new SpellResolver(caster.Position + _dir, targets =>
            {
                Coord t = targets.LastOrDefault();
                if (t != default(Coord))
                    caster.Position = t;
            });
        }

        public SpellResolver Secondary(SpellResolver parent)
        {
            parent.Target += _dir;
            return parent;
        }
    }
}
