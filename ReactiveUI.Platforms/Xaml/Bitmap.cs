using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ReactiveUI.Xaml.Xaml
{
    class BitmapLoader : IBitmapLoader
    {
        public IObservable<IBitmap> Load(Stream sourceStream, float? desiredWidth, float? desiredHeight)
        {
            return Observable.Start(() => {
                var source = new BitmapImage();

                source.BeginInit();
                source.StreamSource = sourceStream;
                if (desiredWidth != null) {
                    source.DecodePixelWidth = (int)desiredWidth;
                    source.DecodePixelHeight = (int)desiredHeight;
                }
                source.EndInit();
                source.Freeze();

                return new BitmapSourceBitmap(source);
            }, RxApp.TaskpoolScheduler);
        }

        public IBitmap Create(float width, float height)
        {
            return new BitmapSourceBitmap(new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Default, null));
        }
    }

    class BitmapSourceBitmap : IBitmap
    {
        BitmapSource inner;

        public float Width { get; protected set; }
        public float Height { get; protected set; }

        public BitmapSourceBitmap(BitmapSource bitmap)
        {
            inner = bitmap;
            Width = (float)inner.Width;
            Height = (float)inner.Height;
        }

        public IObservable<Unit> Save(CompressedBitmapFormat format, float quality, Stream target)
        {
            return Observable.Start(() => {
                var encoder = format == CompressedBitmapFormat.Jpeg ?
                    (BitmapEncoder)new JpegBitmapEncoder() { QualityLevel = (int)(quality * 100.0f) } :
                    (BitmapEncoder)new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(inner));
                encoder.Save(target);
            }, RxApp.TaskpoolScheduler);
        }

        public void Dispose()
        {
            inner = null;
        }
    }
}