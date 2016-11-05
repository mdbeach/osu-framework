﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using OpenTK;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Containers;
using System;
using System.Collections.Generic;

namespace osu.Framework.Graphics.UserInterface
{
    public enum DropDownMenuState
    {
        Closed,
        Opened,
    }

    public class DropDownMenu<T> : Container, IBindable, IStateful<DropDownMenuState>
    {
        private bool opened;
        private bool listInitialized = false;

        private readonly List<DropDownMenuItem<T>> selectableItems = new List<DropDownMenuItem<T>>();
        private List<DropDownMenuItem<T>> internalItems = new List<DropDownMenuItem<T>>();

        public IEnumerable<DropDownMenuItem<T>> Items
        {
            get
            {
                return internalItems;
            }
            set
            {
                ClearItems();
                internalItems = new List<DropDownMenuItem<T>>(value);
                for (int i = 0; i < internalItems.Count; i++)
                {
                    addMenuItem(internalItems[i], i);
                }
            }
        }

        protected DropDownComboBox ComboBox;

        protected ScrollContainer DropDown;
        protected FlowContainer DropDownList;
        protected virtual float DropDownListSpacing => 0;

        protected virtual DropDownComboBox CreateComboBox()
        {
            return new DropDownComboBox();
        }

        private int maxDropDownHeight = 100;

        /// <summary>
        /// Maximum height the Drop-down box can reach.
        /// </summary>
        public int MaxDropDownHeight
        {
            get
            {
                return maxDropDownHeight;
            }
            set
            {
                maxDropDownHeight = value;
                updateDropDownListSize();
            }
        }

        public string Description { get; set; }

        private int selectedIndex = -1;

        /// <summary>
        /// Gets the index of the selected item in the menu.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (SelectedItem != null)
                    SelectedItem.IsSelected = false;

                selectedIndex = value;

                SelectedItem.IsSelected = true;
                TriggerValueChanged();
            }
        }

        protected DropDownMenuItem<T> SelectedItem
        {
            get
            {
                if (SelectedIndex < 0)
                    return null;
                return selectableItems[SelectedIndex];
            }
        }

        /// <summary>
        /// Gets the selected item in the menu.
        /// </summary>
        public T SelectedValue
        {
            get
            {
                if (SelectedItem == null)
                    return default(T);
                return SelectedItem.Value;
            }
            set
            {
                Parse(value);
            }
        }

        public DropDownMenuState State
        {
            get
            {
                return opened ? DropDownMenuState.Opened : DropDownMenuState.Closed;
            }
            set
            {
                switch (value)
                {
                    case DropDownMenuState.Closed:
                        Close();
                        break;
                    case DropDownMenuState.Opened:
                        Open();
                        break;
                }
            }
        }

        /// <summary>
        /// Occurs when the selected item changes.
        /// </summary>
        public event EventHandler ValueChanged;

        public DropDownMenu()
        {
            AutoSizeAxes = Axes.Y;

            Children = new Drawable[]
            {
                ComboBox = CreateComboBox(),
                DropDown = new ScrollContainer
                {
                    RelativeSizeAxes = Axes.X,
                    Alpha = 0,
                    Children = new Drawable[]
                    {
                        DropDownList = new FlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Width = 1,
                            Direction = FlowDirection.VerticalOnly,
                        },
                    },
                }
            };

            ComboBox.Action = Toggle;
            ComboBox.CloseAction = Close;

            DropDownList.OnAutoSize += updateDropDownListSize;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            ComboBox.UpdateLabel(SelectedItem?.DisplayText);
        }

        public void Open()
        {
            if (!IsLoaded)
                return;

            opened = true;
            if (!listInitialized)
                initializeDropDownList();
            AnimateOpen();
        }

        public void Close()
        {
            opened = false;
            if (IsLoaded)
                AnimateClose();
        }

        public bool Parse(object value)
        {
            if (selectableItems == null)
                return false;

            for (int i = 0; i < selectableItems.Count; i++)
            {
                if (value.Equals(selectableItems[i].Value))
                {
                    SelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        public void UnbindAll()
        {
            ValueChanged = null;
        }

        public void TriggerValueChanged()
        {
            ComboBox.UpdateLabel(SelectedItem?.DisplayText);
            Close();
            ValueChanged?.Invoke(this, null);
        }

        protected virtual void AnimateOpen()
        {
            foreach (Drawable child in DropDownList.Children)
                child.Show();
            DropDown.Show();
        }

        protected virtual void AnimateClose()
        {
            foreach (Drawable child in DropDownList.Children)
                child.Hide();
            DropDown.Hide();
        }

        public void Toggle()
        {
            if (opened)
                Close();
            else
                Open();
        }

        public void AddItem(DropDownMenuItem<T> item)
        {
            addMenuItem(item, internalItems.Count);
            internalItems.Add(item);
            if (listInitialized)
                DropDownList.Add(item);
        }

        public void AddItemRange(IEnumerable<DropDownMenuItem<T>> range)
        {
            foreach (DropDownMenuItem<T> item in range)
            {
                AddItem(item);
            }
        }

        public void ClearItems()
        {
            internalItems.Clear();
            selectableItems.Clear();
            DropDownList.Clear();
        }

        private void addMenuItem(DropDownMenuItem<T> item, int index)
        {
            item.PositionIndex = index;
            if (item.CanSelect)
            {
                item.Index = selectableItems.Count;
                item.Action = delegate
                {
                    if (opened)
                        SelectedIndex = item.Index;
                };
                selectableItems.Add(item);
            }
        }

        private void initializeDropDownList()
        {
            if (DropDownList == null || !DropDownList.IsLoaded)
                return;
            
            for (int i = 0; i < internalItems.Count; i++)
                DropDownList.Add(internalItems[i]);

            DropDown.Position = new Vector2(0, ComboBox.Height + DropDownListSpacing);

            listInitialized = true;
        }

        private void updateDropDownListSize()
        {
            DropDown.Height = Math.Min(DropDownList.Height, MaxDropDownHeight);
        }
    }
}