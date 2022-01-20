using PlatformLib;
using SettingsLib;
using System.IO;

namespace ExifPhotos
{
    public class ExifPhotosSettings : Settings
    {
        /// <summary>
        /// Get the instance of the Settings 
        /// </summary>
        public static ExifPhotosSettings Instance
        {
            get { return s_Settings; }
        }

        /// <summary>
        /// number of grids (1 for phone)
        /// </summary>
        public static int NumberOfGrids
        {
            get { return Instance.GetOrAddSetting(nameof(NumberOfGrids), 1); }
            set { Instance.SetSetting(nameof(NumberOfGrids), value); }
        }

        /// <summary>
        /// image height
        /// </summary>
        public static int ImageHeightRequest
        {
            get { return Instance.GetOrAddSetting(nameof(ImageHeightRequest), 640); }
            set { Instance.SetSetting(nameof(ImageHeightRequest), value); }
        }

        /// <summary>
        /// image width
        /// </summary>
        public static int ImageWidthRequest
        {
            get { return Instance.GetOrAddSetting(nameof(ImageWidthRequest), 800); }
            set { Instance.SetSetting(nameof(ImageWidthRequest), value); }
        }

        /// <summary>
        /// Grid width
        /// </summary>
        public static int GridWidthRequest
        {
            get { return Instance.GetOrAddSetting(nameof(GridWidthRequest), 500); }
            set { Instance.SetSetting(nameof(GridWidthRequest), value); }
        }

        /// <summary>
        /// include photos in sub-folders
        /// </summary>
        public static bool IncludeSubFolder
        {
            get { return Instance.GetOrAddSetting(nameof(IncludeSubFolder), true); }
            set { Instance.SetSetting(nameof(IncludeSubFolder), value); }
        }

        /// <summary>
        /// whether the settings is valid
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// private constructor
        /// </summary>
        private ExifPhotosSettings()
            : base(null)
        {
            string folderPath = FolderUtil.GetFolderPath(ThingsSettingsFolder, false);
            if (!string.IsNullOrEmpty(folderPath)) SettingsFile = Path.Combine(folderPath, ThingsSettingsFile);
            else SettingsFile = ThingsSettingsFile;
            IsValid = LoadSettings();
        }

        private const string ThingsSettingsFolder = "docs";
        private const string ThingsSettingsFile = "exifphotossettings.txt";
        private static ExifPhotosSettings s_Settings = new ExifPhotosSettings();
    }
}
