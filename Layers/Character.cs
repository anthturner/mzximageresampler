using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MZXImageResampler
{
    public class Character
    {
        public Color Foreground = Color.White;
        public Color Background = Color.Black;

        public Color Color;
        public int ID;

        private Image CachedImage;
        private bool RegenerateImage = true;
        private byte[] CachedChar;
        private bool RegenerateChar = true;
        private string _hash;

        private bool[,] CharPositions = new bool[8, 14];

        private Dictionary<int, int> quadrantCache = new Dictionary<int, int>();

        private object Padlock = new object();

        #region Manipulation functions
        public void On(int x, int y)
        {
            SetVal(x, y, true);
        }

        public void Off(int x, int y)
        {
            SetVal(x, y, false);
        }
        
        private void SetVal(int x, int y, bool val)
        {
            if (x < 0 || y < 0)
                return;
            if (x > 7 || y > 13)
                return;
            RegenerateImage = true;
            RegenerateChar = true;
            CharPositions[x, y] = val;
            quadrantCache.Clear();
        }
        #endregion

        public bool ValueAt(int x, int y)
        {
            return CharPositions[x, y];
        }

        public void SetColors(Color fg, Color bg)
        {
            RegenerateImage = true;
            Foreground = fg;
            Background = bg;
        }

        public Image Display
        {
            get
            {
                if (!RegenerateImage && CachedImage != null)
                    return CachedImage;

                Bitmap newImage = new Bitmap(8 * Tuning.CharacterExpansion, 14 * Tuning.CharacterExpansion);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.FillRectangle(new SolidBrush(Color.Black), 0, 0, newImage.Width, newImage.Height);
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 14; j++)
                        {
                            var thisChar = ValueAt(i, j);
                            if (!thisChar)
                                g.FillRectangle(new SolidBrush(Color.White), i * Tuning.CharacterExpansion, j * Tuning.CharacterExpansion, Tuning.CharacterExpansion, Tuning.CharacterExpansion);
                        }
                }
                CachedImage = newImage;
                return newImage;
            }
        }

        public bool Match(Character c)
        {
            if (c.MZXCharacter == MZXCharacter)
                return true;
            return false;
        }

        public bool Inverse(Character c)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 14; j++)
                    if (c.CharPositions[i, j] == CharPositions[i, j])
                        return false;
            return true;
        }

        public byte[] MZXCharacter
        {
            get
            {
                if (RegenerateChar || CachedChar.Length == 0)
                {
                    string bitString = "";
                    for (int j = 0; j < 14; j++)
                        for (int i = 0; i < 8; i++)
                            if (CharPositions[i, j])
                                bitString += "0";
                            else
                                bitString += "1";
                    CachedChar = Enumerable.Range(0, bitString.Length / 8).
                        Select(pos => Convert.ToByte(bitString.Substring(pos*8, 8), 2)).ToArray();
                    _hash = Tuning.StringValueOfByte(CachedChar, "");
                }
                return CachedChar;
            }
        }

        public string Hash
        {
            get
            {
                var pullHash = MZXCharacter;
                return _hash;
            }
        }

        public int Differences(Character c, int abortAt)
        {
            int Differences = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 14; j++)
                    if (c.CharPositions[i, j] != CharPositions[i, j])
                    {
                        Differences++;
                        if (Differences >= abortAt)
                            return abortAt;
                    }
            return Differences;
        }

        public override string ToString()
        {
            return "ID: " + ID + " / Hash: " + Hash;
        }
    }
}
