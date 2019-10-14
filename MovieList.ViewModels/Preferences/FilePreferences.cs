using System.Collections.Generic;

namespace MovieList.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FilePreferences
    {
        public bool ShowRecentFiles { get; set; }
        public List<RecentFile> RecentFiles { get; set; }

        public FilePreferences(bool showRecentFiles, List<RecentFile> recentFiles)
        {
            this.ShowRecentFiles = showRecentFiles;
            this.RecentFiles = recentFiles;
        }
    }
}
