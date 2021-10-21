using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AvaloniaDockApplication.Models.Menus;
using Dock.Model.Core;
using ReactiveUI;

namespace AvaloniaDockApplication.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private IFactory _factory;
		private IDock _layout;
		private string _currentView;

		public MainWindowViewModel()
		{
			var tb = new List<MenuItemModel>()
			{
				new MenuItemModel(
					new Lazy<IMenuItem>(
						() => new CommonMenuItem("Save",
							IconService.Instance.GetCompletionKindImage("Save"))),
					Enumerable.Empty<MenuItemModel>()),
				new MenuItemModel(
					new Lazy<IMenuItem>(
						() => new CommonMenuItem("SaveAll",
							IconService.Instance.GetCompletionKindImage("SaveAll"))),
					Enumerable.Empty<MenuItemModel>()),
				new MenuItemSeparatorModel(),
				new MenuItemModel(
					new Lazy<IMenuItem>(
						() => new CommonMenuItem("Undo",
							IconService.Instance.GetCompletionKindImage("Undo"))),
					Enumerable.Empty<MenuItemModel>()),
				new MenuItemModel(
					new Lazy<IMenuItem>(
						() => new CommonMenuItem("Redo",
							IconService.Instance.GetCompletionKindImage("Redo"))),
					Enumerable.Empty<MenuItemModel>()),
			};
			ToolBar = new ToolBarViewModel(ImmutableList.Create<MenuItemModel>(tb.ToArray()));
		}

		public IFactory Factory
		{
			get => _factory;
			set => this.RaiseAndSetIfChanged(ref _factory, value);
		}

		public IDock Layout
		{
			get => _layout;
			set => this.RaiseAndSetIfChanged(ref _layout, value);
		}

		public string CurrentView
		{
			get => _currentView;
			set => this.RaiseAndSetIfChanged(ref _currentView, value);
		}

		public ToolBarViewModel ToolBar { get; }
	}
}
