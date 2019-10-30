using GoRogue;
using GoRogue.DiceNotation;
using GoRogue.GameFramework;
using System;
using System.Linq;

namespace Spellcraft.Shards
{
    class Move : IShard
    {
        public string Name { get; }
        public char Symbol { get; }
        private readonly Direction _dir;
        private readonly IDiceExpression _dirChoice = Dice.Parse("1d4");

        public Move()
        {
            switch (_dirChoice.Roll())
            {
                case 1:
                    _dir = Direction.UP;
                    Name = "North";
                    Symbol = 'N';
                    break;
                case 2:
                    _dir = Direction.RIGHT;
                    Name = "East";
                    Symbol = 'E';
                    break;
                case 3:
                    _dir = Direction.DOWN;
                    Name = "South";
                    Symbol = 'S';
                    break;
                case 4:
                    _dir = Direction.LEFT;
                    Name = "West";
                    Symbol = 'W';
                    break;
            }
        }

        public SpellResolver Primary(IGameObject caster)
        {
            return new SpellResolver(targets =>
            {
                Coord t = targets.LastOrDefault();
                if (t != default(Coord))
                    caster.Position = t;
            })
            {
                Radius = 1,
                Target = caster.Position + _dir
            };
        }

        public SpellResolver Secondary(SpellResolver parent)
        {
            parent.Target += _dir;
            return parent;
        }
    }
}
