using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MZXImageResampler
{
    public static class WorkingImage
    {
        public static EnhancedBitmap BaseImage;
        public static Character[,] CharacterImage;
        public static PaletteEntry[,] PaletteImage;

        public static DitheringEngine Ditherer = DitheringEngine.Custom;
        public static int CharacterX, CharacterY;
        private static int PixelX, PixelY;

        public static void Init(string fileName)
        {
            PixelX = CharacterX*8;
            PixelY = CharacterY*14;
            ImageImport.SourceFile = fileName;
            ImageImport.X = PixelX;
            ImageImport.Y = PixelY;

            BaseImage = new EnhancedBitmap(ImageImport.GetByDitherer(Ditherer));
            CharacterImage = PixelatedImage;
        }

        public static Bitmap UIImage
        {
            get
            {
                var pixels = CharacterImage;

                Bitmap bmp = new Bitmap(pixels.GetLength(0) * 8 * Tuning.CharacterExpansion, pixels.GetLength(1) * 14 * Tuning.CharacterExpansion);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < pixels.GetLength(0); i++)
                        for (int j = 0; j < pixels.GetLength(1); j++)
                            g.DrawImage(pixels[i, j].Display, i*8*Tuning.CharacterExpansion, j*14*Tuning.CharacterExpansion);
                }
                return bmp;
            }
        }

        public static Character[,] PixelatedImage
        {
            get
            {
                var thisImage = new Character[CharacterX, CharacterY];

                BaseImage.LockImage();
                for (int i = 0; i < CharacterX; i++)
                    for (int j = 0; j < CharacterY; j++)
                    {
                        thisImage[i, j] = SliceAt(i, j);
                    }
                BaseImage.UnlockImage();

                return thisImage;
            }
        }

        static Character SliceAt(int x, int y)
        {
            var thisChar = new Character();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 14; j++)
                {
                    var thisColor = BaseImage.GetPixel((x * 8) + i, (y * 14) + j);
                    if (thisColor.GetBrightness() < .5)
                        thisChar.Off(i, j);
                    else
                        thisChar.On(i, j);
                }

            return thisChar;
        }
    }
}
