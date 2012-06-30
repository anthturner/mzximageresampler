using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;

namespace MZXImageResampler
{
    public enum DitheringEngine
    {
        FloydSteinberg,
        Sierra,
        Bayer,
        Burkes,
        JarvisJudiceNinke,
        Stucki,
        Custom
    };

    public static class ImageImport
    {
        public static string SourceFile;
        public static int X, Y;

        public static Bitmap GetByDitherer(DitheringEngine engine)
        {
            switch (engine)
            {
                case DitheringEngine.Bayer:
                    return Bayer1bpp;
                case DitheringEngine.Burkes:
                    return Burkes1bpp;
                case DitheringEngine.Custom:
                    return UniformFlatten(0.5);
                case DitheringEngine.FloydSteinberg:
                    return FloydSteinberg1bpp;
                case DitheringEngine.JarvisJudiceNinke:
                    return JarvisJudiceNinke1bpp;
                case DitheringEngine.Sierra:
                    return Sierra1bpp;
                case DitheringEngine.Stucki:
                    return Stucki1bpp;
            }
            return new Bitmap(1,1);
        }

        #region Floyd-Steinberg Error-Diffusion 1bpp Dithering
        public static Bitmap FloydSteinberg1bpp
        {
            get
            {
                var image = BaseResized;
                var dithering = new FloydSteinbergDithering();
                var newImage = dithering.Apply(image);
                return newImage;
            }
        }
        #endregion
        #region Sierra Error-Diffusion 1bpp Dithering
        public static Bitmap Sierra1bpp
        {
            get
            {
                var image = BaseResized;
                var dithering = new SierraDithering();
                var newImage = dithering.Apply(image);
                return newImage;
            }
        }
        #endregion
        #region Bayer Error-Diffusion 1bpp Dithering
        public static Bitmap Bayer1bpp
        {
            get
            {
                var image = BaseResized;
                var dithering = new BayerDithering();
                var newImage = dithering.Apply(image);
                return newImage;
            }
        }
        #endregion
        #region Burkes Error-Diffusion 1bpp Dithering
        public static Bitmap Burkes1bpp
        {
            get
            {
                var image = BaseResized;
                var dithering = new BurkesDithering();
                var newImage = dithering.Apply(image);
                return newImage;
            }
        }
        #endregion
        #region Jarvis-Judice-Ninke Error-Diffusion 1bpp Dithering
        public static Bitmap JarvisJudiceNinke1bpp
        {
            get
            {
                var image = BaseResized;
                var dithering = new JarvisJudiceNinkeDithering();
                var newImage = dithering.Apply(image);
                return newImage;
            }
        }
        #endregion
        #region Stucki Error-Diffusion 1bpp Dithering
        public static Bitmap Stucki1bpp
        {
            get
            {
                var image = BaseResized;
                var dithering = new StuckiDithering();
                var newImage = dithering.Apply(image);
                return newImage;
            }
        }
        #endregion

        #region Custom Flattener
        public static Bitmap UniformFlatten(double percentage=0.5)
        {
            var image = new EnhancedBitmap(BaseResized);
            var newImage = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                image.LockImage();
                for (int i = 0; i < image.Width; i++)
                    for (int j = 0; j < image.Height; j++)
                    {
                        var px = image.GetPixel(i, j);
                        if (px.GetBrightness() > percentage)
                            g.DrawRectangle(Pens.White, i, j, 1, 1);
                        else
                            g.DrawRectangle(Pens.Black, i, j, 1, 1);
                    }
                image.UnlockImage();
            }
            return newImage;
        }
        #endregion

        #region PREREQUISITE - Image Resize
        internal static Bitmap BaseResized
        {
            get
            {
                var sourceMaster = new Bitmap(SourceFile);
                var newImage = new Bitmap(X, Y);
                using (Graphics gr = Graphics.FromImage(newImage))
                {
                    gr.SmoothingMode = SmoothingMode.AntiAlias;
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(sourceMaster, new Rectangle(0, 0, X, Y));
                }
                return newImage;
            }
        }
        #endregion
    }
}
