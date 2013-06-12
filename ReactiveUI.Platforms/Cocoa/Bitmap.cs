using System;
using System.Threading.Tasks;
using System.IO;
using MonoTouch.UIKit;
using System.Threading;
using MonoTouch.Foundation;
using System.Reactive.Linq;
using System.Reactive;

namespace ReactiveUI.Cocoa
{
    class BitmapLoader : IBitmapLoader
    {
        public IObservable<IBitmap> Load(Stream sourceStream, float? desiredWidth, float? desiredHeight)
        {
            return Observable.Start(() => {
                var data = NSData.FromStream(sourceStream);
                return (IBitmap) new CocoaBitmap(UIImage.LoadFromData(data));
            }, RxApp.TaskpoolScheduler);
        }
        
        public IBitmap Create(float width, float height)
        {
            throw new NotImplementedException();
        }
    }
    
    sealed class CocoaBitmap : IBitmap
    {
        internal UIImage inner;
        public CocoaBitmap(UIImage inner)
        {
            this.inner = inner;
        }
        
        public float Width {
            get { return inner.Size.Width; }
        }
        
        public float Height {
            get { return inner.Size.Height; }
        }

        public IObservable<Unit> Save(CompressedBitmapFormat format, float quality, Stream target)
        {
            return Observable.Start(() => {
                var data = format == CompressedBitmapFormat.Jpeg ? inner.AsJPEG((float)quality) : inner.AsPNG();
                data.AsStream().CopyTo(target);
            }, RxApp.TaskpoolScheduler);
        }
        
        public void Dispose()
        {
            var disp = Interlocked.Exchange(ref inner, null);
            if (disp != null) disp.Dispose();
        }
    }
    
    public static class BitmapMixins
    {
        public static UIImage ToNative(this IBitmap This)
        {
            return ((CocoaBitmap)This).inner;
        }
        
        public static IBitmap FromNative(this UIImage This, bool copy = false)
        {
            if (copy) return new CocoaBitmap((UIImage)This.Copy());

            return new CocoaBitmap(This);
        }
    }
}


