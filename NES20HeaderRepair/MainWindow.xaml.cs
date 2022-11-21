using Mostrow.NESGameDatabase;
using NES20HeaderRepair.NESHeaderRepair.Helpers;
using NES20HeaderRepair.NESHeaderRepair.ROM;
using Ookii.Dialogs.Wpf;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NES20HeaderRepair
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Header> headers;
        private ObservableCollection<Game> games;
        private readonly Settings s;

        public MainWindow()
        {
            InitializeComponent();
            s = new Settings();
            Loaded += MainWindow_Loaded;
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.N)
            {
                new HeaderEditorWindow(s)
                {
                    Owner = this,
                }.ShowDialog();
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            headers = new ObservableCollection<Header>();
            games = new ObservableCollection<Game>();

            btnROMFolder.Click += BtnROMFolder_Click;
            btnXMLFile.Click += BtnXMLFile_Click;
            btnConvertHeaders.Click += BtnConvertHeaders_Click;
            btnSettings.Click += BtnSettings_Click;

            FileDataGrid.PreviewMouseDoubleClick += FileDataGrid_PreviewMouseDoubleClick;

            games = await LoadDatabase();
            ROMEntryDataGrid.DataContext = games;
            ROMEntryDataGrid.PreviewMouseDoubleClick += ROMEntryDataGrid_PreviewMouseDoubleClick;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow
            {
                Owner = this
            }.ShowDialog();
        }

        private async void BtnROMFolder_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog fbd = new()
            {
                Multiselect = false,
                Description = "Pick your ROM folder",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            bool? result = fbd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                headers = await LoadHeaders(fbd.SelectedPath);
                FileDataGrid.DataContext = null;
                FileDataGrid.DataContext = headers;
                CheckConvertButton();
            }
        }

        private async void BtnXMLFile_Click(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog fd = new()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "XML Files|*.xml"
            };

            bool? result = fd.ShowDialog(this);

            if (result.HasValue && result.Value)
            {
                games = await LoadDatabase(fd.FileName);
                ROMEntryDataGrid.DataContext = null;
                ROMEntryDataGrid.DataContext = games;
                CheckConvertButton();
            }
        }

        private void BtnConvertHeaders_Click(object sender, RoutedEventArgs e)
        {
            new ConverterWindow(s)
            {
                Owner = this,
                Headers = headers,
                Games = games,
            }.ShowDialog();
        }

        private void CheckConvertButton()
        {
            btnConvertHeaders.IsEnabled = headers.Count > 0 && games.Count > 0;
        }

        private void FileDataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Header h = (Header)FileDataGrid.SelectedItem;
            if (h != null)
            {
                new ROMInformationWindow(h)
                {
                    Owner = this
                }.ShowDialog();
            }
        }

        private void ROMEntryDataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Game g = (Game)ROMEntryDataGrid.SelectedItem;
            if (g != null)
            {
                new ROMDatabaseWindow(g)
                {
                    Owner = this
                }.ShowDialog();
            }
        }

        private Task<ObservableCollection<Header>> LoadHeaders(string FileName)
        {
            return Task.Run(() =>
            {
                ObservableCollection<Header> th = new();

                foreach (string f in new Loader(FileName).FileNames)
                {
                    th.Add(new Header(f));
                }

                return th;
            });
        }

        private Task<ObservableCollection<Game>> LoadDatabase(string FileName = null)
        {
            return Task.Run(() =>
            {
                DatabaseLoader dbl = new(FileName);
                ObservableCollection<Game> gs = new();

                if (dbl.GameDatabase != null)
                {
                    foreach (Game g in dbl.GameDatabase.Games)
                    {
                        gs.Add(g);
                    }
                }

                return gs;
            });
        }
    }
}