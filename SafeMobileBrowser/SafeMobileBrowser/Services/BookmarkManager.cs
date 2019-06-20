using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;

namespace SafeMobileBrowser.Models
{
    public class BookmarkManager
    {
        private static MDataInfo _accesscontainerMdinfo;
        private static Session _session;

        public void SetSession(Session session)
        {
            _session = session;
        }

        public void SetMdInfo(MDataInfo mdinfo)
        {
            _accesscontainerMdinfo = mdinfo;
        }

        internal async Task<List<string>> FetchBookmarks()
        {
            var bookmarks = new List<string>();
            try
            {
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
                Console.WriteLine($"Error: {ex.Message}");
            }
            return bookmarks;
        }

        internal async Task DeleteBookmarks(string bookmark)
        {
            using (var entriesHandle = await _session.MDataEntries.GetHandleAsync(_accesscontainerMdinfo))
            {
                try
                {
                    var encryptedKey = await _session.MDataInfoActions.EncryptEntryKeyAsync(_accesscontainerMdinfo, Constants.AppStateMdEntryKey.ToUtfBytes());
                    var (value, version) = await _session.MData.GetValueAsync(_accesscontainerMdinfo, encryptedKey);
                    var decryptedValue = (await _session.MDataInfoActions.DecryptAsync(_accesscontainerMdinfo, value)).ToUtfString();
                    var browserState = JObject.Parse(decryptedValue);
                    var bookmarks = (JArray)browserState["bookmarks"];

                    var bookmarkToDelete = bookmarks.Where(b => b["url"].ToString().Equals(bookmark)).FirstOrDefault();

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
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
