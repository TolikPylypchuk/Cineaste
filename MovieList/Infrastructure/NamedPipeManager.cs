using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace MovieList.Infrastructure
{
    public sealed class NamedPipeManager
    {
        private Thread? thread;

        private readonly Subject<string> receivedString = new Subject<string>();

        public NamedPipeManager(string? name)
        {
            this.NamedPipeName = name;
        }

        public string? NamedPipeName { get; }

        public IObservable<string> ReceivedString
            => this.receivedString.AsObservable();

        public void StartServer()
        {
            this.thread = new Thread(this.WaitForMessages)
            {
                IsBackground = true
            };

            this.thread.Start();
        }

        public bool Write(string text, int connectTimeout = 300)
        {
            using var client = new NamedPipeClientStream(this.NamedPipeName);

            try
            {
                client.Connect(connectTimeout);
            } catch
            {
                return false;
            }

            if (!client.IsConnected)
            {
                return false;
            }

            using var writer = new StreamWriter(client);
            writer.Write(text);
            writer.Flush();

            return true;
        }

        private void WaitForMessages()
        {
            while (true)
            {
                string text;
                using (var server = new NamedPipeServerStream(this.NamedPipeName, PipeDirection.InOut, 10))
                {
                    server.WaitForConnection();

                    using var reader = new StreamReader(server);
                    text = reader.ReadToEnd();
                }

                this.receivedString.OnNext(text);
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}
