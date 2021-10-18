using System.Collections.Immutable;
using AvaloniaDockApplication.Models.Menus;

namespace AvaloniaDockApplication.ViewModels
{
	public class ToolBarViewModel : MenuViewModel
	{
		public ToolBarViewModel(IImmutableList<MenuItemModel> items) 
			: base(items)
		{
		}
	}
}