// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
