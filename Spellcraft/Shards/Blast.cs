using GoRogue.GameFramework;
using Spellcraft.Components;

namespace Spellcraft.Shards
{
    internal class Blast : IShard
    {
        public string Name => "Blast";
        public char Symbol => 'B';

        public SpellResolver Primary()
        {
            var spellBase = new SpellResolver(targets =>
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
                Target = Game.Player.Position
            };
            return spellBase;
        }

        public SpellResolver Secondary(SpellResolver parent)
        {
            parent.Radius++;
            return parent;
        }
    }
}
