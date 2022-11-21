namespace NES20HeaderRepair.NESHeaderRepair.Helpers
{
    public class ComboBase
    {
        public ComboBase(string Name, int Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        public string Name { get; private set; }

        public int Value { get; set; }
    }
}