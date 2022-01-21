using ExifMetadata;
using FormsLib;
using PlatformLib;
using SettingsLib;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace ExifPhotos
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Title = "Exif Photos";

            // create label & image to display photo
            _imageLabel = new Label() { Text = "image", FontSize = FormsUtil.LabelMediumFontSize };
            ImageStack.Children.Add(_imageLabel);
            int width = Math.Min((int)Device.Info.ScaledScreenSize.Width - 10, ExifPhotosSettings.ImageWidthRequest);
            int height = Math.Max(width * 3 / 4, ExifPhotosSettings.ImageHeightRequest);
            _image = new ZoomImage() { HeightRequest = height, WidthRequest = width };
            // todo: swipe doesn't work?
            var leftSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
            leftSwipeGesture.Swiped += OnSwipedLeft;
            ExifDataStack.GestureRecognizers.Add(leftSwipeGesture);
            var rightSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };
            rightSwipeGesture.Swiped += OnSwipedRight;
            ExifDataStack.GestureRecognizers.Add(rightSwipeGesture);
            ImageStack.Children.Add(_image);

            // create grids and initialize tag list to be displayed for each grid
            int numberOfGrids = ExifPhotosSettings.NumberOfGrids;
            for (int i = 0; i < numberOfGrids; i++)
            {
                KeyValueGrid grid = new KeyValueGrid("Tag", "Value", ExifPhotosSettings.GridWidthRequest, 2, 2, 3);
                _grids.Add(grid);
                ExifDataStack.Children.Add(grid);
                List<string> tagList = new List<string>();
                _gridTags.Add(tagList);

                // get list og tags from all the groups assigned with this index value
                IEnumerable<SettingGroup> groups = ExifPhotosSettings.Instance.GetSettingGroups("Index", i.ToString());
                foreach (SettingGroup group in groups)
                {
                    // the Tags setting contains comma separated list of tags
                    string tagsString;
                    if (group.Settings.TryGetValue("Tags", out tagsString))
                    {
                        string[] parts = tagsString.Split(CommaDelimiter);
                        foreach (string part in parts)
                        {
                            tagList.Add(part);
                        }
                    }
                }
                if (tagList.Count > _gridRows) _gridRows = tagList.Count;
            }

            // create extractor and initialize with standard and nikon tag mapping
            _extractor = new MetadataExtractorProvider();
            _extractor.AddMap(MetadataExtractorProvider.StandardTagMapDefs);
            // load mappings specified in settings file
            string mappingSetting;
            if (ExifPhotosSettings.Instance.TryGetSetting<string>("TagMappingFile", out mappingSetting))
            {
                string[] parts = mappingSetting.Split(CommaDelimiter);
                foreach (string part in parts)
                {
                    string csvFile = part;
                    if (!Path.IsPathRooted(csvFile)) csvFile = Path.Combine(ExifPhotosSettings.Instance.SettingsFolder, part);
                    _extractor.AddMap(MetadataExtractorProvider.LoadTagMapFromCsv(csvFile));
                }
            }

            // initialize list of photos to be displayed
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    break;
                case Device.Android:
                    FindPhotosFromCameraFolderAndroid();
                    UpdateDisplay();
                    break;
                case Device.UWP:
                    // temp
                    _photoList.Add("PXL_20220111_214012553.jpg");
                    UpdateDisplay();
                    break;
                default:
                    break;
            }
        }

        protected override void OnAppearing()
        {
            UpdateDisplay();
        }

        protected override void OnDisappearing()
        {
        }

        private static readonly char[] CommaDelimiter = { ',' };
        private MetadataExtractorProvider _extractor;
        private Label _imageLabel;
        private Image _image;
        private List<KeyValueGrid> _grids = new List<KeyValueGrid>();
        private int _gridRows;      // max number of data rows (not including header row
        private List<List<string>> _gridTags = new List<List<string>>();    // tags for each grid
        private PhotoList _photoList = new PhotoList();

        private void UpdateDisplay()
        {
            string photoFile = _photoList.CurrentPhoto;
            if (File.Exists(photoFile))
            {
                _imageLabel.Text = Path.GetFileName(photoFile);
                _image.Source = photoFile;
                Metadata photoMetadata = new Metadata(_extractor.Extract(photoFile));
                for (int i = 0; i < _grids.Count; i++)
                {
                    KeyValueGrid grid = _grids[i];
                    grid.Clear();
                    List<string> tagList = _gridTags[i];
                    foreach (string part in tagList)
                    {
                        string tagValue;
                        if (!photoMetadata.TryGetTagValue(part, out tagValue)) tagValue = string.Empty;
                        grid.AddRow(part, tagValue);
                    }
                    for (int row = grid.RowCount; row <= _gridRows; row++)
                    {
                        // fill empty rows so the size of all grid are equal
                        grid.AddRow(string.Empty, string.Empty);
                    }
                }
            }
        }

        private void FindPhotosFromCameraFolderAndroid()
        {
            string currentFolder = FolderUtil.GetFolderPath("DCIM", "Camera", false);       //"/storage/emulated/0/DCIM/Camera";
            _photoList.CurrentFolder = currentFolder;
            List<string> files = _photoList.Photos;
            if (!Util.FindPhotosUnderFolder(currentFolder, files, ExifPhotosSettings.IncludeSubFolder))
            {
                List<string> folders = new List<string>();
                Util.GetSubFolders("/storage", folders);
                foreach (string folder in folders)
                {
                    try
                    {
                        // will get exception when trying system folder like /storage/emulated or /storage/self
                        currentFolder = folder + "/DCIM/Camera";
                        if (Util.FindPhotosUnderFolder(currentFolder, files, ExifPhotosSettings.IncludeSubFolder))
                        {
                            _photoList.CurrentFolder = currentFolder;
                            break;
                        }
                    }
                    catch
                    {
                        // just skip
                    }
                }
            }
        }

        private void PrevPhoto()
        {
            if (_photoList.PrevPhoto()) UpdateDisplay();
        }

        private void NextPhoto()
        {
            if (_photoList.NextPhoto()) UpdateDisplay();
        }

        private async void OpenButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new PickFolderPage(_photoList)));
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            NextPhoto();
        }

        private void PrevButton_Clicked(object sender, EventArgs e)
        {
            PrevPhoto();
        }

        private void OnSwipedLeft(object sender, SwipedEventArgs e)
        {
            NextPhoto();
        }
        private void OnSwipedRight(object sender, SwipedEventArgs e)
        {
            PrevPhoto();
        }
    }
}
