read -r -d '' release_description << 'EOF'
The SAFE Mobile Browser is a cross-platform mobile (Android, iOS) browser, built to provide web browsing experience to the users on the SAFE Network.

## Changelog
CHANGELOG_CONTENT

## SHA-256 checksums:
```
APK checksum
APK_CHECKSUM

IPA checksum
IPA_CHECKSUM
```

## Related Links
* [Safe Mobile Authenticator Browser](https://github.com/maidsafe/safe-authenticator-mobile/releases/)
* [Safe Desktop Browser](https://github.com/maidsafe/safe_browser/releases/)
* [Safe CLI](https://github.com/maidsafe/sn_api/tree/master/sn_cli)
* [Safe Vault](https://github.com/maidsafe/safe_vault/releases/latest/)
* [safe_app_csharp](https://github.com/maidsafe/safe_app_csharp/)
EOF

apk_checksum=$(sha256sum "../net.maidsafe.browser.apk" | awk '{ print $1 }')
ipa_checksum=$(sha256sum "../SafeMobileBrowser.iOS.ipa" | awk '{ print $1 }')
changelog_content=$(sed '1,/]/d;/##/,$d' ../CHANGELOG.MD)
release_description=$(sed "s/APK_CHECKSUM/$apk_checksum/g" <<< "$release_description")
release_description=$(sed "s/IPA_CHECKSUM/$ipa_checksum/g" <<< "$release_description")

echo "${release_description/CHANGELOG_CONTENT/$changelog_content}" > release_description.txt
