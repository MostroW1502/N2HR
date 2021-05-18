using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;

namespace NES20HeaderRepair.NESHeaderRepair.Helpers
{
    public class Settings : ApplicationSettingsBase, INotifyPropertyChanged
    {
        private readonly string workingfolder = $@"{Directory.GetCurrentDirectory()}\NES2\Working",
                                defectfolder = $@"{Directory.GetCurrentDirectory()}\NES2\Defective";

        [UserScopedSetting()]
        public string WorkingFolder
        {
            get
            {
                if (string.IsNullOrEmpty((string)this["WorkingFolder"])) this["WorkingFolder"] = workingfolder;
                TryCreateOrDefault("WorkingFolder", workingfolder);
                return (string)this["WorkingFolder"];
            }
            set
            {
                this["WorkingFolder"] = Directory.Exists(value) ? value : workingfolder;
            }
        }

        [UserScopedSetting()]
        public string DefectFolder
        {
            get
            {
                if (string.IsNullOrEmpty((string)this["DefectFolder"])) this["DefectFolder"] = defectfolder;
                TryCreateOrDefault("DefectFolder", defectfolder);
                return (string)this["DefectFolder"];
            }
            set
            {
                this["DefectFolder"] = Directory.Exists(value) ? value : defectfolder;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("true")]
        public bool PreserveFolderStructure
        {
            get
            {
                return (bool)this["PreserveFolderStructure"];
            }
            set
            {
                this["PreserveFolderStructure"] = value;
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