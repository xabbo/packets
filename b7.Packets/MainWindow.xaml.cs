using System;
using System.Threading.Tasks;
using System.Windows;

using b7.Packets.ViewModel;

namespace b7.Packets
{
    public partial class MainWindow : Window
    {
        private readonly MainViewManager _mainViewManager;

        public MainWindow()
        {
            _mainViewManager = null!;

            InitializeComponent();
        }

        public MainWindow(MainViewManager mainViewManager)
        {
            _mainViewManager = mainViewManager;
            _mainViewManager.OpenStructureView += (s, e) => tabControl.SelectedItem = structureTab;

            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            Task.Run(() => _mainViewManager.InitializeAsync());
        }
    }
}
