using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rabbit_Face_Detector.Tools
{
    internal class Tools
    {
        public static void BackgroundWorker(Action act, System.Windows.Threading.DispatcherPriority dispatcherPriority = System.Windows.Threading.DispatcherPriority.Background)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(act, dispatcherPriority);
            }
            catch { }
        }

        public static void Try(Action act)
        {
            try
            {
                act.Invoke();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
