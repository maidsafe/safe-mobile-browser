using System;
using System.Linq;
using Xamarin.Forms;

namespace SafeMobileBrowser.Helpers
{
    public static class RandomGenerators
    {
        static readonly Random random = new Random();

        // Used to generate random string of any length
        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static Color GenerateRandomColor()
        {
            return Color.FromRgb(random.Next(255), random.Next(255), random.Next(255));
        }
    }
}
