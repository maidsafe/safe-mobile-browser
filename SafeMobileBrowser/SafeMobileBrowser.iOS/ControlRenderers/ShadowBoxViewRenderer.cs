// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.iOS.ControlRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ShadowBoxView), typeof(ShadowBoxViewRenderer))]

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class ShadowBoxViewRenderer : VisualElementRenderer<ShadowBoxView>
    {
        private static readonly Dictionary<int, (Point startPoint, Point endPoint)> GradientPointsByAngle =
            new Dictionary<int, (Point startPoint, Point endPoint)>();

        private CGSize _previousSize;

        public static (Point startPoint, Point endPoint) RadiusGradientToPoints(int angle)
        {
            if (GradientPointsByAngle.TryGetValue(angle, out (Point startPoint, Point endPoint) points))
            {
                return points;
            }

            double anglePercent = angle / 360.0f;
            double a = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.75) / 2)), 2);
            double b = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.0) / 2)), 2);
            double c = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.25) / 2)), 2);
            double d = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.5) / 2)), 2);

            var result = (new Point(a, b), new Point(c, d));
            GradientPointsByAngle.Add(angle, result);

            return result;
        }

        public override void LayoutSubviews()
        {
            if (_previousSize != Bounds.Size)
            {
                SetNeedsDisplay();
            }

            base.LayoutSubviews();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (NativeView.Layer.Sublayers != null
                && NativeView.Layer.Sublayers.Length > 0
                && NativeView.Layer.Sublayers[0] is CAGradientLayer)
            {
                return;
            }

            if (Element.ShadowType != ShadowType.Top && Element.ShadowType != ShadowType.Bottom)
            {
                return;
            }

            var gradientsPoints = RadiusGradientToPoints(180);

            // Top shadow
            var startColor = Color.FromHex("30000000");
            var endColor = Color.FromHex("10ffffff");
            if (Element.ShadowType == ShadowType.Bottom)
            {
                var tmpColor = startColor;
                startColor = endColor;
                endColor = tmpColor;
            }

            var gradientLayer = new CAGradientLayer
            {
                Frame = rect,
                StartPoint = gradientsPoints.startPoint.ToPointF(),
                EndPoint = gradientsPoints.endPoint.ToPointF(),
                Colors = new[]
                {
                    startColor.ToCGColor(),
                    endColor.ToCGColor(),
                },
            };

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
            _previousSize = Bounds.Size;
        }
    }
}
