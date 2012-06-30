using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZXImageResampler
{
    public static class Tuning
    {
        public static int NumQuadrants = 2;
        public static int MaxDensityDifferential = 1;
        public static int GraphicalExpansion = 2;
        public static int CharacterExpansion = 1;
        public static int PixelDifference = 8;

        internal static string StringValueOfByte(byte[] data, string delimiter)
        {
            if (data == null || data.Length == 0) { return ""; }
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                if (sb.Length > 0) { sb.Append(delimiter); }
                sb.Append(String.Format("{0:X2}", t));
            }
            return sb.ToString();
        }
    }
}
