using System.Collections.Generic;

namespace ExifPhotos
{
    public class PhotoList
    {
        /// <summary>
        /// the photo displayed
        /// </summary>
        public string CurrentPhoto
        {
            get
            {
                if (Photos == null || Photos.Count == 0) return null;
                return Photos[_photoIndex];
            }
        }

        /// <summary>
        /// the folder that is opened
        /// </summary>
        public string CurrentFolder { get; set; }

        /// <summary>
        /// add a photo file to the list
        /// </summary>
        /// <param name="photoFile"></param>
        public void Add(string photoFile)
        {
            Photos.Add(photoFile);
        }

        /// <summary>
        /// add photo files to the list
        /// </summary>
        /// <param name="photoFile"></param>
        public void AddRange(IEnumerable<string> files)
        {
            Photos.AddRange(files);
        }

        /// <summary>
        /// clear the photo list
        /// </summary>
        public void Clear()
        {
            _photoIndex = 0;
            Photos.Clear();
        }

        /// <summary>
        /// change current photo to previous
        /// </summary>
        public bool PrevPhoto()
        {
            int oldIndex = _photoIndex;
            if (Photos.Count > 1)
            {
                if (_photoIndex > 0) _photoIndex--;
                else if (Photos.Count > 1) _photoIndex = Photos.Count - 1;
            }
            return oldIndex != _photoIndex;
        }

        /// <summary>
        /// change current photo to next one
        /// </summary>
        public bool NextPhoto()
        {
            int oldIndex = _photoIndex;
            if (Photos.Count > _photoIndex+1) _photoIndex++;
            else if (Photos.Count > 1) _photoIndex = 0;
            return oldIndex != _photoIndex;
        }

        /// <summary>
        /// list of available photos
        /// </summary>
        public List<string> Photos { get; private set; } = new List<string>();

        // index to _photoList to display
        private int _photoIndex;
    }
}
