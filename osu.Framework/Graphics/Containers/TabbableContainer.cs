﻿using System;
using System.Linq;
using osu.Framework.Input;
using osu.Framework.Lists;
using OpenTK.Input;

namespace osu.Framework.Graphics.Containers
{
    public class TabbableContainer : TabbableContainer<Drawable> { }

    public class TabbableContainer<T> : Container<T> 
        where T : Drawable
    {
        /// <summary>
        /// Allows for tabbing between multiple levels within the BaseContainer.
        /// </summary>
        public Container<Drawable> BaseContainer { private get; set; }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (args.Key != Key.Tab)
                return false;
            var next = nextTabStop(BaseContainer, state.Keyboard.ShiftPressed ? -1 : 1);
            next?.TriggerFocus();
            return true;
        }

        private Drawable nextTabStop(Container<Drawable> target, int step, bool intialized = false)
        {
            if (intialized && target is TabbableContainer)
                return target;

            LifetimeList<Drawable> children = target?.Children as LifetimeList<Drawable>;
            if (children == null)
                return null;

            var filtered = children.FindAll(t => t is Container<Drawable>)
                .Cast<Container<Drawable>>()
                .ToList();

            int current = 0;
            if (!intialized)
            {
                // Find self, to know starting point
                current = filtered.IndexOf(this as TabbableContainer);
                // Search own children
                if (current != -1)
                {
                    intialized = true;
                    var next = nextTabStop(filtered[current], step);
                    if (next != null)
                        return next;
                    current += step;
                }
                else
                    current = 0;
            }

            // Search other children
            for (int i = 0; i < filtered.Count; i++)
            {
                var next = nextTabStop(filtered[Math.Abs(current + step * i) % filtered.Count], step, intialized);
                if (next != null)
                    return next;
            }

            return null;
        }
    }
}
