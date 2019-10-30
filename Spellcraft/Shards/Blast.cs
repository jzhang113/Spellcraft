using GoRogue.GameFramework;
using Spellcraft.Components;

namespace Spellcraft.Shards
{
    internal class Blast : IShard
    {
        public string Name => "Blast";
        public char Symbol => 'B';

        public SpellResolver Primary(IGameObject caster)
        {
            return new SpellResolver(targets =>
            {
                targets.ForEach(pos =>
                {
                    Game.Map.GetEntity<IGameObject>(pos)
                            ?.GetComponent<HealthComponent>()
                            ?.Damage(1);
                });
            })
            {
                Radius = 1,
                Target = caster.Position
            };
        }

        public SpellResolver Secondary(SpellResolver parent)
        {
            parent.Radius++;
            return parent;
        }
    }
}
