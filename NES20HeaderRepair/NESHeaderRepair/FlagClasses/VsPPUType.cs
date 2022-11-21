using System.Collections.ObjectModel;

namespace NES20HeaderRepair.NESHeaderRepair.FlagClasses
{
    public class VsPPUType
    {
        public VsPPUType()
        {
            PPU = new ObservableCollection<VsPPU>
            {
                new VsPPU("RP2C03B", 0),
                new VsPPU("RP2C03G", 1),
                new VsPPU("RP2C04-0001", 2),
                new VsPPU("RP2C04-0002", 3),
                new VsPPU("RP2C04-0003", 4),
                new VsPPU("RP2C04-0004", 5),
                new VsPPU("RC2C03B", 6),
                new VsPPU("RC2C03C", 7),
                new VsPPU("RC2C05-01 ($2002 AND $?? =$1B)", 8),
                new VsPPU("RC2C05-02 ($2002 AND $3F =$3D)", 9),
                new VsPPU("RC2C05-03 ($2002 AND $1F =$1C)", 10),
                new VsPPU("RC2C05-04 ($2002 AND $1F =$1B)", 11),
                new VsPPU("RC2C05-05 ($2002 AND $1F =unknown)", 12),
            };
        }

        public ObservableCollection<VsPPU> PPU { get; private set; }
    }
}