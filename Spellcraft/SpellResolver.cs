using GoRogue;
using System;
using System.Collections.Generic;

namespace Spellcraft
{
    class SpellResolver
    {
        public Action<IEnumerable<Coord>> Effect { get; }

        public Coord Target { get; set; }
        public bool NeedsTarget { get; set; }
        public int Radius { get; set; }

        public SpellResolver(Action<IEnumerable<Coord>> effect)
        {
            Effect = effect;
        }

        public void Resolve()
        {
            if (NeedsTarget)
            {
                // get player targetting
            }

            Effect(new List<Coord>() { Target });
        }
    }
}
