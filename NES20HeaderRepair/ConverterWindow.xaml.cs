using Mostrow.NESGameDatabase;
using NES20HeaderRepair.NESHeaderRepair;
using NES20HeaderRepair.NESHeaderRepair.Helpers;
using NES20HeaderRepair.NESHeaderRepair.ROM;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace NES20HeaderRepair
{
    /// <summary>
    /// Interaction logic for ConverterWindow.xaml
    /// </summary>
    public partial class ConverterWindow : Window
    {
        private Progress<Comparer> progress;
        private readonly Settings s;

        public ConverterWindow(Settings Settings)
        {
            InitializeComponent();
            s = Settings;
            Loaded += ConverterWindow_Loaded;
        }

        private async void ConverterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnClose.Click += BtnClose_Click;
            progress = new Progress<Comparer>(ReportProgress);
            await Task.Run(() =>
            {
                Compare(progress);
            });
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public Collection<Header> Headers { get; set; }

        public Collection<Game> Games { get; set; }

        private void ReportProgress(Comparer c)
        {
            btnClose.IsEnabled = !c.Running;
            if (c.Header == null && c.Game == null) return;

            lblFileName.Content = $"{c.Header.FriendlyFileName}";

            lblHeaderPRGSize.Content = $"{c.Header.PRGROMSize}";
            lblHeaderPRGCRC32.Content = $"{c.Header.PRGROMCRC32AsString}";
            lblHeaderPRGSHA1.Content = $"{c.Header.PRGROMSHA1AsString}";
            lblHeaderPRGSUM16.Content = $"{c.Header.PRGROMSUM16AsString}";

            lblHeaderCHRSize.Content = $"{c.Header.CHRROMSize}";
            lblHeaderCHRCRC32.Content = $"{c.Header.CHRROMCRC32AsString}";
            lblHeaderCHRSHA1.Content = $"{c.Header.CHRROMSHA1AsString}";
            lblHeaderCHRSUM16.Content = $"{c.Header.CHRROMSUM16AsString}";

            if (c.Game != null)
            {
                lblDatabasePRGSize.Content = $"{c.Game.PRGROM.Size}";
                lblDatabasePRGCRC32.Content = $"{c.Game.PRGROM.CRC32AsString}";
                lblDatabasePRGSHA1.Content = $"{c.Game.PRGROM.SHA1AsString}";
                lblDatabasePRGSUM16.Content = $"{c.Game.PRGROM.SUM16AsString}";

                if (c.Game.CHRROM != null)
                {
                    lblDatabaseCHRSize.Content = $"{c.Game.CHRROM.Size}";
                    lblDatabaseCHRCRC32.Content = $"{c.Game.CHRROM.CRC32AsString}";
                    lblDatabaseCHRSHA1.Content = $"{c.Game.CHRROM.SHA1AsString}";
                    lblDatabaseCHRSUM16.Content = $"{c.Game.CHRROM.SUM16AsString}";
                }
                else
                {
                    lblDatabaseCHRSize.Content = "";
                    lblDatabaseCHRCRC32.Content = "";
                    lblDatabaseCHRSHA1.Content = "";
                    lblDatabaseCHRSUM16.Content = "";
                }
            }
            else
            {
                lblDatabasePRGSize.Content = "";
                lblDatabasePRGCRC32.Content = "";
                lblDatabasePRGSHA1.Content = "";
                lblDatabasePRGSUM16.Content = "";

                lblDatabaseCHRSize.Content = "";
                lblDatabaseCHRCRC32.Content = "";
                lblDatabaseCHRSHA1.Content = "";
                lblDatabaseCHRSUM16.Content = "";
            }
        }

        private async void Compare(IProgress<Comparer> progress)
        {
            for (int h = 0; h < Headers.Count; h++)
            {
                if (!Headers[h].FileSizeMatchesHeader)
                    await new NES2Header(s).CopyToDefectFolder(Headers[h].FileName);
                else
                {
                    Headers[h].LoadROMData();
                    progress?.Report(new Comparer { Running = true, Header = Headers[h], Game = null });

                    for (int g = 0; g < Games.Count; g++)
                    {
                        bool a, b;

                        a = (Headers[h].ROMCRC32 == Games[g].ROM.CRC32 &&
                             Headers[h].ROMSHA1AsString == Games[g].ROM.SHA1AsString &&
                             Headers[h].PRGROMSize == Games[g].PRGROM.Size &&
                             Headers[h].PRGROMCRC32 == Games[g].PRGROM.CRC32 &&
                             Headers[h].PRGROMSHA1AsString == Games[g].PRGROM.SHA1AsString &&
                             Headers[h].PRGROMSUM16 == Games[g].PRGROM.SUM16);

                        b = Games[g].CHRROM == null ? true :
                            (Headers[h].CHRROMSize == Games[g].CHRROM.Size &&
                             Headers[h].CHRROMCRC32 == Games[g].CHRROM.CRC32 &&
                             Headers[h].CHRROMSHA1AsString == Games[g].CHRROM.SHA1AsString &&
                             Headers[h].CHRROMSUM16 == Games[g].CHRROM.SUM16);

                        if (g == 0 || g % 100 == 0)
                            progress?.Report(new Comparer { Running = true, Header = Headers[h], Game = Games[g] });
                        else if (g == Games.Count - 1)
                            progress?.Report(new Comparer { Running = true, Header = Headers[h], Game = Games[g] });

                        if (a && b)
                        {
                            if (Headers[h].ROMTYPE == FORMAT.NES20)
                                await new NES2Header(Games[g], s).CopyToWorkingFolder(Headers[h].FileName);
                            else
                                await new NES2Header(Games[g], s).WriteHeaderAsync(Headers[h].FileName);

                            break;
                        }

                        if (g == Games.Count - 1)
                        {
                            await new NES2Header(Games[g], s).CopyToUnverifiedFolder(Headers[h].FileName);
                        }
                    }
                }
            }

            progress?.Report(new Comparer { Running = false, Header =  null, Game = null });
        }
    }
}