using System;
using System.Windows.Media;

namespace MovieList.Services
{
    public static class Util
    {
        public static Color IntToColor(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static int ColorToInt(Color color)
        {
            var bytes = new byte[] { color.B, color.G, color.R, color.A };
            return BitConverter.ToInt32(bytes);
        }
    }
}
