using System.Windows.Input;
using Avalonia.Media;

namespace AvaloniaDockApplication
{
	public class CommonMenuItem : IMenuItem
	{
		public string Label { get; }
		public DrawingGroup Icon { get; }
		public ICommand Command { get; }
		public string Gesture { get; }

		public CommonMenuItem(string label, DrawingGroup icon)
		{
			Label = label;
			Icon = icon;
		}
	}
}