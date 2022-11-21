using System.Collections.ObjectModel;

namespace NES20HeaderRepair.NESHeaderRepair.FlagClasses
{
    public class ConsoleType
    {
        public ConsoleType()
        {
            Consoles = new ObservableCollection<Console>
            {
                new Console("Nintendo Entertainment System/Family Computer", 0),
                new Console("Nintendo Vs. System", 1),
                new Console("Nintendo Playchoice 10", 2),
                new Console("Extended Console Type", 3)
            };
        }

        public ObservableCollection<Console> Consoles { get; private set; }
    }
}