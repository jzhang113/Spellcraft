using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoRogue;
using GoRogue.GameFramework;

namespace Spellcraft.Shards
{
    class Dash : IShard
    {
        private static readonly Coord[] _modifiers = new Coord[3] { (1, 0), (1, -1), (1, 1) };

        public string Name => "Dash";
        public char Symbol => 'D';
        public Coord[] Modifiers => _modifiers;

        public SpellResolver Primary(IGameObject caster)
        {
            return new SpellResolver(caster.Position, AreaType.Move, targets =>
            {
                targets.ForEach(dir => caster.Position += dir);
            });
        }

        public SpellResolver Secondary(SpellResolver parent)
        {
            throw new NotImplementedException();
        }
    }
}
