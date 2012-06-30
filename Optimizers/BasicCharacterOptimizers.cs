using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge;

namespace MZXImageResampler.Optimizers
{
    public static class BasicCharacterOptimizers
    {
        public static Character[,] LastCharset;
        public static int IntArg;

        #region Picks out characters that are within N pixels difference
        public static void PixelDifference(ref Character[,] charSet, int difference)
        {
            IntArg = difference;
            LastCharset = charSet;
            Parallel.For(0, charSet.GetLength(0) * charSet.GetLength(1), P_PixelDifference);
            charSet = LastCharset;
        }
        private static void P_PixelDifference(int lowerBound)
        {
            var needle = LastCharset[lowerBound / LastCharset.GetLength(0), lowerBound % LastCharset.GetLength(1)];
            var totalLength = LastCharset.GetLength(0) * LastCharset.GetLength(1);

            for (int i = lowerBound; i < totalLength; i++)
            {
                var p = GetPositionality(i);
                if (LastCharset[(int)p.X, (int)p.Y].Differences(needle, IntArg) < IntArg)
                    LastCharset[(int)p.X, (int)p.Y] = needle;
            }
        }
        #endregion

        private static Point GetPositionality(int index)
        {
            int thisRow = index / LastCharset.GetLength(1);
            int thisCol = index % LastCharset.GetLength(1);
            return new Point(thisRow, thisCol);
        }
    }
}
