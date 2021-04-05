using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using b7.Packets.ViewModel;

namespace b7.Packets.View
{
    public partial class LogView : UserControl
    {
        protected LogViewManager Manager => (LogViewManager)DataContext;

        public LogView()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListView listView) return;

            Manager.UpdateSelection(listView.SelectedItems);
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element ||
                element.DataContext is not PacketLogViewModel)
            {
                return;
            }

            Manager.LoadInStructuralizerCommand.Execute(null);
        }
    }
}
