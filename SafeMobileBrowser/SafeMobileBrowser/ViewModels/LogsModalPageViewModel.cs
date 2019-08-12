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
        private readonly string _logFileExtension = "log";
        private readonly INavigation _navigation;

        public ICommand GoBackCommand { get; }

        public ICommand CopyLogFileContentCommand { get; }

        public ICommand DeleteLogFilecommand { get; }

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
                DeleteLogFilecommand = new Command<string>(DeleteLogFile);

                if (LogFiles == null)
                    LogFiles = new ObservableCollection<string>();

                var files = Directory.GetFiles(_logFilesPath, "*.log");
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
                UserDialogs.Instance.Toast("Log file deleted");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                UserDialogs.Instance.Toast("Failed to delete log file");
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
                UserDialogs.Instance.Toast("Log file content copied to clipboard");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                UserDialogs.Instance.Toast("Failed to copy log file content");
            }
        }

        private async void GoBackToHomePage()
        {
            await _navigation.PopModalAsync();
        }
    }
}
