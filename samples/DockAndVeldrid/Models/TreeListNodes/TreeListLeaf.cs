using Avalonia.Media;

namespace AvaloniaDockApplication.Models
{
	public class TreeListLeaf : ITreeListNode
	{
		public string Caption { get; set; }
		public DrawingGroup Icon { get; set; }
	}
}