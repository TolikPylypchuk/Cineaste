using System;
using System.Collections.Generic;

using MovieList.Config;
using MovieList.ViewModels;

namespace MovieList.Events
{
    public class SettingsUpdatedEventArgs : EventArgs
    {
        public SettingsUpdatedEventArgs(Configuration configuration, IList<KindViewModel> kinds)
        {
            this.Configuration = configuration;
            this.Kinds = kinds;
        }

        public Configuration Configuration { get; }
        public IList<KindViewModel> Kinds { get; }
    }
}
