using Force.Crc32;
using System;
using System.IO;
using System.Security.Cryptography;

namespace NES20HeaderRepair.NESHeaderRepair.ROM
{
    public partial class Header : NES2Header
    {
        private int headerOffset;

        public Header(string FileName) : base(null)
        {
            this.FileName = FileName;
            LoadROMHeader();
        }

        public string Error { get; private set; }

        public FORMAT ROMTYPE
        {
            get
            {
                if (!((char)header[0] == 'N' && (char)header[1] == 'E' && (char)header[2] == 'S')) return FORMAT.NO_NES_HEADER;
                int count = 0;

                if ((header[7] & 0xC) == 0x8) return FORMAT.NES20;
                else
                {
                    for (int i = 7; i <= 15; i++)
                    {
                        if (header[i] != 0) count++;
                    }
                    return count > 0 ? FORMAT.ARCHAIC : FORMAT.INES;
                }
            }
        }

        public string FileName { get; private set; }

        public string FriendlyFileName
        {
            get
            {
                return Path.GetFileName(FileName);
            }
        }

        public int FileSizeOnDisk { get; private set; }

        public bool FileSizeMatchesHeader
        {
            get
            {
                return PRGROMSize + CHRROMSize + MISCROMSize == FileSizeOnDisk - headerOffset;
            }
        }

        public int MISCROMSize
        {
            get
            {
                int result = FileSizeOnDisk - (PRGROMSize + CHRROMSize + headerOffset);
                return result < 0 ? 0 : result;
            }
        }

        public int ROMSize { get; private set; }

        public uint ROMCRC32 { get; private set; }

        public string ROMCRC32AsString
        {
            get
            {
                return $"{ROMCRC32:X8}";
            }
        }

        public byte[] ROMSHA1 { get; private set; }

        public string ROMSHA1AsString
        {
            get
            {
                return ROMSHA1.ToSha1String();
            }
        }

        public override int PRGROMSize
        {
            get
            {
                return PRGROMBankCount * 0x4000;
            }
        }

        public override int PRGROMBankCount
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.PRGROMBankCount;
                else return header[4];
            }
        }

        public uint PRGROMCRC32 { get; private set; }

        public string PRGROMCRC32AsString
        {
            get
            {
                return $"{PRGROMCRC32:X8}";
            }
        }

        public byte[] PRGROMSHA1 { get; private set; }

        public string PRGROMSHA1AsString
        {
            get
            {
                return PRGROMSHA1.ToSha1String();
            }
        }

        public ushort PRGROMSUM16 { get; private set; }

        public string PRGROMSUM16AsString
        {
            get
            {
                return $"{PRGROMSUM16:X4}";
            }
        }

        public override int CHRROMSize
        {
            get
            {
                return CHRROMBankCount * 0x2000;
            }
        }

        public override int CHRROMBankCount
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.CHRROMBankCount;
                else return header[5];
            }
        }

        public uint CHRROMCRC32 { get; private set; }

        public string CHRROMCRC32AsString
        {
            get
            {
                return CHRROMSize > 0 ? $"{CHRROMCRC32:X8}" : string.Empty;
            }
        }

        public byte[] CHRROMSHA1 { get; private set; }

        public string CHRROMSHA1AsString
        {
            get
            {
                return CHRROMSize > 0 ? CHRROMSHA1.ToSha1String() : string.Empty;
            }
        }

        public ushort CHRROMSUM16 { get; private set; }

        public string CHRROMSUM16AsString
        {
            get
            {
                return CHRROMSize > 0 ? $"{CHRROMSUM16:X4}" : string.Empty;
            }
        }

        public override int PRGRAMSize
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.PRGRAMSize;
                else return 0;
            }
        }

        public override int PRGRAMShiftCount
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.PRGRAMShiftCount;
                else return 0;
            }
        }

        public override int PRGNVRAMSize
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.PRGNVRAMSize;
                else return 0;
            }
        }

        public override int PRGNVRAMShiftCount
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.PRGNVRAMShiftCount;
                else return 0;
            }
        }

        public override int CHRRAMSize
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.CHRRAMSize;
                else return 0;
            }
        }

        public override int CHRRAMShiftCount
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.CHRRAMShiftCount;
                else return 0;
            }
        }

        public override int CHRNVRAMSize
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.CHRNVRAMSize;
                else return 0;
            }
        }

        public override int CHRNVRAMShiftCount
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.CHRNVRAMShiftCount;
                else return 0;
            }
        }

        public override int Mapper
        {
            get
            {
                return ROMTYPE switch
                {
                    FORMAT.NES20 => base.Mapper,
                    FORMAT.ARCHAIC => (header[6] & 0xF0) >> 4,
                    FORMAT.INES => (header[7] & 0xF0) | ((header[6] & 0xF0) >> 4),
                    _ => 0,
                };
            }
        }

        public override int SubMapper
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.SubMapper;
                else return 0;
            }
        }

        public override int DefaultExpansionDevice
        {
            get
            {
                if (ROMTYPE == FORMAT.NES20) return base.DefaultExpansionDevice;
                else return 0;
            }
        }

        public void LoadROMData()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    byte[] rom = new byte[FileSizeOnDisk];
                    byte[] temp;

                    using (FileStream fs = File.OpenRead(FileName))
                    {
                        fs.Read(rom, 0, FileSizeOnDisk);
                    }

                    ROMSize = PRGROMSize + CHRROMSize + MISCROMSize;
                    temp = rom.GetBytes(headerOffset, ROMSize);
                    ROMCRC32 = Crc32Algorithm.Compute(temp);
                    ROMSHA1 = SHA1.Create().ComputeHash(temp);

                    temp = rom.GetBytes(headerOffset, PRGROMSize);
                    PRGROMCRC32 = Crc32Algorithm.Compute(temp);
                    PRGROMSHA1 = SHA1.Create().ComputeHash(temp);
                    PRGROMSUM16 = temp.CalculateSUM16();

                    if (CHRROMSize > 0)
                    {
                        temp = rom.GetBytes(headerOffset + PRGROMSize, CHRROMSize);
                        CHRROMCRC32 = Crc32Algorithm.Compute(temp);
                        CHRROMSHA1 = SHA1.Create().ComputeHash(temp);
                        CHRROMSUM16 = temp.CalculateSUM16();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void LoadROMHeader()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    using FileStream fs = File.OpenRead(FileName);
                    fs.Read(header, 0, 16);
                    headerOffset = Trainer ? 16 + 512 : 16;

                    FileSizeOnDisk = (int)fs.Length;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}