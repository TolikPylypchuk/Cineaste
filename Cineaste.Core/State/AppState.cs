using System.Runtime.Serialization;

namespace MovieList.Core.State
{
    [DataContract]
    public class AppState
    {
        [DataMember]
        public double WindowWidth { get; set; }

        [DataMember]
        public double WindowHeight { get; set; }

        [DataMember]
        public double WindowX { get; set; }

        [DataMember]
        public double WindowY { get; set; }

        [DataMember]
        public bool IsWindowMaximized { get; set; }

        [DataMember]
        public bool IsInitialized { get; set; }
    }
}
