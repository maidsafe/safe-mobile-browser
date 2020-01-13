// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Concurrent;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using AApplication = Android.App.Application;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public static class FontExtensions
    {
        static readonly ConcurrentDictionary<Tuple<string, FontAttributes>, Typeface> Typefaces =
            new ConcurrentDictionary<Tuple<string, FontAttributes>, Typeface>();

        static Typeface _sDefaultTypeface;

        public static float ToScaledPixel(this Font self)
        {
            if (self.IsDefault)
                return 14;

            if (self.UseNamedSize)
            {
                switch (self.NamedSize)
                {
                    case NamedSize.Micro:
                        return 10;

                    case NamedSize.Small:
                        return 12;

                    case NamedSize.Default:
                    case NamedSize.Medium:
                        return 14;

                    case NamedSize.Large:
                        return 18;
                }
            }

            return (float)self.FontSize;
        }

        internal static Typeface ToTypeFace(this string fontfamily, FontAttributes attr = FontAttributes.None)
        {
            fontfamily = fontfamily ?? string.Empty;
            return ToTypeface(fontfamily, attr);
        }

        public static Typeface ToTypeface(this Font self)
        {
            if (self.IsDefault || (self.FontAttributes == FontAttributes.None && string.IsNullOrEmpty(self.FontFamily)))
                return _sDefaultTypeface ?? (_sDefaultTypeface = Typeface.Default);

            return ToTypeface(self.FontFamily, self.FontAttributes);
        }

        internal static bool IsDefault(this IFontElement self)
        {
            return self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) && self.FontAttributes == FontAttributes.None;
        }

        static bool IsAssetFontFamily(string name)
        {
            return name != null && (name.Contains(".ttf#") || name.Contains(".otf#"));
        }

        internal static Typeface ToTypeface(this IFontElement self)
        {
            if (self.IsDefault())
                return _sDefaultTypeface ?? (_sDefaultTypeface = Typeface.Default);

            return ToTypeface(self.FontFamily, self.FontAttributes);
        }

        static Typeface ToTypeface(string fontFamily, FontAttributes fontAttributes)
        {
            fontFamily = fontFamily ?? string.Empty;
            return Typefaces.GetOrAdd(new Tuple<string, FontAttributes>(fontFamily, fontAttributes), CreateTypeface);
        }

        static Typeface CreateTypeface(Tuple<string, FontAttributes> key)
        {
            Typeface result;
            var fontFamily = key.Item1;
            var fontAttribute = key.Item2;

            if (fontFamily == null)
            {
                var style = ToTypefaceStyle(fontAttribute);
                result = Typeface.Create(Typeface.Default, style);
            }
            else if (IsAssetFontFamily(fontFamily))
            {
                result = Typeface.CreateFromAsset(AApplication.Context.Assets, FontNameToFontFile(fontFamily));
            }
            else
            {
                var style = ToTypefaceStyle(fontAttribute);
                result = Typeface.Create(fontFamily, style);
            }

            return result;
        }

        public static TypefaceStyle ToTypefaceStyle(FontAttributes attrs)
        {
            var style = TypefaceStyle.Normal;
            if ((attrs & (FontAttributes.Bold | FontAttributes.Italic)) == (FontAttributes.Bold | FontAttributes.Italic))
                style = TypefaceStyle.BoldItalic;
            else if ((attrs & FontAttributes.Bold) != 0)
                style = TypefaceStyle.Bold;
            else if ((attrs & FontAttributes.Italic) != 0)
                style = TypefaceStyle.Italic;
            return style;
        }

        static string FontNameToFontFile(string fontFamily)
        {
            fontFamily = fontFamily ?? string.Empty;
            int hashtagIndex = fontFamily.IndexOf('#');
            if (hashtagIndex >= 0)
                return fontFamily.Substring(0, hashtagIndex);

            throw new InvalidOperationException($"Can't parse the {nameof(fontFamily)} {fontFamily}");
        }
    }
}
