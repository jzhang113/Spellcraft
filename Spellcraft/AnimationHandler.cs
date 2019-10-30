using Spellcraft.Animations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spellcraft
{
    class AnimationHandler
    {
        private IDictionary<int, List<IAnimation>> _current { get; }

        public AnimationHandler()
        {
            _current = new Dictionary<int, List<IAnimation>>();
        }

        public void Clear() => _current.Clear();

        public void Add(int id, IAnimation animation)
        {
            if (_current.TryGetValue(id, out List<IAnimation> queue))
                queue.Add(animation);
            else
                _current.Add(id, new List<IAnimation>() { animation });
        }

        public bool IsDone()
        {
            foreach ((int _, List<IAnimation> queue) in _current)
            {
                if (queue.Count != 0)
                    return false;
            }

            return true;
        }

        public void Run(TimeSpan frameTime, double remaining)
        {
            foreach ((int _, List<IAnimation> queue) in _current)
            {
                IAnimation animation = queue.FirstOrDefault();
                if (animation?.Update(frameTime) ?? false)
                {
                    queue.Remove(animation);
                }
            }
        }

        public void Draw()
        {
            foreach ((int _, List<IAnimation> queue) in _current)
            {
                IAnimation animation = queue.FirstOrDefault();
                animation?.Draw();
            }
        }
    }
}
