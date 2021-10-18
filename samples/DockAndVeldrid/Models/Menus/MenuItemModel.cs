using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;

namespace AvaloniaDockApplication.Models.Menus
{
    public class MenuItemModel : ReactiveObject
    {
        public string Label => _menuItem.Value.Label;
        public DrawingGroup Icon => _menuItem.Value.Icon;

        public ICommand Command => _menuItem.Value.Command;

        public string Gesture => _menuItem.Value?.Gesture;

        public IEnumerable<MenuItemModel> Children { get; }

        private Lazy<IMenuItem> _menuItem;

        public MenuItemModel(
            Lazy<IMenuItem> menuItem, IEnumerable<MenuItemModel> children)
        {
            _menuItem = menuItem;

            Children = children?.ToList();
        }
    }
}