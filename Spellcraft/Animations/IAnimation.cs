﻿using System;

namespace Spellcraft.Animations
{
    public interface IAnimation
    {
        TimeSpan Duration { get; }

        TimeSpan CurrentTime { get; }

        TimeSpan EndTime { get; }

        // Returns true when an animation is done updating
        bool Update(TimeSpan dt);

        // Draw the animation to the screen
        void Draw();
    }
}
