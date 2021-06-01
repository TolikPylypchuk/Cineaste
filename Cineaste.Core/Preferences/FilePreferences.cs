using System.Collections.Generic;

namespace Cineaste.Core.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FilePreferences
    {
        public FilePreferences(bool showRecentFiles, List<RecentFile> recentFiles)
        {
            this.ShowRecentFiles = showRecentFiles;
            this.RecentFiles = recentFiles;
        }

        public bool ShowRecentFiles { get; set; }
        public List<RecentFile> RecentFiles { get; set; }
    }
}
