using System.Collections.ObjectModel;

namespace NES20HeaderRepair.NESHeaderRepair.FlagClasses
{
    public class CPUPPUTiming
    {
        public CPUPPUTiming()
        {
            Timing = new ObservableCollection<CPUPPU>
            {
                new CPUPPU("RP2C02(\"NTSC NES\")", 0),
                new CPUPPU("RP2C07(\"Licensed PAL NES\")", 1),
                new CPUPPU("Multiple - region", 2),
                new CPUPPU("UMC 6527P(\"Dendy\")", 3)
            };
        }

        public ObservableCollection<CPUPPU> Timing { get; private set; }
    }
}
