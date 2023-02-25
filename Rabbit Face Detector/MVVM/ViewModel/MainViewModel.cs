using System;
using System.Threading.Tasks;
using Rabbit_Face_Detector.Core;
using Rabbit_Face_Detector.MVVM.Model;
using Rabbit_Face_Detector.Extensions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;

namespace Rabbit_Face_Detector.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        #region Fields
        private CaptureModel _camera;
        private ImageModel _currentImage = new ImageModel(new BitmapImage(new Uri(@"C:\Users\mehme\source\repos\Bunny Face Detector\Bunny Face Detector\Icons\noCam.png", UriKind.Relative)), Stretch.None);
        private ImageModel _detectedFace = new ImageModel(new BitmapImage(new Uri(@"C:\Users\mehme\source\repos\Bunny Face Detector\Bunny Face Detector\Icons\noCam.png", UriKind.Relative)), Stretch.None);
        private string _faceName;
        private bool _isLoading;
        #endregion

        #region Properties
        public CaptureModel Camera
        {
            get { return _camera; }
            set
            {
                _camera = value;
                OnPropertyChanged();
            }
        }
        public ImageModel CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }
        public ImageModel DetectedFace
        {
            get { return _detectedFace; }
            set
            {
                _detectedFace = value;
                OnPropertyChanged();
            }
        }
        public string FaceName
        {
            get { return _faceName; }
            set
            {
                _faceName = value;
                OnPropertyChanged();
            }
        }
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region RelayCommands
        public RelayCommand MoveWindowCommand { get; set; }
        public RelayCommand ShutDownCommand { get; set; }
        public RelayCommand MaximizeCommand { get; set; }
        public RelayCommand MinimizeCommand { get; set; }
        public RelayCommand OpenCamCommand { get; set; }
        public RelayCommand DetectFacesCommand { get; set; }
        public RelayCommand AddPersonCommand { get; set; }
        public RelayCommand SavePersonCommand { get; set; }
        #endregion

        public MainViewModel()
        {
            MoveWindowCommand = new RelayCommand(o => { Application.Current.MainWindow.DragMove(); });
            ShutDownCommand = new RelayCommand(o => { Application.Current.Dispatcher.InvokeShutdown(); Application.Current.Shutdown(); });
            MaximizeCommand = new RelayCommand(o => { Application.Current.MainWindow.WindowState = (Application.Current.MainWindow.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized; });
            MinimizeCommand = new RelayCommand(o => { Application.Current.MainWindow.WindowState = WindowState.Minimized; });
            OpenCamCommand = new RelayCommand(o =>
            {
                IsLoading = true;
                Task.Run(() =>
                {
                    if (Camera == null)
                    {
                        Camera = new CaptureModel();
                        Camera.Capture.ImageGrabbed += Camera_ImageGrabbed;
                        Camera.Start();
                    }
                }).ContinueWith(t => { IsLoading = false; });
            });
            DetectFacesCommand = new RelayCommand(o =>{
                //Old style methodologizing, it's over because this method takes huge memory...
                //Camera.Capture.ImageGrabbed -= Camera_ImageGrabbed;

                //New style methodologizing, it's best feature is taking less memory than old style...
                Camera.IsDetecing = !Camera.IsDetecing;
            });
            AddPersonCommand = new RelayCommand(o =>
            {
                Camera.IsAddPerson = !Camera.IsAddPerson;
            });
            SavePersonCommand = new RelayCommand(o =>
            {
                Camera.PersonName = FaceName;
                Console.WriteLine(Camera.PersonName);
                Camera.SavePerson();
            });
        }

        public void Camera_ImageGrabbed(object sender, EventArgs e)
        {
            Tools.Tools.BackgroundWorker(new Action(() =>
            {
                CurrentImage = new ImageModel(EmguCvExtension.ToBitmapSource(Camera.CurrentFrame), Stretch.Fill);
                if (Camera.DetectedFace!=null) { DetectedFace = new ImageModel(EmguCvExtension.ToBitmapSource(Camera.DetectedFace), Stretch.Fill); }
            }), DispatcherPriority.Background);
        }
    }
}
