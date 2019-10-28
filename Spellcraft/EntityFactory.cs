using GoRogue;
using GoRogue.GameFramework;
using Spellcraft.Components;
using System.Drawing;

namespace Spellcraft.Entities
{
    static class EntityFactory
    {
        public static IGameObject Player(Coord position)
        {
            var player = new GameObject(position, 1, null, false, false, true);
            player.AddComponent(new DrawComponent('@', Color.White));
            return player;
        }
    }
}
