using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(BookmarkManager))]

namespace SafeMobileBrowser.Models
{
    public class BookmarkManager
    {
        private static MDataInfo _accesscontainerMdinfo;
        private static Session _session;
        private static List<string> bookmarksList;

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
            bookmarksList = new List<string>();
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
                using (var entriesHandle = await _session.MDataEntries.GetHandleAsync(_accesscontainerMdinfo))
                {
                    var encryptedKey = await _session.MDataInfoActions.EncryptEntryKeyAsync(_accesscontainerMdinfo, Constants.AppStateMdEntryKey.ToUtfBytes());
                    var (value, version) = await _session.MData.GetValueAsync(_accesscontainerMdinfo, encryptedKey);

                    var decryptedValue = (await _session.MDataInfoActions.DecryptAsync(_accesscontainerMdinfo, value)).ToUtfString();
                    var browserState = JObject.Parse(decryptedValue);
                    var bookmarksArray = (JArray)browserState["bookmarks"];
                    browserState.Remove("bookmarks");
                    JTokenWriter writer = new JTokenWriter();
                    writer.WriteStartObject();
                    writer.WritePropertyName("url");
                    writer.WriteValue(bookmarkUrl);
                    writer.WriteEndObject();
                    JObject newBookmarksJObject = (JObject)writer.Token;
                    bookmarksArray.Add(newBookmarksJObject);
                    browserState.Add("bookmarks", bookmarksArray);
                    var newbrowserState = JsonConvert.SerializeObject(browserState);
                    var encryptedValue = await _session.MDataInfoActions.EncryptEntryValueAsync(_accesscontainerMdinfo, newbrowserState.ToUtfBytes());
                    using (var mutateEntriesHandle = await _session.MDataEntryActions.NewAsync())
                    {
                        await _session.MDataEntryActions.UpdateAsync(mutateEntriesHandle, encryptedKey, encryptedValue, version + 1);
                        await _session.MData.MutateEntriesAsync(_accesscontainerMdinfo, mutateEntriesHandle);
                    }
                }
                bookmarksList.Add(bookmarkUrl);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        internal bool CheckIfBookmarkAvailable(string currentUrl)
        {
            return bookmarksList.Contains(currentUrl);
        }

        internal List<string> RetrieveBookmarks()
        {
            return bookmarksList;
        }

        internal async Task FetchBookmarks()
        {
            var bookmarks = new List<string>();
            try
            {
                ReconnectBookmarkSession();
                using (var entriesHandle = await _session.MDataEntries.GetHandleAsync(_accesscontainerMdinfo))
                {
                    var encryptedKey = await _session.MDataInfoActions.EncryptEntryKeyAsync(_accesscontainerMdinfo, Constants.AppStateMdEntryKey.ToUtfBytes());
                    var (value, version) = await _session.MData.GetValueAsync(_accesscontainerMdinfo, encryptedKey);

                    var decryptedValue = (await _session.MDataInfoActions.DecryptAsync(_accesscontainerMdinfo, value)).ToUtfString();
                    var browserState = JObject.Parse(decryptedValue);
                    var bookmarksArray = (JArray)browserState["bookmarks"];

                    foreach (var item in bookmarksArray)
                    {
                        bookmarks.Add(item["url"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            bookmarksList = bookmarks;
        }

        internal async Task DeleteBookmarks(string bookmark)
        {
            try
            {
                ReconnectBookmarkSession();
                using (var entriesHandle = await _session.MDataEntries.GetHandleAsync(_accesscontainerMdinfo))
                {
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
                    bookmarksList.Remove(bookmark);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw ex;
            }
        }
    }
}
