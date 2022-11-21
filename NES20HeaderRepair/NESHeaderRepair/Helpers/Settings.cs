using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;

namespace NES20HeaderRepair.NESHeaderRepair.Helpers
{
    public class Settings : ApplicationSettingsBase, INotifyPropertyChanged
    {
        private readonly string workingfolder = $@"{Directory.GetCurrentDirectory()}\NES2\Working",
                                defectfolder = $@"{Directory.GetCurrentDirectory()}\NES2\Defective",
                                unverifiedfolder = $@"{Directory.GetCurrentDirectory()}\NES2\Unverified";

        [UserScopedSetting()]
        public string WorkingFolder
        {
            get
            {
                if (string.IsNullOrEmpty((string)this[nameof(WorkingFolder)])) this[nameof(WorkingFolder)] = workingfolder;
                TryCreateOrDefault(nameof(WorkingFolder), workingfolder);
                return (string)this[nameof(WorkingFolder)];
            }
            set
            {
                this[nameof(WorkingFolder)] = Directory.Exists(value) ? value : workingfolder;
            }
        }

        [UserScopedSetting()]
        public string DefectFolder
        {
            get
            {
                if (string.IsNullOrEmpty((string)this[nameof(DefectFolder)])) this[nameof(DefectFolder)] = defectfolder;
                TryCreateOrDefault(nameof(DefectFolder), defectfolder);
                return (string)this[nameof(DefectFolder)];
            }
            set
            {
                this[nameof(DefectFolder)] = Directory.Exists(value) ? value : defectfolder;
            }
        }

        [UserScopedSetting()]
        public string UnverifiedFolder
        {
            get
            {
                if (string.IsNullOrEmpty((string)this[nameof(UnverifiedFolder)])) this[nameof(UnverifiedFolder)] = unverifiedfolder;
                TryCreateOrDefault(nameof(UnverifiedFolder), unverifiedfolder);
                return (string)this[nameof(UnverifiedFolder)];
            }
            set
            {
                this[nameof(UnverifiedFolder)] = Directory.Exists(value) ? value : unverifiedfolder;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("true")]
        public bool PreserveFolderStructure
        {
            get
            {
                return (bool)this[nameof(PreserveFolderStructure)];
            }
            set
            {
                this[nameof(PreserveFolderStructure)] = value;
            }
        }

        private void TryCreateOrDefault(string PropertyName, string FolderName, int Attempt = 0)
        {
            try
            {
                if (!Directory.Exists((string)this[PropertyName])) Directory.CreateDirectory((string)this[PropertyName]);
            }
            catch (Exception)
            {
                this[PropertyName] = FolderName;
                if (Attempt == 0) TryCreateOrDefault(PropertyName, FolderName, 1);
            }
        }
    }
}