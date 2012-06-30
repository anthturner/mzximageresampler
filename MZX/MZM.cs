using System;
using System.Collections.Generic;
using System.Text;

namespace MZXImageResampler.MZX
{
    public class MZM
    {
        private Character[,] RawInput;
        private CHR Charset;

        public byte[] RawMZMFile;

        public MZM(Character[,] raw, int offset=0, bool encodeCharacters=false)
        {
            RawInput = raw;
            Charset = new CHR(ref raw, offset);

            var result = new List<byte>();
            #region MZM2 Header Magic

            result.AddRange(Encoding.ASCII.GetBytes("MZM2")); // Bytes 0-3
            result.AddRange(BitConverter.GetBytes((short)RawInput.GetLength(0))); // Bytes 4-5
            result.AddRange(BitConverter.GetBytes((short)RawInput.GetLength(1))); // Bytes 6-7
            result.AddRange(BitConverter.GetBytes((int)0)); // Bytes 8-11
            result.Add(0x00);
            result.Add(0x00);
            result.Add(0x00);
            result.Add(0x00);

            #endregion

            for (int i = 0; i < raw.GetLength(0); i++)
                for (int j = 0; j < raw.GetLength(1); j++)
                    result.AddRange(GetCharAt(i, j, encodeCharacters));
            RawMZMFile = result.ToArray();
        }

        public int NumChars {
            get { return Charset.CharSetSize; }
        }

        byte[] GetCharAt(int x, int y, bool encodeID)
        {
            try
            {
                var localChar = RawInput[x, y];
                var thisChar = new byte[6];
                thisChar[0] = 0x05; // CustomBlock
                if (encodeID)
                    thisChar[1] = Convert.ToByte(localChar.ID);
                thisChar[2] = 0x0f;
                thisChar[3] = 0x00;
                thisChar[4] = 0x00;
                thisChar[5] = 0x00;
                return thisChar;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
