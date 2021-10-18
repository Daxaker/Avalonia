using Avalonia.Media;

namespace AvaloniaDockApplication.Models
{
	public interface ITreeListNode
	{
		string Caption { get; set; }
		DrawingGroup Icon { get; set; }
	}
}