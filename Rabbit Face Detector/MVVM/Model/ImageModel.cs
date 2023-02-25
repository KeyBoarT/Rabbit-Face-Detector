using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rabbit_Face_Detector.MVVM.Model
{
    class ImageModel
    {
        #region Fields
        private ImageSource _image;
        private Stretch _mode;
        #endregion
        #region Properties
        public ImageSource Image { get { return _image; } set { _image = value; } }
        public Stretch Mode { get { return _mode; } set { _mode = value; } }
        #endregion

        public ImageModel(BitmapImage image, Stretch mode)
        {
            Image = image;
            Mode = mode;
        }

        public ImageModel(BitmapSource image, Stretch mode)
        {
            Image = image;
            Mode = mode;
        }
    }
}
