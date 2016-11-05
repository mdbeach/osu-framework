﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;

namespace osu.Framework.Graphics.UserInterface
{
    public enum DropDownMenuItemState
    {
        NotSelected,
        Selected,
    }

    public class DropDownMenuItem<T> : ClickableContainer, IStateful<DropDownMenuItemState>
    {
        public int Index;
        public int PositionIndex;
        public readonly string DisplayText;
        public readonly T Value;
        public virtual bool CanSelect { get; set; } = true;

        private bool selected;

        public bool IsSelected
        {
            get
            {
                if (!CanSelect)
                    return false;
                return selected;
            }
            set
            {
                selected = value;
                OnSelectChange();
            }
        }

        public DropDownMenuItemState State
        {
            get
            {
                return IsSelected ? DropDownMenuItemState.Selected : DropDownMenuItemState.NotSelected;
            }

            set
            {
                IsSelected = (value == DropDownMenuItemState.Selected);
            }
        }

        protected Box Background;
        protected virtual Color4 BackgroundColour => Color4.DarkSlateGray;
        protected virtual Color4 BackgroundColourHover => Color4.DarkGray;
        protected Container Foreground;
        protected SpriteText Label;
        protected Container Caret;
        protected virtual float CaretSpacing => 15;

        public DropDownMenuItem(string text, T value)
        {
            RelativeSizeAxes = Axes.X;
            Width = 1;
            AutoSizeAxes = Axes.Y;
            Masking = true;
            DisplayText = text;
            Value = value;

            Children = new Drawable[]
            {
                Background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },
                Foreground = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        Caret = new SpriteText(),
                        Label = new SpriteText
                        {
                            Margin = new MarginPadding { Left = CaretSpacing }
                        },
                    },
                },
            };
        }

        protected virtual void OnSelectChange()
        {
            if (!IsLoaded)
                return;

            FormatCaret();
        }

        protected virtual void FormatCaret()
        {
            (Caret as SpriteText).Text = IsSelected ? @">>" : @">";
        }

        protected virtual void FormatLabel()
        {
            Label.Text = DisplayText;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Background.Colour = BackgroundColour;
            FormatCaret();
            FormatLabel();
        }

        protected override bool OnHover(InputState state)
        {
            Background.Colour = BackgroundColourHover;
            return base.OnHover(state);
        }

        protected override void OnHoverLost(InputState state)
        {
            base.OnHover(state);
            Background.Colour = BackgroundColour;
        }
    }
}