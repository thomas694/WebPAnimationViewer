using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WebpAnimationViewer
{
    // based on: https://social.msdn.microsoft.com/Forums/vstudio/en-US/f75a854d-0eb1-4aca-af37-65368d9049e4/how-to-animate-a-gif-in-wpf-using-drawimage
    // (https://stackoverflow.com/questions/10764585/how-to-add-gif-image-to-wpf)

    public class AnimatedGIFControl : FrameworkElement
    {
        private Bitmap _bitmap;
        private BitmapSource _bitmapSource;
        private bool _started;
        public delegate void FrameUpdatedEventHandler();

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(_bitmapSource, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        /// <summary>
        /// Delete local bitmap resource
        /// Reference: http://msdn.microsoft.com/en-us/library/dd183539(VS.85).aspx
        /// </summary>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Override the OnInitialized method
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Loaded += new RoutedEventHandler(AnimatedGIFControl_Loaded);
            Unloaded += new RoutedEventHandler(AnimatedGIFControl_Unloaded);
            //IsVisibleChanged += new DependencyPropertyChangedEventHandler(AnimatedGIFControl_IsVisibleChanged);

           // GetWindow().StateChanged += AnimatedGIFControl_WindowStateChanged;
        }

        private Window GetWindow()
        {
            FrameworkElement el = this;
            do
            {
                el = el.Parent as FrameworkElement;

            } while (!(el is Window));
            
            return el as Window;
        }

        private void AnimatedGIFControl_WindowStateChanged(object sender, EventArgs e)
        {
            if (GetWindow().WindowState == WindowState.Minimized)
            {
                StopAnimate();
            }
            else
            {
                StartAnimate();
            }
        }

        /// <summary>
        /// Load the embedded image for the Image.Source
        /// </summary>
        void AnimatedGIFControl_Loaded(object sender, RoutedEventArgs e)
        {
            //_bitmap = new Bitmap(@"C:\#\dancing-banana.gif");
            //_bitmap = new Bitmap(@"C:\#\tumblr_08f205da4d228695b84834c3fc491f72_9e5fea94_540.gif");
            //_bitmap = new Bitmap(@"C:\#\85755769624_()_tumblr_n54eohQ6Ei1smjc75o1_400.gif");
            _bitmap = new Bitmap(@"C:\#\664572310349594624_()_8e225a5ea0e25445ea748a45bfd8c94e1539dbf4.gif");
            
            //using (WebP webp = new WebP())
            //    _bitmap = webp.Load(@"C:\#\85755769624_()_tumblr_n54eohQ6Ei1smjc75o1_400.gif.lossless100.webp");

            if (_bitmap != null)
            {
                _bitmapSource = GetBitmapSource();

                double factor = _bitmapSource.Width / _bitmapSource.Height;

                var parentWidth = ((FrameworkElement)((FrameworkElement)sender).Parent).ActualWidth;
                var parentHeight = ((FrameworkElement)((FrameworkElement)sender).Parent).ActualHeight;

                if (factor > 1)
                {
                    Width = Math.Min(parentWidth, _bitmapSource.Width);
                    Height = Width / factor;
                }
                else
                {
                    Height = Math.Min(parentHeight, _bitmapSource.Height);
                    Width = Height * factor;
                }
                Thickness tn = Margin;
                if (parentHeight - Height > 0)
                {
                    tn.Top = (parentHeight - Height) / 2;
                }
                if (parentWidth - Width > 0)
                {
                    tn.Left = (parentWidth - Width) / 2;
                }
                Margin = tn;

                InvalidateVisual();

                StartAnimate();
            }
        }

        /// <summary>
        /// Close the FileStream to unlock the GIF file
        /// </summary>
        private void AnimatedGIFControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopAnimate();
        }

        /// <summary>
        /// Start animation
        /// </summary>
        private void StartAnimate()
        {
            if (_started) { return; }
            _started = true;

            ImageAnimator.Animate(_bitmap, OnFrameChanged);
        }

        /// <summary>
        /// Stop animation
        /// </summary>
        private void StopAnimate()
        {
            if (!_started) { return; }
            _started = false;

            ImageAnimator.StopAnimate(_bitmap, OnFrameChanged);
        }

        /// <summary>
        /// Event handler for the frame changed
        /// </summary>
        private void OnFrameChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FrameUpdatedEventHandler(FrameUpdatedCallback));
        }

        private void FrameUpdatedCallback()
        {
            ImageAnimator.UpdateFrames();

            if (_bitmapSource != null)
            {
                _bitmapSource.Freeze();
            }

            // Convert the bitmap to BitmapSource that can be display in WPF Visual Tree
            _bitmapSource = GetBitmapSource();
            InvalidateVisual();
        }

        private BitmapSource GetBitmapSource()
        {
            IntPtr handle = IntPtr.Zero;

            try
            {
                handle = _bitmap.GetHbitmap();
                _bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    DeleteObject(handle);
                }
            }

            return _bitmapSource;
        }

    }

}
