using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaDockApplication.Views.Tools
{
    public class TreeViewToolView : UserControl
    {
        public TreeViewToolView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
