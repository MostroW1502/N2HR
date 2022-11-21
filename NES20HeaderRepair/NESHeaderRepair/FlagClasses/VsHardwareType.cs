using System.Collections.ObjectModel;

namespace NES20HeaderRepair.NESHeaderRepair.FlagClasses
{
    public class VsHardwareType
    {
        public VsHardwareType()
        {
            Hardware = new ObservableCollection<VsHardware>
            {
                new VsHardware("Vs. Unisystem (normal)", 0),
                new VsHardware("Vs. Unisystem (RBI Baseball protection)", 1),
                new VsHardware("Vs. Unisystem (TKO Boxing protection)", 2),
                new VsHardware("Vs. Unisystem (Super Xevious protection)", 3),
                new VsHardware("Vs. Unisystem (Vs. Ice Climber Japan protection)", 4),
                new VsHardware("Vs. Dual System (normal)", 5),
                new VsHardware("Vs. Dual System (Raid on Bungeling Bay protection)", 6)
            };
        }

        public ObservableCollection<VsHardware> Hardware { get; private set; }
    }
}