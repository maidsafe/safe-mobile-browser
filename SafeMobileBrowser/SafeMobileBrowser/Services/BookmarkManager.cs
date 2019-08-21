using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(BookmarkManager))]

namespace SafeMobileBrowser.Services
{
    public class BookmarkManager
    {
        private static MDataInfo _accesscontainerMdinfo;
        private static Session _session;
        private static List<string> _bookmarksList;

        public static void InitialiseSession(Session session)
        {
            try
            {
                _session = session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public BookmarkManager()
        {
            _bookmarksList = new List<string>();
        }

        public void SetMdInfo(MDataInfo mdinfo)
        {
            _accesscontainerMdinfo = mdinfo;
        }

        private async void ReconnectBookmarkSession()
        {
            if (_session.IsDisconnected)
                await _session.ReconnectAsync();
        }

        internal async Task AddBookmark(string bookmarkUrl)
        {
            try
            {
                ReconnectBookmarkSession();
                var encryptedKey = await _session.MDataInfoActions.EncryptEntryKeyAsync(_accesscontainerMdinfo, Constants.AppStateMdEntryKey.ToUtfBytes());
                var (value, version) = await _session.MData.GetValueAsync(_accesscontainerMdinfo, encryptedKey);
                var decryptedValue = (await _session.MDataInfoActions.DecryptAsync(_accesscontainerMdinfo, value)).ToUtfString();
                var entryValue = CreateBrowserStateJsonString(bookmarkUrl, decryptedValue);
                var encryptedValue = await _session.MDataInfoActions.EncryptEntryValueAsync(_accesscontainerMdinfo, entryValue);
                using (var mutateEntriesHandle = await _session.MDataEntryActions.NewAsync())
                {
                    await _session.MDataEntryActions.UpdateAsync(mutateEntriesHandle, encryptedKey, encryptedValue, version + 1);
                    await _session.MData.MutateEntriesAsync(_accesscontainerMdinfo, mutateEntriesHandle);
                }

                _bookmarksList.Add(bookmarkUrl);
            }
            catch (FfiException ex)
            {
                if (ex.ErrorCode == -106)
                {
                    using (var entryHandle = await _session.MDataEntryActions.NewAsync())
                    {
                        var encryptedKey = await _session.MDataInfoActions.EncryptEntryKeyAsync(_accesscontainerMdinfo, Constants.AppStateMdEntryKey.ToUtfBytes());
                        var entryValue = CreateBrowserStateJsonString(bookmarkUrl);
                        var encryptedValue = await _session.MDataInfoActions.EncryptEntryValueAsync(_accesscontainerMdinfo, entryValue);
                        await _session.MDataEntryActions.InsertAsync(entryHandle, encryptedKey, encryptedValue);
                        await _session.MData.MutateEntriesAsync(_accesscontainerMdinfo, entryHandle);
                        _bookmarksList.Add(bookmarkUrl);
                    }
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        private List<byte> CreateBrowserStateJsonString(string bookmarkUrl, [Optional] string oldBrowserState)
        {
            JObject browserState = new JObject();
            JArray bookmarksArray = new JArray();

            if (!string.IsNullOrEmpty(oldBrowserState))
            {
                browserState = JObject.Parse(oldBrowserState);
                bookmarksArray = (JArray)browserState["bookmarks"];
                browserState.Remove("bookmarks");
            }

            JTokenWriter writer = new JTokenWriter();
            writer.WriteStartObject();
            writer.WritePropertyName("url");
            writer.WriteValue(bookmarkUrl);
            writer.WriteEndObject();

            JObject newBookmarksJObject = (JObject)writer.Token;
            bookmarksArray.Add(newBookmarksJObject);
            browserState.Add("bookmarks", bookmarksArray);
            var newbrowserState = JsonConvert.SerializeObject(browserState);
            return newbrowserState.ToUtfBytes();
        }

        internal bool CheckIfBookmarkAvailable(string currentUrl)
        {
            return _bookmarksList.Contains(currentUrl);
        }

        internal List<string> RetrieveBookmarks()
        {
            return _bookmarksList;
        }

        internal async Task FetchBookmarks()
        {
            try
            {
                ReconnectBookmarkSession();
                var bookmarks = new List<string>();
                var encryptedKey = await _session.MDataInfoActions.EncryptEntryKeyAsync(_accesscontainerMdinfo, Constants.AppStateMdEntryKey.ToUtfBytes());
                var (value, _) = await _session.MData.GetValueAsync(_accesscontainerMdinfo, encryptedKey);

                var decryptedValue = (await _session.MDataInfoActions.DecryptAsync(_accesscontainerMdinfo, value)).ToUtfString();
                var browserState = JObject.Parse(decryptedValue);
                var bookmarksArray = (JArray)browserState["bookmarks"];

                foreach (var item in bookmarksArray)
                {
                    var bookmark = item["url"].ToString();
                    if (bookmark != "safe-auth://home/#/login")
                    {
                        bookmarks.Add(bookmark);
                    }
                }

                _bookmarksList = bookmarks;
            }
            catch (FfiException ex)
            {
                Logger.Error(ex);
                if (ex.ErrorCode != -103)
                    throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        internal async Task DeleteBookmarks(string bookmark)
        {
            try
            {
                ReconnectBookmarkSession();
                var encryptedKey = await _session.MDataInfoActions.EncryptEntryKeyAsync(_accesscontainerMdinfo, Constants.AppStateMdEntryKey.ToUtfBytes());
                var (value, version) = await _session.MData.GetValueAsync(_accesscontainerMdinfo, encryptedKey);
                var decryptedValue = (await _session.MDataInfoActions.DecryptAsync(_accesscontainerMdinfo, value)).ToUtfString();
                var browserState = JObject.Parse(decryptedValue);
                var bookmarks = (JArray)browserState["bookmarks"];

                var bookmarkToDelete = bookmarks.FirstOrDefault(b => b["url"].ToString().Equals(bookmark));

                if (bookmarkToDelete != null)
                {
                    bookmarks.Remove(bookmarkToDelete);
                    browserState.Remove("bookmarks");
                    browserState.Add("bookmarks", bookmarks);

                    var newbrowserState = JsonConvert.SerializeObject(browserState);

                    var encryptedValue = await _session.MDataInfoActions.EncryptEntryValueAsync(_accesscontainerMdinfo, newbrowserState.ToUtfBytes());
                    using (var mutateEntriesHandle = await _session.MDataEntryActions.NewAsync())
                    {
                        await _session.MDataEntryActions.UpdateAsync(mutateEntriesHandle, encryptedKey, encryptedValue, version + 1);
                        await _session.MData.MutateEntriesAsync(_accesscontainerMdinfo, mutateEntriesHandle);
                    }
                }
                _bookmarksList.Remove(bookmark);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}
