using System.Collections.Generic;

namespace MovieList.Preferences
{
    public class FilePreferences
    {
        public bool ShowRecentFiles { get; set; }
        public List<string> RecentFiles { get; set; }

        public FilePreferences(bool showRecentFiles, List<string> recentFiles)
        {
            this.ShowRecentFiles = showRecentFiles;
            this.RecentFiles = recentFiles;
        }
    }
}
