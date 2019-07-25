using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class LinearProgressBar : UIView
    {
        private static CGRect _screenSize = UIScreen.MainScreen.Bounds;

        private readonly UIView _progressBarIndicator;

        private bool _isAnimationRunning;

        public UIColor BackGroundProgressBarColor { get; set; } = UIColor.White;

        public UIColor ProgressBarColor { get; set; } = UIColor.FromRGB(33, 150, 243);

        public nfloat HeightForLinearBar { get; set; } = 5;

        public nfloat WidthForLinearBar { get; set; } = 0;

        public LinearProgressBar()
            : base(new CGRect(0, 20, _screenSize.Width, 0))
        {
            _progressBarIndicator = new UIView(new CGRect(0, 0, 0, HeightForLinearBar));
        }

        public LinearProgressBar(CGRect frame)
            : base(frame)
        {
            _progressBarIndicator = new UIView(new CGRect(0, 0, 0, HeightForLinearBar));
        }

        public LinearProgressBar(NSCoder nScoder)
        {
            throw new NotImplementedException("constructor(coder) has not implemented");
        }

        // Creating View
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _screenSize = UIScreen.MainScreen.Bounds;

            if (WidthForLinearBar == 0 || WidthForLinearBar == _screenSize.Height)
            {
                WidthForLinearBar = _screenSize.Width;
            }

            if (UIDevice.CurrentDevice.Orientation.IsLandscape())
            {
                Frame = new CGRect(Frame.X, Frame.Y, WidthForLinearBar, Frame.Height);
            }

            if (UIDevice.CurrentDevice.Orientation.IsPortrait())
            {
                Frame = new CGRect(Frame.X, Frame.Y, WidthForLinearBar, Frame.Height);
            }
        }

        public void StartAnimation()
        {
            ConfigureColors();

            if (!_isAnimationRunning)
            {
                _isAnimationRunning = true;
                Animate(
                    0.5,
                    0,
                    UIViewAnimationOptions.TransitionFlipFromLeft,
                    () =>
                    {
                        Frame = new CGRect(0, Frame.Y, WidthForLinearBar, HeightForLinearBar);
                    },
                    () =>
                    {
                        AddSubview(_progressBarIndicator);
                        ConfigureAnimation();
                    });
            }
        }

        public void StopAnimation()
        {
            _isAnimationRunning = false;

            Animate(0.5, () =>
            {
                _progressBarIndicator.Frame = new CGRect(0, 0, WidthForLinearBar, 0);
                Frame = new CGRect(0, Frame.Y, WidthForLinearBar, 0);
            });
        }

        private void ConfigureColors()
        {
            _progressBarIndicator.BackgroundColor = ProgressBarColor;
        }

        private void ConfigureAnimation()
        {
            _progressBarIndicator.Frame = new CGRect(0, 0, 0, HeightForLinearBar);

            Animate(
                0.5,
                0,
                UIViewAnimationOptions.TransitionFlipFromLeft,
                () =>
                {
                    _progressBarIndicator.Frame = new CGRect(0, 0, WidthForLinearBar * 0.7, HeightForLinearBar);
                },
                null);

            Animate(
                0.4,
                0.4,
                UIViewAnimationOptions.TransitionFlipFromLeft,
                () =>
                {
                    _progressBarIndicator.Frame = new CGRect(Frame.Width, 0, 0, HeightForLinearBar);
                },
                () =>
                {
                    if (_isAnimationRunning)
                        ConfigureAnimation();
                });
        }
    }
}
