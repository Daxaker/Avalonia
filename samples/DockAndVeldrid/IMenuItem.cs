using System.Windows.Input;
using Avalonia.Media;

namespace AvaloniaDockApplication
{
    public interface IMenuItem
    {
        string Label { get; }

        DrawingGroup Icon { get; }

        ICommand Command { get; }

        string Gesture { get; }
    }
}