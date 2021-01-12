using System;
using System.Windows;
using System.Windows.Controls;

using b7.Packets.ViewModel;

namespace b7.Packets.View
{
    public partial class StructureView : UserControl
    {
        public StructureView()
        {
            InitializeComponent();
        }

        private void TextBlock_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (sender is not TextBlock textBlock) return;
            if (textBlock.GetValue(ToolTipService.ToolTipProperty) is not FrameworkElement toolTip) return;
            if (textBlock.DataContext is not ByteViewModel byteViewModel) return;

            ByteInfoViewModel byteInfo = new ByteInfoViewModel();

            // TODO Populate view-model with values of each structure type at the specified offset

            toolTip.DataContext = byteInfo;
        }
    }
}
