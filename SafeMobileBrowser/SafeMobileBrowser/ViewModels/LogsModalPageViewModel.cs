// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

﻿using System;
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
        private const string LogFileExtension = ".log";
        private readonly string _logFilesPath = DependencyService.Get<IPlatformService>().ConfigFilesPath;
        private readonly TimeSpan _toastTimeSpan = TimeSpan.FromSeconds(1.5);
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

        private string _lastModifiedFile;

        public string LastModifiedFile
        {
            get => _lastModifiedFile;
            set => SetProperty(ref _lastModifiedFile, value);
        }

        public LogsModalPageViewModel(INavigation navigation)
        {
            try
            {
                _navigation = navigation;
                GoBackCommand = new Command(GoBackToHomePage);
                CopyLogFileContentCommand = new Command<string>(CopyLogFileContent);
                DeleteLogFileCommand = new Command<string>(DeleteLogFile);
                DeleteAllLogFileCommand = new Command(DeleteAllLogFilesAsync, () => LogFiles.Count > 1);

                if (LogFiles == null)
                    LogFiles = new ObservableCollection<string>();

                var logFileDirectory = new DirectoryInfo(_logFilesPath);
                var files = logFileDirectory.GetFiles("*.log")
                    .Where(f => f.Name.StartsWith("log-", StringComparison.InvariantCulture))
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToList();

                if (!files.Any())
                    return;

                LastModifiedFile = files.FirstOrDefault()?.Name.Replace(LogFileExtension, string.Empty);

                foreach (var file in files)
                {
                    LogFiles.Add(file.Name.Replace(LogFileExtension, string.Empty));
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
                if (LastModifiedFile == fileName)
                {
                    UserDialogs.Instance.Toast(Constants.CurrentLogFile, _toastTimeSpan);
                    return;
                }

                var logFileToDelete = Path.Combine(_logFilesPath, $"{fileName}{LogFileExtension}");
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
                var logFileToReadContent = Path.Combine(_logFilesPath, $"{fileName}{LogFileExtension}");
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

                foreach (var file in LogFiles)
                {
                    if (file != LastModifiedFile)
                    {
                        var logFileToDelete = Path.Combine(_logFilesPath, $"{file}{LogFileExtension}");
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

                    LogFiles.Add(LastModifiedFile);
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
