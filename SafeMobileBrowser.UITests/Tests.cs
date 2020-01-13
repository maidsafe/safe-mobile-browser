// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace SafeMobileBrowser.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        private IApp _app;
        private Platform _platform;

        private const string _connectionErrorMsg = "Could not connect to the SAFE Network. Try updating your IP address on invite server.";
        private static readonly Func<AppQuery, AppQuery> _addressBarEntry = c => c.Marked("AddressBarEntry");
        private static readonly Func<AppQuery, AppQuery> _connectionFailedMsg = c => c.Marked(_connectionErrorMsg);
        private static readonly Func<AppQuery, AppQuery> _focusActionIcon = c => c.Marked("FocusActionIcon");
        private static readonly Func<AppQuery, AppQuery> _backActionIcon = c => c.Marked("BackActionIcon");
        private static readonly Func<AppQuery, AppQuery> _forwardActionIcon = c => c.Marked("ForwardActionIcon");
        private static readonly Func<AppQuery, AppQuery> _homeActionIcon = c => c.Marked("HomeActionIcon");
        private static readonly Func<AppQuery, AppQuery> _menuActionIcon = c => c.Marked("MenuActionIcon");
        private static readonly Func<AppQuery, AppQuery> _settingsPageIcon = c => c.Marked("Settings");
        private static readonly Func<AppQuery, AppQuery> _faqLabel = c => c.Marked("FAQs");
        private static readonly Func<AppQuery, AppQuery> _privacyStatementLabel = c => c.Marked("Privacy statement");
        private static readonly Func<AppQuery, AppQuery> _appDarkModeSwitch = c => c.Marked("AppDarkModeSwitch");


        public Tests(Platform platform)
        {
            this._platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _app = AppInitializer.StartApp(_platform);
        }

        [Test]
        public void AfterWhiteListingIPTest()
        {
            Thread.Sleep(5000);
            var connectionResult = _app.Query(_connectionFailedMsg);
            Assert.Throws<IndexOutOfRangeException>(
                            () => { var text = connectionResult[0].Text; });
        }

        [Test]
        public void LaunchTest()
        {
            _app.Tap(_focusActionIcon);
            _app.Tap(_addressBarEntry);
            _app.EnterText("hello");
            _app.PressEnter();
            Thread.Sleep(5000);
            _app.Screenshot("Opened hello page");
            _app.Tap(_backActionIcon);
            Thread.Sleep(5000);
            _app.Tap(_forwardActionIcon);
            Thread.Sleep(5000);
            _app.Tap(_homeActionIcon);
            _app.Screenshot("Home page");
        }


        [Test]
        public void SwitchAppModeTest()
        {
            _app.Tap(_menuActionIcon);
            _app.Tap(_settingsPageIcon);
            _app.Tap(_appDarkModeSwitch);
            _app.Screenshot("Dark mode enabled");
        }

        [Test]
        public void FAQsPageTest()
        {
            _app.Tap(_menuActionIcon);
            _app.Tap(_settingsPageIcon);
            _app.Tap(_faqLabel);
            _app.Screenshot("FAQ Page");
        }

        [Test]
        public void PrivacyPageTest()
        {
            _app.Tap(_menuActionIcon);
            _app.Tap(_settingsPageIcon);
            _app.Tap(_privacyStatementLabel);
            _app.Screenshot("Privacy Page");
        }
    }
}
