using System.Collections.ObjectModel;

namespace NES20HeaderRepair.NESHeaderRepair.FlagClasses
{
    public class ExtendedConsoleType
    {
        public ExtendedConsoleType()
        {
            Types = new ObservableCollection<ExtendedConsole>
            {
                new ExtendedConsole("[Regular NES/Famicom/Dendy]", 0),
                new ExtendedConsole("[Nintendo Vs. System]", 1),
                new ExtendedConsole("[Playchoice 10]", 2),
                new ExtendedConsole("Regular Famiclone, but with CPU that supports Decimal Mode (e.g. Bit Corporation Creator)", 3),
                new ExtendedConsole("V.R. Technology VT01 with monochrome palette", 4),
                new ExtendedConsole("V.R. Technology VT01 with red/cyan STN palette", 5),
                new ExtendedConsole("V.R. Technology VT02", 6),
                new ExtendedConsole("V.R. Technology VT03", 7),
                new ExtendedConsole("V.R. Technology VT09", 8),
                new ExtendedConsole("V.R. Technology VT32", 9),
                new ExtendedConsole("V.R. Technology VT369", 10),
                new ExtendedConsole("UMC UM6578", 11),
            };
        }

        public ObservableCollection<ExtendedConsole> Types { get; private set; }
    }
}