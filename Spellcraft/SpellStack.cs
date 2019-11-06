using BearLib;
using GoRogue;
using GoRogue.GameFramework;
using Spellcraft.Shards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spellcraft
{
    internal class SpellStack
    {
        private readonly IDictionary<Coord, IShard> _stack;
        private readonly IGameObject _caster;

        public SpellStack(IGameObject caster)
        {
            _caster = caster;
            _stack = new Dictionary<Coord, IShard>
            {
                [(0, 0)] = new Empty()
            };
        }


        public void Resolve()
        {
            // sanity check, but _stack should never be empty
            if (_stack.Count == 0)
                return;

            SpellResolver spell = _stack[(0, 0)].Primary(_caster);
            _stack.Skip(1)
                 .Aggregate(spell, (resolver, shard) => shard.Value.Secondary(resolver))
                 .Resolve();
        }

        public void Clear()
        {
            _stack.Clear();
            _stack.Add((0, 0), new Empty());
        }

        public bool Add(IShard shard)
        {
            int minDist = int.MaxValue;
            Coord closest = (-200, -200);

            // find closest Empty
            foreach ((Coord pos, IShard card) in _stack)
            {
                if (card is Empty)
                {
                    int dist = Math.Abs(pos.X) + Math.Abs(pos.Y);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = pos;
                    }
                }
            }

            if (closest != (-200, -200))
            {
                Add(closest, shard);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Add(Coord pos, IShard shard)
        {
            _stack[pos] = shard;
            foreach (Coord offset in shard.Modifiers)
            {
                Coord next = pos + offset;
                if (!_stack.ContainsKey(next))
                {
                    _stack[next] = new Empty();
                }
            }
        }

        public void Draw(int xPos, int yPos)
        {
            Terminal.Print(new Rectangle(xPos, yPos, 6, 1), "Stack:");

            foreach (((int cx, int cy), IShard card) in _stack)
            {
                Terminal.Put(xPos + cx * 2 + 2, yPos + cy + 1, '┬');
                Terminal.Put(xPos + cx * 2 + 2, yPos + cy + 3, '┴');
                Terminal.Put(xPos + cx * 2 + 1, yPos + cy + 2, card.Symbol);
            }

            Terminal.Put(xPos, yPos + 1, '┌');
            Terminal.Put(xPos, yPos + 3, '└');
            Terminal.Put(xPos + _stack.Count * 2, yPos + 1, '┐');
            Terminal.Put(xPos + _stack.Count * 2, yPos + 3, '┘');
        }
    }
}
