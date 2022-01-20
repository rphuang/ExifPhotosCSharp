using FormsLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExifPhotos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickFolderPage : ContentPage
    {
        public PickFolderPage(PhotoList photoList)
        {
            InitializeComponent();
            Title = "Exif Photos";
            _photoList = photoList;

            Open();
        }

        private PhotoList _photoList;
        private List<LabelCheckBox> _labelCheckBoxes = new List<LabelCheckBox>();
        private List<string> _fullPaths = new List<string>();

        private void Open()
        {
            ContentStackLayout.Children.Clear();
            _labelCheckBoxes.Clear();
            _fullPaths.Clear();

            string parentFolder = Path.GetDirectoryName(_photoList.CurrentFolder);
            AddLabelCheckBox($"Parent Folder: {Path.GetFileName(parentFolder)}", parentFolder);
            ContentStackLayout.Children.Add(new Label() { Text = "Sub Folders" });

            // get all subfolders
            List<string> folders = new List<string>();
            Util.GetSubFolders(_photoList.CurrentFolder, folders);
            foreach (string folder in folders)
            {
                AddLabelCheckBox(Path.GetFileName(folder), folder);
            }
        }

        private void AddLabelCheckBox(string name, string fullPath)
        {
            LabelCheckBox labelCheckBox = new LabelCheckBox(name);
            ContentStackLayout.Children.Add(labelCheckBox);
            _labelCheckBoxes.Add(labelCheckBox);
            _fullPaths.Add(fullPath);
        }

        private async void QuitToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void OpenToolbarItem_Clicked(object sender, EventArgs e)
        {
            // open the first picked folder
            string folder = null;
            for (int jj = 0; jj < _fullPaths.Count; jj++)
            {
                if (_labelCheckBoxes[jj].IsChecked)
                {
                    folder = _fullPaths[jj];
                    break;
                }
            }
            // if nothing is clicked then open parent folder
            if (folder == null) folder = _fullPaths[0];
            _photoList.CurrentFolder = folder;
            Open();
        }

        private async void PickToolbarItem_Clicked(object sender, EventArgs e)
        {
            _photoList.Clear();
            for (int jj = 0; jj < _fullPaths.Count; jj++)
            {
                if (_labelCheckBoxes[jj].IsChecked)
                {
                    string folder = _fullPaths[jj];
                    Util.FindPhotosUnderFolder(folder, _photoList.Photos, ExifPhotosSettings.IncludeSubFolder);
                }
            }
            await Navigation.PopModalAsync();
        }
    }
}