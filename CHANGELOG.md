## 0.2.0 - (2019-09-05)

### Features

* Enable app dark theme :tada: features for Android and iOS (#175) ([25e744f](https://github.com/maidsafe/safe-mobile-browser/commit/25e744f)), closes [#175](https://github.com/maidsafe/safe-mobile-browser/issues/175) [#158](https://github.com/maidsafe/safe-mobile-browser/issues/158) [#158](https://github.com/maidsafe/safe-mobile-browser/issues/158)
* Update progress bar to show content load %age instead of indeterminate progress ([dc524c6](https://github.com/maidsafe/safe-mobile-browser/commit/dc524c6)), closes [#159](https://github.com/maidsafe/safe-mobile-browser/issues/159)
* Add image download context menu for Android (#170) ([538834b](https://github.com/maidsafe/safe-mobile-browser/commit/538834b)), closes [#170](https://github.com/maidsafe/safe-mobile-browser/issues/170)
* Enable back and forward navigation gestures on iOS (#177) ([f38d906](https://github.com/maidsafe/safe-mobile-browser/commit/f38d906)), closes [#177](https://github.com/maidsafe/safe-mobile-browser/issues/177)
* Enable landscape mode in iOS (#164) ([dfc8225](https://github.com/maidsafe/safe-mobile-browser/commit/dfc8225)), closes [#164](https://github.com/maidsafe/safe-mobile-browser/issues/164)
* Add feature to show, copy, & delete log files from settings page (#168) ([1ad61a2](https://github.com/maidsafe/safe-mobile-browser/commit/1ad61a2)), closes [#168](https://github.com/maidsafe/safe-mobile-browser/issues/168)

### Bug Fixes

* Update bookmarks page 'no bookmarks text' (#176) ([7aceb1f](https://github.com/maidsafe/safe-mobile-browser/commit/7aceb1f)), closes [#176](https://github.com/maidsafe/safe-mobile-browser/issues/176)
* Update info.plist to allow photo download from webpage on iOS devices (#160) ([e22e391](https://github.com/maidsafe/safe-mobile-browser/commit/e22e391)), closes [#160](https://github.com/maidsafe/safe-mobile-browser/issues/160)
* Fix loading of numeric websites on Android devices  (#174) ([313a54f](https://github.com/maidsafe/safe-mobile-browser/commit/313a54f)), closes [#174](https://github.com/maidsafe/safe-mobile-browser/issues/174)
* Refactored code to show error pages from webfetch (#155) ([c02b62e](https://github.com/maidsafe/safe-mobile-browser/commit/c02b62e)), closes [#155](https://github.com/maidsafe/safe-mobile-browser/issues/155)
* Fixed iOS settings page to open FAQ, and Privacy page links (#153) ([01b610d](https://github.com/maidsafe/safe-mobile-browser/commit/01b610d)), closes [#153](https://github.com/maidsafe/safe-mobile-browser/issues/153)

### Other Changes

* Update MaidSafe.SafeApp package to v0.2.3 (#183) ([8c1e3b4](https://github.com/maidsafe/safe-mobile-browser/commit/8c1e3b4)), closes [#183](https://github.com/maidsafe/safe-mobile-browser/issues/183)
* Code refactoring for uniformality (#178) ([3ade46d](https://github.com/maidsafe/safe-mobile-browser/commit/3ade46d)), closes [#178](https://github.com/maidsafe/safe-mobile-browser/issues/178)
* Remove authentication page and unnecessary default images (#162) ([8d8244c](https://github.com/maidsafe/safe-mobile-browser/commit/8d8244c)), closes [#162](https://github.com/maidsafe/safe-mobile-browser/issues/162)
* Update hybridwebview to use commands (#156) ([eabfb95](https://github.com/maidsafe/safe-mobile-browser/commit/eabfb95)), closes [#156](https://github.com/maidsafe/safe-mobile-browser/issues/156)
* Use Xamarin.Forms inbuild zoom feature (#165) ([c3322d0](https://github.com/maidsafe/safe-mobile-browser/commit/c3322d0)), closes [#165](https://github.com/maidsafe/safe-mobile-browser/issues/165)
* Update README to include AppCenter links (#179) ([95a112e](https://github.com/maidsafe/safe-mobile-browser/commit/95a112e)), closes [#179](https://github.com/maidsafe/safe-mobile-browser/issues/179)
* Update licensing to mention GPL3 (#166) ([5b2378b](https://github.com/maidsafe/safe-mobile-browser/commit/5b2378b)), closes [#166](https://github.com/maidsafe/safe-mobile-browser/issues/166)

### Known Issues

* Mobile browser (iOS) may not be able to connect to the SAFE Network when using cellular data.
* Changing the device network connection between whitelisted and non-whilelisted IP network may cause app to feeze.

## 0.1.1 - (2019-07-31)

### Features

* Open safe url in browser from any app in iOS (#145) ([76b2af7](https://github.com/ravinderjangra/safe-mobile-browser/commit/76b2af7)), closes [#145](https://github.com/ravinderjangra/safe-mobile-browser/issues/145)
* Zooming in and out in Android  (#142) ([dea9f98](https://github.com/ravinderjangra/safe-mobile-browser/commit/dea9f98)), closes [#142](https://github.com/ravinderjangra/safe-mobile-browser/issues/142)

### Bug Fixes

* Update webfetch to load content from url with long path ([e0a998b](https://github.com/ravinderjangra/safe-mobile-browser/commit/e0a998b))
* Share option in popup menu in iOS (#140) ([72c5aa3](https://github.com/ravinderjangra/safe-mobile-browser/commit/72c5aa3)), closes [#140](https://github.com/ravinderjangra/safe-mobile-browser/issues/140)
* Strip url schema from address bar (#136) ([0be4c4b](https://github.com/ravinderjangra/safe-mobile-browser/commit/0be4c4b)), closes [#136](https://github.com/ravinderjangra/safe-mobile-browser/issues/136)
* Check if the Sessions is not disconnected when fetching the web page (#148) ([b1c9584](https://github.com/ravinderjangra/safe-mobile-browser/commit/b1c9584)), closes [#148](https://github.com/ravinderjangra/safe-mobile-browser/issues/148)
* Refactor browser code using resharper (#132) ([1f78142](https://github.com/ravinderjangra/safe-mobile-browser/commit/1f78142)), closes [#132](https://github.com/ravinderjangra/safe-mobile-browser/issues/132)
* chore/browser: update MaidSafe.SafeApp package version (#138) ([5326e51](https://github.com/ravinderjangra/safe-mobile-browser/commit/5326e51)), closes [#138](https://github.com/ravinderjangra/safe-mobile-browser/issues/138)

### Known Issues

* Changing the device orientation while loaading the web page may cause app to freeze or crash.
* Changing the device network connection between whitelisted and non-whilelisted IP network may cause app to feeze.

## 0.1.0 - (2019-07-24)

* Integrate address bar (#25) ([8c0daa5](https://github.com/ravinderjangra/SafeMobileBrowser/commit/8c0daa5)), closes [#25](https://github.com/ravinderjangra/SafeMobileBrowser/issues/25)
* Add bottom-navbar and progress-bar ([ea63f0e](https://github.com/ravinderjangra/SafeMobileBrowser/commit/ea63f0e))
* Add advance progressbar to show progress when loading websites ([b929028](https://github.com/ravinderjangra/SafeMobileBrowser/commit/b929028))
* Add new README file with basic details ([2a90375](https://github.com/ravinderjangra/SafeMobileBrowser/commit/2a90375))
* Add new HTML welcome and error page (#27) ([5a27ca1](https://github.com/ravinderjangra/SafeMobileBrowser/commit/5a27ca1)), closes [#27](https://github.com/ravinderjangra/SafeMobileBrowser/issues/27)
* Add popup menu, bookmarks page and bookmarks manager ([b5ddbe5](https://github.com/ravinderjangra/SafeMobileBrowser/commit/b5ddbe5))
* Add refresh and bookmark functionality to popup menu (#22) ([57ac449](https://github.com/ravinderjangra/SafeMobileBrowser/commit/57ac449)), closes [#22](https://github.com/ravinderjangra/SafeMobileBrowser/issues/22)
* Add settings page controls (#24) ([1064f54](https://github.com/ravinderjangra/SafeMobileBrowser/commit/1064f54)), closes [#24](https://github.com/ravinderjangra/SafeMobileBrowser/issues/24)
* Add toast message on bookmark add and delete (#46) ([b2b7fea](https://github.com/ravinderjangra/SafeMobileBrowser/commit/b2b7fea)), closes [#46](https://github.com/ravinderjangra/SafeMobileBrowser/issues/46)
* Add touch effect/feedback on menu buttons (#53) ([46739bb](https://github.com/ravinderjangra/SafeMobileBrowser/commit/46739bb)), closes [#53](https://github.com/ravinderjangra/SafeMobileBrowser/issues/53)
* Update App Icons and Launch/Splash screens for Android and iOS (#21) ([493fec2](https://github.com/ravinderjangra/SafeMobileBrowser/commit/493fec2)), closes [#21](https://github.com/ravinderjangra/SafeMobileBrowser/issues/21)
* Bookmarks are fetched directly without opening bookmarks page and show loading dialogue  ([c86ea4b](https://github.com/ravinderjangra/SafeMobileBrowser/commit/c86ea4b)), closes [#44](https://github.com/ravinderjangra/SafeMobileBrowser/issues/44)
* Update popup menu and bottom bar item states (#28) ([dc35eb8](https://github.com/ravinderjangra/SafeMobileBrowser/commit/dc35eb8)), closes [#28](https://github.com/ravinderjangra/SafeMobileBrowser/issues/28)
* Update the session initialisation to show dialog on session failure (#38) ([53ca417](https://github.com/ravinderjangra/SafeMobileBrowser/commit/53ca417)), closes [#38](https://github.com/ravinderjangra/SafeMobileBrowser/issues/38)
* Add links for FAQ and privacy statement and initialise session fix (#113) ([e2c6827](https://github.com/ravinderjangra/SafeMobileBrowser/commit/e2c6827)), closes [#113](https://github.com/ravinderjangra/SafeMobileBrowser/issues/113)
* Use hardcoded auth response ([b78a538](https://github.com/ravinderjangra/SafeMobileBrowser/commit/b78a538))
* Add stylecop to enforce .Net Coding Standard ([1a06721](https://github.com/ravinderjangra/SafeMobileBrowser/commit/1a06721))
* Move webfetch to seperate directory to implement proper exceptions ([3f22d74](https://github.com/ravinderjangra/SafeMobileBrowser/commit/3f22d74))
* Add Logger Service and use single console logger helper to show console messages (#65) ([f15a8a1](https://github.com/ravinderjangra/SafeMobileBrowser/commit/f15a8a1)), closes [#65](https://github.com/ravinderjangra/SafeMobileBrowser/issues/65)
