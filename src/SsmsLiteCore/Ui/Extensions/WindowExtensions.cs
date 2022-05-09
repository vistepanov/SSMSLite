using System.Windows;

namespace SsmsLite.Core.Ui.Extensions
{
    public static class WindowExtensions
    {
        public static void ShowAndActivate(this Window window)
        {
            window.Show();
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }
            window.Activate();
        }
    }
}
