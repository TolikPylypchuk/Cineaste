using System.Collections.Generic;

using MovieList.Data.Models;
using MovieList.Properties;

namespace MovieList.Views.Forms
{
    public sealed class SeriesStatusString
    {
        private static readonly Dictionary<SeriesStatus, string> StatusToString = new Dictionary<SeriesStatus, string>
        {
            [SeriesStatus.NotStarted] = Messages.SeriesNotStarted,
            [SeriesStatus.Running] = Messages.SeriesRunning,
            [SeriesStatus.Finished] = Messages.SeriesFinished,
            [SeriesStatus.Cancelled] = Messages.SeriesCancelled
        };

        public SeriesStatusString(SeriesStatus status)
        {
            this.Status = status;
            this.Name = StatusToString[status];
        }

        public SeriesStatus Status { get; }
        public string Name { get; }
    }
}
