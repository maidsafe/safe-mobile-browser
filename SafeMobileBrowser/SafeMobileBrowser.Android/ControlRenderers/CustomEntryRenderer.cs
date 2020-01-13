// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

﻿using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AddressBarEntry), typeof(CustomEntryRenderer))]

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            var gd = new GradientDrawable();
            gd.SetColor(global::Android.Graphics.Color.Transparent);
            Control.SetBackground(gd);
            Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
            Control.SetPadding(Control.PaddingLeft, 0, Control.PaddingRight, 0);
        }
    }
}
