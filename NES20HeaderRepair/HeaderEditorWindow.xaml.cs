using NES20HeaderRepair.NESHeaderRepair;
using NES20HeaderRepair.NESHeaderRepair.FlagClasses;
using NES20HeaderRepair.NESHeaderRepair.Helpers;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace NES20HeaderRepair
{
    /// <summary>
    /// Interaction logic for Headitor.xaml
    /// </summary>
    public partial class HeaderEditorWindow : Window
    {
        private NES2Header n2h;
        private string FileName;

        // OnEnter event toevoegen en ctrl+a en pijltjes links / rechts toevoegen aan events.

        public HeaderEditorWindow(Settings Settings)
        {
            InitializeComponent();
            n2h = new NES2Header(Settings);
            DataContext = n2h;
            cboMirroringType.ItemsSource = new MirroringType().Mirroring;
            cboConsoleType.ItemsSource = new ConsoleType().Consoles;
            cboCPUPPUTiming.ItemsSource = new CPUPPUTiming().Timing;
            cboDefaultExpansionDevice.ItemsSource = new DefaultExpansionDevice().DefaultExpansion;

            cboConsoleType.SelectionChanged += CboConsoleType_SelectionChanged;
            txtPRGROMSize.PreviewKeyDown += TxtPRGROM_PreviewKeyDown;
            txtPRGROMBankCount.PreviewKeyDown += TxtPRGROM_PreviewKeyDown;
            txtCHRROMSize.PreviewKeyDown += TxtCHRROM_PreviewKeyDown;
            txtCHRROMBankCount.PreviewKeyDown += TxtCHRROM_PreviewKeyDown;
            txtPRGRAMSize.PreviewKeyDown += TxtPRGRAM_PreviewKeyDown;
            txtPRGRAMShiftCount.PreviewKeyDown += TxtPRGRAM_PreviewKeyDown;
            txtPRGNVRAMSize.PreviewKeyDown += TxtPRGNVRAM_PreviewKeyDown;
            txtPRGNVRAMShiftCount.PreviewKeyDown += TxtPRGNVRAM_PreviewKeyDown;
            txtCHRRAMSize.PreviewKeyDown += TxtCHRRAM_PreviewKeyDown;
            txtCHRRAMShiftCount.PreviewKeyDown += TxtCHRRAM_PreviewKeyDown;
            txtCHRNVRAMSize.PreviewKeyDown += TxtCHRNVRAM_PreviewKeyDown;
            txtCHRNVRAMShiftCount.PreviewKeyDown += TxtCHRNVRAM_PreviewKeyDown;
            txtMapper.PreviewKeyDown += TxtMappers_PreviewKeyDown;
            txtSubMapper.PreviewKeyDown += TxtMappers_PreviewKeyDown;

            btnSelectROM.Click += BtnSelectROM_Click;
            btnWriteHeader.Click += btnWriteHeader_Click;
            btnClose.Click += btnClose_Click;

            Loaded += Headitor_Loaded;
        }

        private void CboConsoleType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (n2h.ConsoleType)
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
                    n2h.ExtendedConsoleType = 0;
                    break;
            }

            cboVsPPUType.ItemsSource = n2h.ConsoleType == 0x1 ? new VsPPUType().PPU : null;
            cboVsPPUType.IsEnabled = n2h.ConsoleType == 0x1;
            cboVsPPUType.SelectedIndex = cboVsPPUType.IsEnabled ? 0 : -1;

            cboVsHardwareType.ItemsSource = n2h.ConsoleType == 0x1 ? new VsHardwareType().Hardware : null;
            cboVsHardwareType.IsEnabled = n2h.ConsoleType == 0x1;
            cboVsHardwareType.SelectedIndex = cboVsHardwareType.IsEnabled ? 0 : -1;

            cboExtendedConsoleType.ItemsSource = n2h.ConsoleType == 0x3 ? new ExtendedConsoleType().Types : null;
            cboExtendedConsoleType.IsEnabled = n2h.ConsoleType == 0x3;
            cboExtendedConsoleType.SelectedIndex = cboExtendedConsoleType.IsEnabled ? 0 : -1;
        }

        private void Headitor_Loaded(object sender, RoutedEventArgs e)
        {
            cboMirroringType.SelectedIndex = 0;
            cboConsoleType.SelectedIndex = 0;
            cboCPUPPUTiming.SelectedIndex = 0;
        }

        private void TxtPRGROM_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) n2h.PRGROMBankCount++;
            else if (e.Key == Key.Down) n2h.PRGROMBankCount--;

            e.Handled = !IsValidKey(e);
        }

        private void TxtCHRROM_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) n2h.CHRROMBankCount++;
            else if (e.Key == Key.Down) n2h.CHRROMBankCount--;

            e.Handled = !IsValidKey(e);
        }

        private void TxtPRGRAM_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) n2h.PRGRAMShiftCount++;
            if (e.Key == Key.Down) n2h.PRGRAMShiftCount--;

            e.Handled = !IsValidKey(e);
        }

        private void TxtPRGNVRAM_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) n2h.PRGNVRAMShiftCount++;
            if (e.Key == Key.Down) n2h.PRGNVRAMShiftCount--;

            e.Handled = !IsValidKey(e);
        }

        private void TxtCHRRAM_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) n2h.CHRRAMShiftCount++;
            if (e.Key == Key.Down) n2h.CHRRAMShiftCount--;

            e.Handled = !IsValidKey(e);
        }

        private void TxtCHRNVRAM_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) n2h.CHRNVRAMShiftCount++;
            if (e.Key == Key.Down) n2h.CHRNVRAMShiftCount--;

            e.Handled = !IsValidKey(e);
        }

        private void TxtMappers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !IsValidKey(e);
        }

        private bool IsValidKey(KeyEventArgs e)
        {
            return ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                    (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                    (e.Key == Key.Back) || (e.Key == Key.Tab) ||
                    (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.Tab));
        }

        private void BtnSelectROM_Click(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog ofd = new()
            {
                CheckFileExists = true,
                CheckPathExists = true
            };

            bool? result = ofd.ShowDialog(this);

            if (result.HasValue && result.Value)
            {
                FileName = ofd.FileName;
                btnWriteHeader.IsEnabled = true;
                return;
            }

            FileName = string.Empty;
            btnWriteHeader.IsEnabled = false;
        }

        private async void btnWriteHeader_Click(object sender, RoutedEventArgs e)
        {
            btnWriteHeader.IsEnabled = !await n2h.WriteHeaderAsync(FileName);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}