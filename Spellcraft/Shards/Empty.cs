using GoRogue;
using GoRogue.GameFramework;

namespace Spellcraft.Shards
{
    // Non-spell serving as a placeholder for other spells (dealing with nulls is messy)
    // Should do nothing if cast
    class Empty : IShard
    {
        private static readonly Coord[] _modifiers = new Coord[0];

        public string Name => "V";
        public char Symbol => ' ';

        public Coord[] Modifiers => _modifiers;

        public SpellResolver Primary(IGameObject caster) => new SpellResolver(caster.Position, _ => { });
        public SpellResolver Secondary(SpellResolver parent) => parent;
    }
}
