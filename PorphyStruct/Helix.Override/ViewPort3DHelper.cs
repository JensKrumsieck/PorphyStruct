using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;


namespace PorphyStruct.Helix.Override
{
    public static class Viewport3DHelper
    {
        /// <summary>
        /// Renders the viewport to a bitmap.
        /// </summary>
        /// <param name="view">The viewport.</param>
        /// <param name="background">The background.</param>
        /// <param name="m">The oversampling multiplier.</param>
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(this Viewport3D view, Brush background, int m = 1)
        {
            var target = new WriteableBitmap((int)view.ActualWidth * m, (int)view.ActualHeight * m, 300, 300, PixelFormats.Pbgra32, null);

            var originalCamera = view.Camera;
            var vm = originalCamera.GetViewMatrix();
            double ar = view.ActualWidth / view.ActualHeight;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    // change the camera viewport and scaling
                    var pm = originalCamera.GetProjectionMatrix(ar);
                    if (originalCamera is OrthographicCamera)
                    {
                        pm.OffsetX = m - 1 - (i * 2);
                        pm.OffsetY = -(m - 1 - (j * 2));
                    }

                    if (originalCamera is PerspectiveCamera)
                    {
                        pm.M31 = -(m - 1 - (i * 2));
                        pm.M32 = m - 1 - (j * 2);
                    }

                    pm.M11 *= m;
                    pm.M22 *= m;

                    var mc = new MatrixCamera(vm, pm);
                    view.Camera = mc;

                    var partialBitmap = new RenderTargetBitmap((int)view.ActualWidth, (int)view.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                    // render background
                    var backgroundRectangle = new Rectangle { Width = partialBitmap.Width, Height = partialBitmap.Height, Fill = background };
                    backgroundRectangle.Arrange(new Rect(0, 0, backgroundRectangle.Width, backgroundRectangle.Height));
                    partialBitmap.Render(backgroundRectangle);

                    // render 3d
                    partialBitmap.Render(view);

                    // copy to the target bitmap
                    CopyBitmap(partialBitmap, target, (int)(i * view.ActualWidth), (int)(j * view.ActualHeight));
                }
            }

            // restore the camera
            view.Camera = originalCamera;
            return target;
        }

        /// <summary>
        /// Renders the viewport to a bitmap.
        /// </summary>
        /// <param name="view">The viewport.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="background">The background.</param>
        /// <param name="m">The oversampling multiplier.</param>
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(this Viewport3D view, double width, double height, Brush background, int m = 1)
        {
            double w = view.Width;
            double h = view.Height;
            HelixToolkit.Wpf.Viewport3DHelper.ResizeAndArrange(view, width, height);
            var rtb = RenderBitmap(view, background, m);
            HelixToolkit.Wpf.Viewport3DHelper.ResizeAndArrange(view, w, h);
            return rtb;
        }

        /// <summary>
        /// Copies the bitmap.
        /// </summary>
        /// <param name="source">The source bitmap.</param>
        /// <param name="target">The target bitmap.</param>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        private static void CopyBitmap(BitmapSource source, WriteableBitmap target, int x, int y)
        {
            // Calculate stride of source
            int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);

            // Create data array to hold source pixel data
            var data = new byte[stride * source.PixelHeight];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // Write the pixel data to the bitmap.
            target.WritePixels(new Int32Rect(x, y, source.PixelWidth, source.PixelHeight), data, stride, 0);
        }

    }
}
