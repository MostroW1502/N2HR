using System;
using System.IO;

namespace NES20HeaderRepair.NESHeaderRepair.ROM
{
    public delegate void GetFilesCompletedEventHandler(object sender);

    public class Loader
    {
        public Loader(string Folder)
        {
            this.Folder = Folder;
            GetFileNames();
        }

        public event GetFilesCompletedEventHandler GetFilesCompleted;

        public string Folder { get; private set; }

        public string[] FileNames { get; private set; }

        public string Error { get; private set; }

        private void GetFileNames()
        {
            try
            {
                FileNames = Directory.GetFiles(Folder, "*.nes", SearchOption.AllDirectories);
                OnGetFilesCompleted();
            }
            catch(Exception e)
            {
                if (e is IOException ||
                    e is UnauthorizedAccessException ||
                    e is PathTooLongException ||
                    e is DirectoryNotFoundException)
                {
                    Error = e.Message;
                }
            }
        }

        private void OnGetFilesCompleted()
        {
            GetFilesCompleted?.Invoke(this);
        }
    }
}