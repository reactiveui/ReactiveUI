using System;
using System.Threading.Tasks;
using System.IO;
using Android.Graphics;
using System.Threading;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI.Android
{
    public class BitmapLoader : IBitmapLoader
    {
        public IObservable<IBitmap> Load(Stream sourceStream, float? desiredWidth, float? desiredHeight)
        {
            return Observable.Start(() => 
                BitmapFactory.DecodeStream(sourceStream).FromNative(), RxApp.TaskpoolScheduler);
        }

        public IBitmap Create(float width, float height)
        {
            return Bitmap.CreateBitmap((int)width, (int)height, Bitmap.Config.Argb8888).FromNative();
        }
    }
            
    sealed class AndroidBitmap : IBitmap
    {
        internal Bitmap inner;
        public AndroidBitmap(Bitmap inner)
        {
            this.inner = inner;
        }
                    
        public float Width {
            get { return inner.Width; }
        }

        public float Height {
            get { return inner.Height; }
        }

        public IObservable<Unit> Save(CompressedBitmapFormat format, float quality, Stream target)
        {
            var fmt = format == CompressedBitmapFormat.Jpeg ? Bitmap.CompressFormat.Jpeg : Bitmap.CompressFormat.Png;
            return Observable.Start(() => { inner.Compress(fmt, (int)quality * 100, target); }, RxApp.TaskpoolScheduler);
        }

        public void Dispose()
        {
            var disp = Interlocked.Exchange(ref inner, null);
            if (disp != null) disp.Dispose();
        }
    }

    public static class BitmapMixins
    {
        public static Bitmap ToNative(this IBitmap This)
        {
            return ((AndroidBitmap)This).inner;
        }

        public static IBitmap FromNative(this Bitmap This, bool copy = false)
        {
            if (copy) return new AndroidBitmap(This.Copy(This.GetConfig(), true));
            return new AndroidBitmap(This);
        }
    }
}

