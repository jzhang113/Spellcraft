namespace Spellcraft.Shards
{
    interface IShard
    {
        string Name { get; }
        char Symbol { get; }

        // Primary effect, when used as base
        SpellResolver Primary();

        // Secondary effect, when modifying parent
        SpellResolver Secondary(SpellResolver parent);
    }
}
