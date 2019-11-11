using GoRogue;
using GoRogue.GameFramework;
using Spellcraft.Animations;
using Spellcraft.Components;
using System;
using System.Drawing;

namespace Spellcraft.Shards
{
    internal class Blast : IShard
    {
        private static readonly Coord[] _modifiers = new Coord[1] { (1, 0) };

        public string Name => "Blast";
        public char Symbol => 'B';
        public Coord[] Modifiers => _modifiers;

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

                Game.Animations.Add(
                    (int)caster.ID,
                    new FlashAnimation(targets, Color.Red, TimeSpan.FromMilliseconds(400)));
            });
        }

        public SpellResolver Secondary(SpellResolver parent)
        {
            parent.Radius++;
            return parent;
        }
    }
}
