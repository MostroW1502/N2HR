using Microsoft.WindowsAPICodePack.Dialogs;
using NES20HeaderRepair.NESHeaderRepair.Helpers;
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
            btnClose.Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnPickWorkingFolder_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog cofd = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                if (cofd.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    s.WorkingFolder = cofd.FileName;
                }
            }
        }

        private void BtnPickDefectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog cofd = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                if (cofd.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    s.DefectFolder = cofd.FileName;
                }
            }
        }
    }
}