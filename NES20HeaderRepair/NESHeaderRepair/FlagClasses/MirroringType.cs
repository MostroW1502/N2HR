using System.Collections.ObjectModel;

namespace NES20HeaderRepair.NESHeaderRepair.FlagClasses
{
    public class MirroringType
    {
        public MirroringType()
        {
            Mirroring = new ObservableCollection<Mirroring>
            {
                new Mirroring("Horizontal or mapper-controlled", 0),
                new Mirroring("Vertical", 1)
            };
        }

        public ObservableCollection<Mirroring> Mirroring { get; private set; }
    }
}