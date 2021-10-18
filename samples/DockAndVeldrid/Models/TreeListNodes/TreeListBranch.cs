using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Media;
using ReactiveUI;

namespace AvaloniaDockApplication.Models
{
	public class TreeListBranch:ITreeListNode
	{
		public string Caption { get; set; }
		public DrawingGroup Icon { get; set; }
		public ObservableCollection<ITreeListNode> Items { get; }

		public TreeListBranch()
		{
			Items = new ObservableCollection<ITreeListNode>();
			RemoveCommand = ReactiveCommand.Create(() =>
			{
				throw new NotImplementedException();
			});
		}
		
		public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
	}
}