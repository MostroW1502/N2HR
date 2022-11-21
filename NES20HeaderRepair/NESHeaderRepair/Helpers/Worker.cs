namespace NES20HeaderRepair.NESHeaderRepair.Helpers
{
    public class Worker
    {
        public Worker(string FileName, int FileTotal, int FileNumber, int FileSize)
        {
            this.FileName = FileName;
            this.FileTotal = FileTotal;
            this.FileNumber = FileNumber;
            this.FileSize = FileSize;
        }

        public string FileName { get; private set; }

        public int FileNumber { get; private set; }

        public int FileTotal { get; private set; }

        public int FileSize { get; private set; }
    }
}