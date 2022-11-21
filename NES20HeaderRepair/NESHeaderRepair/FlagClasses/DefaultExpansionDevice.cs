using System.Collections.ObjectModel;

namespace NES20HeaderRepair.NESHeaderRepair.FlagClasses
{
    public class DefaultExpansionDevice
    {
        public DefaultExpansionDevice()
        {
            DefaultExpansion = new ObservableCollection<ExpansionDevice>
            {
                new ExpansionDevice("Unspecified", 0x00),
                new ExpansionDevice("Standard NES/Famicom controllers", 0x01),
                new ExpansionDevice("NES Four Score/Satellite with two additional standard controllers", 0x02),
                new ExpansionDevice("Famicom Four Players Adapter with two additional standard controllers", 0x03),
                new ExpansionDevice("Vs. System", 0x04),
                new ExpansionDevice("Vs. System with reversed inputs", 0x05),
                new ExpansionDevice("Vs. Pinball (Japan)", 0x06),
                new ExpansionDevice("Vs. Zapper", 0x07),
                new ExpansionDevice("Zapper ($4017)", 0x08),
                new ExpansionDevice("Two Zappers", 0x09),
                new ExpansionDevice("Bandai Hyper Shot Lightgun", 0x0A),
                new ExpansionDevice("Power Pad Side A", 0x0B),
                new ExpansionDevice("Power Pad Side B", 0x0C),
                new ExpansionDevice("Family Trainer Side A", 0x0D),
                new ExpansionDevice("Family Trainer Side B", 0x0E),
                new ExpansionDevice("Arkanoid Vaus Controller (NES)", 0x0F),
                new ExpansionDevice("Arkanoid Vaus Controller (Famicom)", 0x10),
                new ExpansionDevice("Two Vaus Controllers plus Famicom Data Recorder", 0x11),
                new ExpansionDevice("Konami Hyper Shot Controller", 0x12),
                new ExpansionDevice("Coconuts Pachinko Controller", 0x13),
                new ExpansionDevice("Exciting Boxing Punching Bag (Blowup Doll)", 0x14),
                new ExpansionDevice("Jissen Mahjong Controller", 0x15),
                new ExpansionDevice("Party Tap ", 0x16),
                new ExpansionDevice("Oeka Kids Tablet", 0x17),
                new ExpansionDevice("Sunsoft Barcode Battler", 0x18),
                new ExpansionDevice("Miracle Piano Keyboard", 0x19),
                new ExpansionDevice("Pokkun Moguraa (Whack-a-Mole Mat and Mallet)", 0x1A),
                new ExpansionDevice("Top Rider (Inflatable Bicycle)", 0x1B),
                new ExpansionDevice("Double-Fisted (Requires or allows use of two controllers by one player)", 0x1C),
                new ExpansionDevice("Famicom 3D System", 0x1D),
                new ExpansionDevice("Doremikko Keyboard", 0x1E),
                new ExpansionDevice("R.O.B. Gyro Set", 0x1F),
                new ExpansionDevice("Famicom Data Recorder (don't emulate keyboard)", 0x20),
                new ExpansionDevice("ASCII Turbo File", 0x21),
                new ExpansionDevice("IGS Storage Battle Box", 0x22),
                new ExpansionDevice("Family BASIC Keyboard plus Famicom Data Recorder", 0x23),
                new ExpansionDevice("Dongda PEC-586 Keyboard", 0x24),
                new ExpansionDevice("Bit Corp. Bit-79 Keyboard", 0x25),
                new ExpansionDevice("Subor Keyboard", 0x26),
                new ExpansionDevice("Subor Keyboard plus mouse (3x8-bit protocol)", 0x27),
                new ExpansionDevice("Subor Keyboard plus mouse (24-bit protocol)", 0x28),
                new ExpansionDevice("SNES Mouse ($4017.d0)", 0x29),
                new ExpansionDevice("Multicart", 0x2A),
                new ExpansionDevice("Two SNES controllers replacing the two standard NES controllers", 0x2B),
                new ExpansionDevice("RacerMate Bicycle", 0x2C),
                new ExpansionDevice("U-Force", 0x2D),
                new ExpansionDevice("R.O.B. Stack-Up", 0x2E),
                new ExpansionDevice("City Patrolman Lightgun", 0x2F),
                new ExpansionDevice("Sharp C1 Cassette Interface", 0x30),
                new ExpansionDevice("Standard Controller with swapped Left-Right/Up-Down/B-A", 0x31),
                new ExpansionDevice("Excalibor Sudoku Pad", 0x32),
                new ExpansionDevice("ABL Pinball", 0x33),
                new ExpansionDevice("Golden Nugget Casino extra buttons", 0x34)
            };
        }

        public ObservableCollection<ExpansionDevice> DefaultExpansion { get; private set; }
    }
}