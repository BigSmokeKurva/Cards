using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Cards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private static string activePage = string.Empty;
        #region Rounded window
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                DwmSetWindowAttribute(
                    new WindowInteropHelper(GetWindow(this)).EnsureHandle(),
                    DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                    ref preference,
                    sizeof(uint));
            }
            catch { }

            GoToPage("LoginPage");
        }
        private void CenterWindow()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);

            int screenW = screen.Bounds.Width;
            int screenH = screen.Bounds.Height;
            int screenTop = screen.Bounds.Top;
            int screenLeft = screen.Bounds.Left;
            Left = (int)(screenLeft + (screenW - Width * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / SystemParameters.WorkArea.Width) / 2) * SystemParameters.WorkArea.Width / System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            Top = (int)(screenTop + (screenH - Height * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height) / 2) * SystemParameters.WorkArea.Height / System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
        }

        internal void GoToPage(string pageName)
        {
            LoginPage CreateLoginPage()
            {
                activePage = pageName;
                var loginPage = new LoginPage(this);
                Application.Current.MainWindow.Height = 530;
                Application.Current.MainWindow.Width = 893;
                Maximize_Button.Visibility = Visibility.Collapsed;
                return loginPage;
            }
            if (activePage == "LoginPage")
            {
                activePage = pageName;
                Application.Current.MainWindow.Height = 720;
                Application.Current.MainWindow.Width = 1280;
                Maximize_Button.Visibility = Visibility.Visible;
                CenterWindow();
            }
            NavigationControl.Content = pageName switch
            {
                "LoginPage" => CreateLoginPage(),
                "EducationPage" => new HubControl(pageName),
                "EditorPage" => new HubControl(pageName),
                _ => CreateLoginPage(),
            };
        }
        private void Button_Close(object sender, RoutedEventArgs e) => Close();
        private void Button_Roll(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;
        private void Button_Maximize(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        private void WindowMoveEvent(object sender, MouseButtonEventArgs e)
        {
            static Point GetMousePosition()
            {
                System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
                return new Point(point.X, point.Y);
            }
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var mouse = transform.Transform(GetMousePosition());
                Left = mouse.X - (((Border)sender).ActualWidth / 2);
                Top = mouse.Y;
            }
            DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (activePage == "LoginPage")
            {
                Application.Current.MainWindow.Height = 530;
                Application.Current.MainWindow.Width = 893;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (activePage == "LoginPage" && Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }
    }
}
