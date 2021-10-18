using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaDockApplication.Models;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;

namespace AvaloniaDockApplication.ViewModels.Tools
{
    public class TreeViewToolViewModel : Tool
    {
        private ObservableCollection<ITreeListNode> items;

        private ObservableCollection<ITreeListNode> Items
        {
            get => items;
            set => this.RaiseAndSetIfChanged(ref items, value);
        }

        public TreeViewToolViewModel()
        {
            var root = new TreeListBranch()
                { Caption = "Root", Icon = IconService.Instance.GetCompletionKindImage("FolderIcon") };
            root.Items.Add(new TreeListLeaf(){Caption = "SubLeaf", Icon = IconService.Instance.GetCompletionKindImage("Save")});
            var nodes = new List<ITreeListNode>()
            {
                root,
                new TreeListLeaf()
                { Caption = "Leaf", Icon = IconService.Instance.GetCompletionKindImage("SaveAll") }
            };
            items = new ObservableCollection<ITreeListNode>(nodes);
        }
    }
}
