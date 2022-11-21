using Mostrow.NESGameDatabase;
using System.Windows;

namespace NES20HeaderRepair
{
    /// <summary>
    /// Interaction logic for ROMDatabaseEntry.xaml
    /// </summary>
    public partial class ROMDatabaseWindow : Window
    {
        public ROMDatabaseWindow(Game game)
        {
            InitializeComponent();
            DataContext = game;
            btnClose.Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}