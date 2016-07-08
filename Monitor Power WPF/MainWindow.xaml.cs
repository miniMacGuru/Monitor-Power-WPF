using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Monitor_Power_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /**
         * http://www.codeproject.com/Articles/12794/Complete-Guide-on-How-To-Turn-A-Monitor-On-Off-Sta 
         */
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        const int SC_MONITORPOWER = 0xF170;
        const int WM_SYSCOMMAND = 0x0112;

        const int MONITOR_ON = -1;
        const int MONITOR_OFF = 2;
        const int MONITOR_STANBY = 1;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void DimMonitors()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
        }

        private void ShutdownComputer()
        {
            Process.Start("shutdown.exe", "/s /f /t 0");
        }

        private void btnDimMonitors_Click(object sender, RoutedEventArgs e)
        {
            DimMonitors();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (shutdownTimer != null)
            {
                shutdownTimer.Stop();
                shutdownTimer.Dispose();
                shutdownTimer = null;
            }
        }

        Timer shutdownTimer = null;
        private void btnTimedShutdown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime targetDate = DateTime.Now;
                targetDate = targetDate.AddHours(Convert.ToInt32(txtHours.Text));
                targetDate = targetDate.AddMinutes(Convert.ToInt32(txtMinutes.Text));
                targetDate = targetDate.AddSeconds(Convert.ToInt32(txtSeconds.Text));

                shutdownTimer = new Timer(1000);
                shutdownTimer.AutoReset = true;
                shutdownTimer.Elapsed += delegate (Object s, ElapsedEventArgs ea)
                {
                    TimeSpan diff = targetDate.Subtract(DateTime.Now);
                    String formattedTime = String.Format("Remaining Time: {0:00}:{1:00}:{2:00}", diff.Hours, diff.Minutes, diff.Seconds);

                    lblRemainingTime.Dispatcher.Invoke(delegate
                    {
                        lblRemainingTime.Content = formattedTime;
                    });
                    
                    
                    if (diff.Seconds <= 0)
                    {
                        shutdownTimer.Stop();
                        ShutdownComputer();
                    }
                };

                shutdownTimer.Start();
                DimMonitors();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to start timer: " + ex.Message);
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtHours_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHours.SelectAll();
        }

        private void txtMinutes_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMinutes.SelectAll();
        }

        private void txtSeconds_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSeconds.SelectAll();
        }
    }
}
