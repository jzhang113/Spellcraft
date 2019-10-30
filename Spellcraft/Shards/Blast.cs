using GoRogue.GameFramework;
using Spellcraft.Animations;
using Spellcraft.Components;

namespace Spellcraft.Shards
{
    internal class Blast : IShard
    {
        public string Name => "Blast";
        public char Symbol => 'B';

        public SpellResolver Primary(IGameObject caster)
        {
            return new SpellResolver(caster.Position, targets =>
            {
                targets.ForEach(pos =>
                {
                    Game.Map.GetEntity<IGameObject>(pos)
                            ?.GetComponent<HealthComponent>()
                            ?.Damage(1);
                });

                Game.Animations.Add((int)caster.ID, new FlashAnimation(targets, System.Drawing.Color.Red));
            });
        }

        public SpellResolver Secondary(SpellResolver parent)
        {
            parent.Radius++;
            return parent;
        }
    }
}
