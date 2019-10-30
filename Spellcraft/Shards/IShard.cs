using GoRogue.GameFramework;

namespace Spellcraft.Shards
{
    interface IShard
    {
        string Name { get; }
        char Symbol { get; }

        // Primary effect, when used as base
        SpellResolver Primary(IGameObject caster);

        // Secondary effect, when modifying parent
        SpellResolver Secondary(SpellResolver parent);
    }
}
