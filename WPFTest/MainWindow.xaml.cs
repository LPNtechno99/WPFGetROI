using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetROI_Click(object sender, RoutedEventArgs e)
        {
            if (!ZoomBorder.GetROI)
            {
                ZoomBorder.GetROI = true;
                border.Cursor = null;
                btnGetROI.Header = "Deselect ROI";
                imageEx.InvalidateVisual();
            }
            else
            {
                ZoomBorder.GetROI = false;
                border.Cursor = null;
                btnGetROI.Header = "Select ROI";
            }

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                imageEx.Source = new BitmapImage(new Uri(ofd.FileName));
            }
        }
    }
}
