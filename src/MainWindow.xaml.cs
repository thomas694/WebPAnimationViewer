using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace WebpAnimationViewer
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            anim.Source = new System.Uri(Path.Combine(path, "dancing_banana2.lossless.webp"));

            Task.Run(async () =>
            {
                await Task.Delay(5000);
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    anim.Source = new System.Uri(Path.Combine(path, "butterfly-small.webp"));
                }));
            });
        }
    }
}
