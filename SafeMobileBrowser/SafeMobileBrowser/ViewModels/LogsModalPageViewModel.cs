using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class LogsModalPageViewModel : BaseViewModel
    {
        private readonly string _logFilesPath = DependencyService.Get<IPlatformService>().ConfigFilesPath;
        private readonly TimeSpan _toastTimeSpan = TimeSpan.FromSeconds(1.5);
        private readonly string _logFileExtension = "log";
        private readonly INavigation _navigation;

        public ICommand GoBackCommand { get; }

        public ICommand CopyLogFileContentCommand { get; }

        public ICommand DeleteLogFileCommand { get; }

        public ICommand DeleteAllLogFileCommand { get; }

        private ObservableCollection<string> _logFiles;

        public ObservableCollection<string> LogFiles
        {
            get => _logFiles;
            set => SetProperty(ref _logFiles, value);
        }

        public LogsModalPageViewModel(INavigation navigation)
        {
            try
            {
                _navigation = navigation;
                GoBackCommand = new Command(GoBackToHomePage);
                CopyLogFileContentCommand = new Command<string>(CopyLogFileContent);
                DeleteLogFileCommand = new Command<string>(DeleteLogFile);
                DeleteAllLogFileCommand = new Command(DeleteAllLogFilesAsync);

                if (LogFiles == null)
                    LogFiles = new ObservableCollection<string>();

                var files = Directory.GetFiles(_logFilesPath, "*.log").Reverse();
                if (!files.Any())
                    return;

                foreach (var file in files)
                {
                    LogFiles.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void DeleteLogFile(string fileName)
        {
            try
            {
                var logFileToDelete = Path.Combine(_logFilesPath, $"{fileName}.{_logFileExtension}");
                File.Delete(logFileToDelete);
                LogFiles.Remove(fileName);
                UserDialogs.Instance.Toast(Constants.LogFileDeleteSuccessfully, _toastTimeSpan);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                UserDialogs.Instance.Toast(ErrorConstants.FailedToDeleteLogFile, _toastTimeSpan);
            }
        }

        private void CopyLogFileContent(string fileName)
        {
            try
            {
                var logFileToReadContent = Path.Combine(_logFilesPath, $"{fileName}.{_logFileExtension}");
                var logFileText = File.ReadAllText(logFileToReadContent);
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Clipboard.SetTextAsync(logFileText);
                });
                UserDialogs.Instance.Toast(Constants.LogFileContentReadSuccessfully, _toastTimeSpan);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                UserDialogs.Instance.Toast(ErrorConstants.FailedToCopyLogFileContent, _toastTimeSpan);
            }
        }

        private async void DeleteAllLogFilesAsync()
        {
            try
            {
                var response = await Application.Current.MainPage.DisplayAlert(
                     Constants.DeleteLogFilesAlertTitle,
                     Constants.DeleteLogFilesAlertMsg,
                     "Yes",
                     "No");

                if (!response)
                    return;

                var directory = new DirectoryInfo(_logFilesPath);
                var lastModifiedFile = directory.GetFiles("*.log")
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault()
                    ?.Name
                    .Replace(".log", string.Empty);

                foreach (var file in LogFiles)
                {
                    if (file != lastModifiedFile)
                    {
                        var logFileToDelete = Path.Combine(_logFilesPath, $"{file}.{_logFileExtension}");
                        File.Delete(logFileToDelete);
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    // Known issue: clear list by removing item(s) one by one on iOS
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        LogFiles.Clear();
                    }
                    else
                    {
                        while (LogFiles.Count > 0)
                        {
                            LogFiles.RemoveAt(0);
                        }
                    }

                    LogFiles.Add(lastModifiedFile);
                });

                UserDialogs.Instance.Toast(Constants.LogFileDeleteSuccessfully, _toastTimeSpan);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                UserDialogs.Instance.Toast(ErrorConstants.FailedToDeleteLogFile, _toastTimeSpan);
            }
        }

        private async void GoBackToHomePage()
        {
            await _navigation.PopModalAsync();
        }
    }
}
