using NES20HeaderRepair.NESHeaderRepair.Helpers;
using Ookii.Dialogs.Wpf;
using System.Windows;

namespace NES20HeaderRepair
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Settings s;

        public SettingsWindow()
        {
            InitializeComponent();
            s = new Settings();
            DataContext = s;
            Loaded += SettingsWindow_Loaded;
            Closing += SettingsWindow_Closing;
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            s.Save();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnPickWorkingFolder.Click += BtnPickWorkingFolder_Click;
            btnPickDefectFolder.Click += BtnPickDefectFolder_Click;
            btnPickUnverifiedFolder.Click += BtnPickUnverifiedFolder_Click;
            btnClose.Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnPickWorkingFolder_Click(object sender, RoutedEventArgs e)
        {
            WritePathToProperty("Pick working folder", out string Property);
            s.WorkingFolder = Property;
        }

        private void BtnPickDefectFolder_Click(object sender, RoutedEventArgs e)
        {
            WritePathToProperty("Pick defect folder", out string Property);
            s.DefectFolder = Property;
        }

        private void BtnPickUnverifiedFolder_Click(object sender, RoutedEventArgs e)
        {
            WritePathToProperty("Pick unverified folder", out string Property);
            s.UnverifiedFolder = Property;
        }

        private void WritePathToProperty(string Description, out string Property)
        {
            VistaFolderBrowserDialog fbd = new()
            {
                Multiselect = false,
                Description = Description,
                ShowNewFolderButton = false,
                UseDescriptionForTitle = true
            };

            bool? result = fbd.ShowDialog(this);

            if (result.HasValue && result.Value)
            {
                Property = fbd.SelectedPath;
                return;
            }

            Property = string.Empty;
        }
    }
}