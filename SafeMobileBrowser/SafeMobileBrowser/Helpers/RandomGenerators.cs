using System;
using System.Linq;
using Xamarin.Forms;

namespace SafeMobileBrowser.Helpers
{
    public static class RandomGenerators
    {
        static readonly Random Random = new Random();

        // Used to generate random string of any length
        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static Color GenerateRandomColor()
        {
            return Color.FromRgb(Random.Next(255), Random.Next(255), Random.Next(255));
        }
    }
}
