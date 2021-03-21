using System.Collections.Generic;
using System.Drawing;

using OpenCL.NET.Memory;

namespace OpenCL.Wrapper.CLFonts
{
    public class CLFont
    {

        private readonly Dictionary<char, CLChar> bufferCache = new Dictionary<char, CLChar>();
        public readonly Font Font;

        public CLFont(Font font)
        {
            Font = font;
        }

        private int Ascend => Font.FontFamily.GetCellAscent(FontStyle.Regular);

        private int Descend => Font.FontFamily.GetCellDescent(FontStyle.Regular);

        private int FontHeight => Ascend + Descend;

        public string Name => Font.Name;

        public CLChar LoadCharBuffer(CLAPI instance, char character)
        {
            if (bufferCache.ContainsKey(character))
            {
                return bufferCache[character];
            }

            Bitmap bmp = RenderToBitmap(character, out Size charSize);
            MemoryBuffer buf = CLAPI.CreateFromImage(instance, bmp, MemoryFlag.ReadOnly, Name + "_" + character);
            CLChar ret = new CLChar(character, buf, charSize.Width, charSize.Height);
            bufferCache[character] = ret;
            return ret;
        }


        private Bitmap RenderToBitmap(char character, out Size charSize)
        {
            Bitmap ret = new Bitmap((int) Font.Size, (int) Font.Size);
            Graphics g = Graphics.FromImage(ret);
            SizeF charwidth = g.MeasureString(character.ToString(), Font);
            ret = new Bitmap((int) charwidth.Width, (int) charwidth.Height);
            g.DrawString(character.ToString(), Font, Brushes.Black, 0, 0);
            g.Flush();

            g.Dispose();

            charSize = new Size((int) charwidth.Width, (int) charwidth.Height);

            return ret;
        }

    }
}