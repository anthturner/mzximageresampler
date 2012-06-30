using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZXImageResampler.MZX
{
    public class CHR
    {
        public byte[] RawCHRFile;
        public int CharSetSize;
        private object Padlock = new object();
        private List<string> _usedChars = new List<string>();
        private List<Character> _charSet = new List<Character>();

        public CHR(ref Character[,] image, int offset)
        {
            var searchable = image; // deref
            _charSet = new List<Character>();
            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    if (_usedChars.Find(b => b.Equals(searchable[i, j].Hash)) == null)
                    {
                        _usedChars.Add(image[i, j].Hash);
                        _charSet.Add(image[i, j]);
                    }

            CharSetSize = _charSet.Count;

            for (int i = 0; i < _charSet.Count; i++)
                _charSet[i].ID = i + offset;

            var chrFile = new List<byte>();
            foreach (Character c in _charSet)
                chrFile.AddRange(c.MZXCharacter);
            RawCHRFile = chrFile.ToArray();
        }
    }
}
