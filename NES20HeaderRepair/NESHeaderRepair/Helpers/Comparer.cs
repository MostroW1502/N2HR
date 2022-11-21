using Mostrow.NESGameDatabase;
using NES20HeaderRepair.NESHeaderRepair.ROM;

namespace NES20HeaderRepair.NESHeaderRepair.Helpers
{
    public class Comparer
    {
        public bool Running { get; set; }
        public Game Game { get; set; }
        public Header Header { get; set; }
    }
}