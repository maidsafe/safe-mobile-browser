APP_START_FILE="SafeMobileBrowser/SafeMobileBrowser/Services/AppUpdateService.cs"

ANDROID_APPCENTER_SECRET_PLACEHOLDER="_ANDROID_APP_CENTER_SECRET_"
IOS_APPCENTER_SECRET_PLACEHOLDER="_IOS_APP_CENTER_SECRET_"

echo "File to modify: $APP_START_FILE"

if [ -e "$APP_START_FILE" ] ; then
	echo "Updating Android AppCenter secret"
	sed -i ".bak" -e "s,$ANDROID_APPCENTER_SECRET_PLACEHOLDER,$ANDROID_APPCENTER_APP_SECRET,g" $APP_START_FILE
	echo "Updating iOS AppCenter secret"
	sed -i ".bak" -e "s,$IOS_APPCENTER_SECRET_PLACEHOLDER,$IOS_APPCENTER_APP_SECRET,g" $APP_START_FILE
	echo "Resulting file content:"
	cat $APP_START_FILE
else
    echo "Error! File not found"
	exit
fi
