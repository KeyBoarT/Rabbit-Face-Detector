using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
using System.Drawing;
using System.Windows.Threading;
using System.Reflection;
using System.IO;

namespace Rabbit_Face_Detector.MVVM.Model
{
    class CaptureModel
    {
        #region Fields
        private Capture _capture;
        private Image<Bgr, byte> _currentFrame;
        private Image<Bgr, byte> _detectedFace;
        private Mat _frame;
        private int _frameSize;
        private bool _isDetecting;
        private bool _isAddPerson;
        private bool _isSavePerson;
        private static bool _isTrained;
        private static List<Image<Gray, Byte>> _trainedFaces = new List<Image<Gray, byte>>();
        private static List<int> _personLabels = new List<int>();
        private string _persoName;
        private CascadeClassifier _cascadeClassifier = new CascadeClassifier(@"C:\Users\mehme\source\repos\Rabbit Face Detector\Rabbit Face Detector\Classifiers\haarcascade_frontalface_alt.xml");
        #endregion

        #region Properties
        public Capture Capture { get { return _capture; } set { _capture = value; } }
        public Image<Bgr, Byte> CurrentFrame { get { return _currentFrame; } set { _currentFrame = value; } }
        public Image<Bgr, Byte> DetectedFace { get { return _detectedFace; } set { _detectedFace = value; } }
        public Mat Frame { get { return _frame; } set { _frame = value; } }
        public int FrameSize { get { return _frameSize; } set { _frameSize = value; } }
        public bool IsDetecing { get { return _isDetecting; } set { _isDetecting = value; } }
        public bool IsAddPerson { get { return _isAddPerson; } set { _isAddPerson = value; } }
        public bool IsSavePerson { get { return _isSavePerson; }  set { _isSavePerson = value; } }
        private static bool IsTrained { get { return _isTrained; } set { _isTrained = value; } }
        private static List<Image<Gray, Byte>> TrainedFaces { get { return _trainedFaces; } set { _trainedFaces = value; } }
        private static List<int> PersonLabels { get { return _personLabels; } set { _personLabels = value; } }
        public string PersonName { get { return _persoName; } set { _persoName = value; } }
        public CascadeClassifier CascadeClassifier { get { return _cascadeClassifier; } set { _cascadeClassifier = value; } }
        #endregion

        public CaptureModel()
        {
            Capture = new Capture();
            Frame = new Mat();
            FrameSize = 1980;
            Capture.ImageGrabbed += Camera_ImageGrabbed;
        }

        public void Start()
        {
            Capture.Start();
        }

        public void Camera_ImageGrabbed(object sender, EventArgs e)
        {
            Tools.Tools.BackgroundWorker(() =>
            {
                Capture.Retrieve(Frame, 0);
                CurrentFrame = Frame.ToImage<Bgr, Byte>();
                if (IsDetecing) { DetectFaces(); }
            }, DispatcherPriority.Background);
        }
        private void DetectFaces()
        {
            Tools.Tools.BackgroundWorker(() =>
            {
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(CurrentFrame, grayImage, ColorConversion.Bgr2Gray);
            CvInvoke.EqualizeHist(grayImage, grayImage);
            Rectangle[] faces = CascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, Size.Empty, Size.Empty);
                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        CvInvoke.Rectangle(CurrentFrame, face, new Bgr(Color.White).MCvScalar, 2);
                        if (IsAddPerson) { AddPerson(face); }
                    }
                }
            }, DispatcherPriority.Background);
        }

        private void AddPerson(Rectangle face)
        {
            Tools.Tools.BackgroundWorker(()=>
            {
                Image<Bgr, Byte> resultImage = CurrentFrame.Convert<Bgr, Byte>();
                resultImage.ROI = face;
                DetectedFace = resultImage;
            });
        }

        public void SavePerson()
        {
            if (PersonName != null)
            {
                Console.WriteLine("Saving Person !");
                string path = Directory.GetCurrentDirectory() + @"\SavedPersons";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(path + @"\" + PersonName + DateTime.Now.Ticks + i.ToString() + ".jpg");
                    DetectedFace.Resize(200, 200, Inter.Cubic).Save(path + @"\" + PersonName + "-" + DateTime.Now.Ticks + "-"  +  i.ToString() +".jpg");
                }
            }
        }

        public void RecognizeImages()
        {
            if (IsTrained)
            {
                Image<Gray, Byte> grayFaceResult = DetectedFace.Convert<Gray, Byte>().Resize(200,200, Inter.Cubic);
            }
        }

        public static bool TrainImagesFromDir()
        {
            int ImagesCount = 0;
            int threshold = 7000;
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\SavedFaces";
                string[] files = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    Image<Gray, Byte> trainedImage = new Image<Gray, Byte>(file);
                    TrainedFaces.Add(trainedImage);
                    PersonLabels.Add(ImagesCount);
                    ImagesCount++;
                }

                EigenFaceRecognizer recognizer = new EigenFaceRecognizer(ImagesCount, threshold);
                recognizer.Train(TrainedFaces.ToArray(), PersonLabels.ToArray());
                return IsTrained = true;
            }
            catch
            {
                return IsTrained = false;
            }
        }
    }
}
