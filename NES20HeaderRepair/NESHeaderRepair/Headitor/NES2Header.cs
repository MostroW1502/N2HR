using Mostrow.NESGameDatabase;
using NES20HeaderRepair.NESHeaderRepair.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NES20HeaderRepair.NESHeaderRepair
{
    public class NES2Header : INotifyPropertyChanged
    {
        protected byte[] header;
        private readonly Settings s;

        public NES2Header(Settings Settings)
        {
            s = Settings;
            PrepareBaseHeader();
        }

        public NES2Header(Game Game, Settings Settings)
        {
            s = Settings;
            PrepareBaseHeader();
            PRGROMSize = Game.PRGROM?.Size ?? 0;
            PRGRAMSize = Game.PRGRAM?.Size ?? 0;
            PRGNVRAMSize = Game.PRGNVRAM?.Size ?? 0;
            CHRROMSize = Game.CHRROM?.Size ?? 0;
            CHRRAMSize = Game.CHRRAM?.Size ?? 0;
            CHRNVRAMSize = Game.CHRNVRAM?.Size ?? 0;
            Mapper = Game.PCB?.Mapper ?? 0;
            SubMapper = Game.PCB?.SubMapper ?? 0;
            int m = 0,
                f = 0;
            switch (Game.PCB.Mirroring)
            {
                case "0":
                    // F..M
                    if (Mapper == 218)
                    {
                        // 1..0
                        f = 1;
                        m = 0;
                    }
                    break;
                case "1":
                    if (Mapper == 30)
                    {
                        // 1..0
                        f = 1;
                        m = 0;
                    }
                    else if (Mapper == 218)
                    {
                        // 1..1
                        f = 1;
                        m = 1;
                    }
                    break;
                case "4":
                    if (Mapper == 30)
                    {
                        // 1..1
                        f = 1;
                        m = 1;
                    }
                    else
                    {
                        // 1..0
                        f = 1;
                        m = 0;
                    }
                    break;
                case "H":
                case "h":
                    // 0..0 (all mappers)
                    f = 0;
                    m = 0;
                    break;
                case "V":
                case "v":
                    // 0..1 (all mappers)
                    f = 0;
                    m = 1;
                    break;
                default: break;
            }
            SetMBTF(
                m, 
                Game.PCB?.Battery ?? 0, 
                (Game.TRAINER?.Size ?? 0) > 0 ? 1 : 0, 
                f
            );
            ConsoleType = Game.Console?.Type ?? 0;
            CPUPPUTiming = Game.Console?.Region ?? 0;
            VsPPUType = Game.VS?.PPU ?? 0;
            VsHardwareType = Game.VS?.Hardware ?? 0;
            DefaultExpansionDevice = Game.Expansion?.Type ?? 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PrepareBaseHeader()
        {
            header = new byte[]
            {
                0x4E, 0x45, 0x53, 0x1A,
                0x00, 0x00, 0x00, 0x08,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };
        }

        public string HeaderBytesAsString
        {
            get
            {
                return header.ToHeaderString();
            }
        }

        public string IdentificationString
        {
            get
            {
                return $"{(char)header[0]}{(char)header[1]}{(char)header[2]}" +
                          (header[3] == 0x1A ? "<EOF>" : ((char)header[3]));
            }
        }

        // PRG ROM Part

        public virtual int PRGROMSize
        {
            get
            {
                if (header[9] == 0xF)
                {
                    int m = (header[4] & 0x3) * 2 + 1;
                    int e = 2 ^ (header[4] >> 2);
                    return e * m;
                }
                else return PRGROMBankCount * 0x4000;
            }
            set
            {
                PRGROMBankCount = value / 0x4000;
            }
        }

        public virtual int PRGROMBankCount
        {
            get
            {
                return (((header[9] & 0xF) << 8) | header[4]);
            }
            set
            {
                if (value < 0x00) value = 0xEFF;
                else if (value > 0xEFF) value = 0x00;

                SetPRGBytes(value);
                OnPropertyChanged(nameof(PRGROMSize));
                OnPropertyChanged(nameof(PRGROMBankCount));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        private void SetPRGBytes(int v)
        {
            header[4] = (byte)(v & 0xFF);
            header[9] = (byte)((header[9] & 0xF0) | ((v & 0xF00) >> 8));
        }

        // CHR ROM Part

        public virtual int CHRROMSize
        {
            get
            {
                if (header[9] == 0xF0)
                {
                    int m = (header[5] & 0x3) * 2 + 1;
                    int e = (header[5] >> 2);
                    return e * m;
                }
                return CHRROMBankCount * 0x2000;
            }
            set
            {
                CHRROMBankCount = value / 0x2000;
            }
        }

        public virtual int CHRROMBankCount
        {
            get
            {
                return (((header[9] & 0xF0) << 4) | header[5]);
            }
            set
            {
                if (value < 0x00) value = 0xEFF;
                else if (value > 0xEFF) value = 0x00;

                SetCHRBytes(value);
                OnPropertyChanged(nameof(CHRROMSize));
                OnPropertyChanged(nameof(CHRROMBankCount));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        private void SetCHRBytes(int v)
        {
            header[5] = (byte)(v & 0xFF);
            header[9] = (byte)(((v & 0xF00) >> 4) | (header[9] & 0xF));
        }

        public virtual int PRGRAMSize
        {
            get
            {
                return PRGRAMShiftCount > 0 ? 64 << PRGRAMShiftCount : 0;
            }
            set
            {
                PRGRAMShiftCount = ReduceToNybble(value);
            }
        }

        public virtual int PRGRAMShiftCount
        {
            get
            {
                return (header[10] & 0xF);
            }
            set
            {
                if (value > 0xF) value = 0x0;
                if (value < 0x0) value = 0xF;

                header[10] = (byte)((header[10] & 0xF0) | value);
                OnPropertyChanged(nameof(PRGRAMSize));
                OnPropertyChanged(nameof(PRGRAMShiftCount));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int PRGNVRAMSize
        {
            get
            {
                return PRGNVRAMShiftCount > 0 ? 64 << PRGNVRAMShiftCount : 0;
            }
            set
            {
                PRGNVRAMShiftCount = ReduceToNybble(value);
            }
        }

        public virtual int PRGNVRAMShiftCount
        {
            get
            {
                return (header[10] & 0xF0) >> 4;
            }
            set
            {
                if (value > 0xF) value = 0x0;
                if (value < 0x0) value = 0xF;

                header[10] = (byte)((header[10] & 0xF) | (value << 4));
                OnPropertyChanged(nameof(PRGNVRAMSize));
                OnPropertyChanged(nameof(PRGNVRAMShiftCount));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int CHRRAMSize
        {
            get
            {
                return CHRRAMShiftCount > 0 ? 64 << CHRRAMShiftCount : 0;
            }
            set
            {
                CHRRAMShiftCount = ReduceToNybble(value);
            }
        }

        public virtual int CHRRAMShiftCount
        {
            get
            {
                return (header[11] & 0xF);
            }
            set
            {
                if (value > 0xF) value = 0x0;
                if (value < 0x0) value = 0xF;

                header[11] = (byte)((header[11] & 0xF0) | value);
                OnPropertyChanged(nameof(CHRRAMSize));
                OnPropertyChanged(nameof(CHRRAMShiftCount));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int CHRNVRAMSize
        {
            get
            {
                return CHRNVRAMShiftCount > 0 ? 64 << CHRNVRAMShiftCount : 0;
            }
            set
            {
                CHRNVRAMShiftCount = ReduceToNybble(value);
            }
        }

        public virtual int CHRNVRAMShiftCount
        {
            get
            {
                return (header[11] & 0xF0) >> 4;
            }
            set
            {
                if (value > 0xF) value = 0x0;
                if (value < 0x0) value = 0xF;

                header[11] = (byte)((header[11] & 0xF) | (value << 4));
                OnPropertyChanged(nameof(CHRNVRAMSize));
                OnPropertyChanged(nameof(CHRNVRAMShiftCount));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        private int ReduceToNybble(int Value)
        {
            int bitvalue = 0;
            while (Value > 64)
            {
                Value /= 2;
                bitvalue++;
            }
            return bitvalue & 0xF;
        }

        private void SetMBTF(int M, int B, int T, int F)
        {
            header[6] = (byte)((header[6] & 0xF0) |
                        (byte)((M > 0 ? 1 : 0) +
                               (B > 0 ? 2 : 0) +
                               (T > 0 ? 4 : 0) +
                               (F > 0 ? 8 : 0)));
        }

        public int Mirroring
        {
            get
            {
                return header[6] & 0x1;
            }
            set
            {
                header[6] ^= 0x1;
                OnPropertyChanged(nameof(Mirroring));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public bool Battery
        {
            get
            {
                return (header[6] & 0x2) == 0x2;
            }
            set
            {
                header[6] ^= 0x2;
                OnPropertyChanged(nameof(Battery));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public bool Trainer
        {
            get
            {
                return (header[6] & 0x4) == 0x4;
            }
            set
            {
                header[6] ^= 0x4;
                OnPropertyChanged(nameof(Trainer));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public bool FourScreen
        {
            get
            {
                return (header[6] & 0x8) == 0x8;
            }
            set
            {
                header[6] ^= 0x8;
                OnPropertyChanged(nameof(FourScreen));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int Mapper
        {
            get
            {
                return ((header[8] & 0xF) << 8) | (header[7] & 0xF0) | ((header[6] & 0xF0) >> 4);
            }
            set
            {
                header[8] = (byte)((header[8] & 0xF0) | ((value & 0xF00) >> 8));
                header[7] = (byte)((header[7] & 0xF) | (value & 0xF0));
                header[6] = (byte)((header[6] & 0xF) | ((value & 0xF)) << 4);
                OnPropertyChanged(nameof(Mapper));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int SubMapper
        {
            get
            {
                return ((header[8] & 0xF0) >> 4);
            }
            set
            {
                if (value < 0) value = 0;
                if (value > 15) value = 15;

                header[8] = (byte)((header[8] & 0xF) | ((value & 0xF) << 4));
                OnPropertyChanged(nameof(SubMapper));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int ConsoleType
        {
            get
            {
                return (header[7] & 0x3);
            }
            set
            {
                header[7] = (byte)((header[7] & 0xFC) | (value & 0x3));
                OnPropertyChanged(nameof(ConsoleType));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int CPUPPUTiming
        {
            get
            {
                return header[12];
            }
            set
            {
                header[12] = (byte)(value & 0x3);
                OnPropertyChanged(nameof(CPUPPUTiming));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int VsPPUType
        {
            get
            {
                return (header[13] & 0xF);
            }
            set
            {
                header[13] = (byte)((header[13] & 0xF0) | (value & 0xF));
                OnPropertyChanged(nameof(VsPPUType));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int VsHardwareType
        {
            get
            {
                return (header[13] & 0xF0) >> 4;
            }
            set
            {
                header[13] = (byte)((header[13] & 0xF) | ((value & 0xF) << 4));
                OnPropertyChanged(nameof(VsHardwareType));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int ExtendedConsoleType
        {
            get
            {
                return (header[13] & 0xF);
            }
            set
            {
                header[13] = (byte)((0x0) | (value & 0xFF));
                OnPropertyChanged(nameof(ExtendedConsoleType));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public virtual int DefaultExpansionDevice
        {
            get
            {
                return (header[15] & 0x3F);
            }
            set
            {
                header[15] = (byte)(value & 0x3F);
                OnPropertyChanged(nameof(DefaultExpansionDevice));
                OnPropertyChanged(nameof(HeaderBytesAsString));
            }
        }

        public Task<bool> WriteHeaderAsync(string FileName)
        {
            return Task.Run(() =>
            {
                try
                {
                    byte[] rom = File.ReadAllBytes(FileName);

                    for (int i = 0; i < header.Length; i++)
                    {
                        rom[i] = header[i];
                    }

                    using (FileStream fs = new FileStream(WorkingFile(FileName), FileMode.Create))
                    {
                        fs.Write(rom, 0, rom.Length);
                    }

                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        public Task CopyToWorkingFolder(string FileName)
        {
            return Task.Run(() =>
            {
                if (File.Exists(FileName)) File.Copy(FileName, WorkingFile(FileName), true);
            });
        }

        public Task CopyToDefectFolder(string FileName)
        {
            return Task.Run(() =>
            {
                if (File.Exists(FileName)) File.Copy(FileName, DefectFile(FileName), true);
            });
        }

        public Task CopyToUnverifiedFolder(string FileName)
        {
            return Task.Run(() =>
            {
                if (File.Exists(FileName)) File.Copy(FileName, UnverifiedFile(FileName), true);
            });
        }

        public Task<bool> LoadHeaderAsync(string FileName)
        {
            return Task.Run(() =>
            {
                try
                {
                    new FileStream(FileName, FileMode.Open).Read(header, 0, 16);
                    return true;
                }
                catch(Exception)
                {
                    throw;
                }
            });
        }

        private string WorkingFile(string FileName)
        {
            if (s.PreserveFolderStructure)
            {
                string newdir = $@"{s.WorkingFolder}\{new DirectoryInfo(Path.GetDirectoryName(FileName)).Name}";

                if (!Directory.Exists(newdir)) Directory.CreateDirectory(newdir);
                return $@"{newdir}\{Path.GetFileName(FileName)}";
            }
            else return $@"{s.WorkingFolder}\{Path.GetFileName(FileName)}";
        }

        private string DefectFile(string FileName)
        {
            if (s.PreserveFolderStructure)
            {
                string newdir = $@"{s.DefectFolder}\{new DirectoryInfo(Path.GetDirectoryName(FileName)).Name}";

                if (!Directory.Exists(newdir)) Directory.CreateDirectory(newdir);
                return $@"{newdir}\{Path.GetFileName(FileName)}";
            }
            else return $@"{s.DefectFolder}\{Path.GetFileName(FileName)}";
        }

        private string UnverifiedFile(string FileName)
        {
            if (s.PreserveFolderStructure)
            {
                string newdir = $@"{s.UnverifiedFolder}\{new DirectoryInfo(Path.GetDirectoryName(FileName)).Name}";

                if (!Directory.Exists(newdir)) Directory.CreateDirectory(newdir);
                return $@"{newdir}\{Path.GetFileName(FileName)}";
            }
            else return $@"{s.UnverifiedFolder}\{Path.GetFileName(FileName)}";
        }

        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}