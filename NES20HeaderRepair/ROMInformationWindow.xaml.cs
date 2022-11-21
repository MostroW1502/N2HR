using NES20HeaderRepair.NESHeaderRepair.FlagClasses;
using NES20HeaderRepair.NESHeaderRepair.ROM;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace NES20HeaderRepair
{
    /// <summary>
    /// Interaction logic for ROMInfo.xaml
    /// </summary>
    public partial class ROMInformationWindow : Window
    {
        readonly Header h;
        private readonly bool forceclose = false;

        public ROMInformationWindow(Header Header)
        {
            InitializeComponent();
            h = Header;
            Loaded += ROMInfo_Loaded;
            if (h.FileSizeMatchesHeader)
            {
                h.LoadROMData();
                DataContext = h;
                btnClose.Click += BtnClose_Click;

                cboMirroringType.ItemsSource = new MirroringType().Mirroring;
                cboConsoleType.ItemsSource = new ConsoleType().Consoles;
                cboCPUPPUTiming.ItemsSource = new CPUPPUTiming().Timing;
                cboDefaultExpansionDevice.ItemsSource = new DefaultExpansionDevice().DefaultExpansion;

                cboConsoleType.SelectionChanged += CboConsoleType_SelectionChanged;
            }
            else
            {
                MessageBox.Show(
                    $"{h.FriendlyFileName} header did not match file content.\n" +
                    $"Aborting operation.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error
                );
                forceclose = true;
            }
        }

        private void ROMInfo_Loaded(object sender, RoutedEventArgs e)
        {
            if (forceclose) DialogResult = true;
            else Title = h.FriendlyFileName;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CboConsoleType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (h.ConsoleType)
            {
                case 0x01:
                    BindingOperations.SetBinding(cboVsPPUType, Selector.SelectedValueProperty, new Binding("VsPPUType"));
                    BindingOperations.SetBinding(cboVsHardwareType, Selector.SelectedValueProperty, new Binding("VsHardwareType"));
                    BindingOperations.ClearBinding(cboExtendedConsoleType, Selector.SelectedValueProperty);
                    break;
                case 0x03:
                    BindingOperations.ClearBinding(cboVsPPUType, Selector.SelectedValueProperty);
                    BindingOperations.ClearBinding(cboVsHardwareType, Selector.SelectedValueProperty);
                    BindingOperations.SetBinding(cboExtendedConsoleType, Selector.SelectedValueProperty, new Binding("ExtendedConsoleType"));
                    break;
                default:
                    BindingOperations.ClearBinding(cboVsPPUType, Selector.SelectedValueProperty);
                    BindingOperations.ClearBinding(cboVsHardwareType, Selector.SelectedValueProperty);
                    BindingOperations.ClearBinding(cboExtendedConsoleType, Selector.SelectedValueProperty);
                    h.ExtendedConsoleType = 0;
                    break;
            }

            cboVsPPUType.ItemsSource = h.ConsoleType == 0x1 ? new VsPPUType().PPU : null;
            cboVsPPUType.IsEnabled = h.ConsoleType == 0x1;
            cboVsPPUType.SelectedIndex = cboVsPPUType.IsEnabled ? 0 : -1;

            cboVsHardwareType.ItemsSource = h.ConsoleType == 0x1 ? new VsHardwareType().Hardware : null;
            cboVsHardwareType.IsEnabled = h.ConsoleType == 0x1;
            cboVsHardwareType.SelectedIndex = cboVsHardwareType.IsEnabled ? 0 : -1;

            cboExtendedConsoleType.ItemsSource = h.ConsoleType == 0x3 ? new ExtendedConsoleType().Types : null;
            cboExtendedConsoleType.IsEnabled = h.ConsoleType == 0x3;
            cboExtendedConsoleType.SelectedIndex = cboExtendedConsoleType.IsEnabled ? 0 : -1;
        }
    }
}