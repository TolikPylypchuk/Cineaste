namespace Cineaste.Infrastructure;

using System.IO.Pipes;

public sealed class NamedPipeManager : IEnableLogger
{
    private Thread? thread;

    private readonly Subject<string> receivedString = new();

    public NamedPipeManager(string name) =>
        this.NamedPipeName = name;

    public string NamedPipeName { get; }

    public IObservable<string> ReceivedString =>
        this.receivedString.AsObservable();

    public void StartServer()
    {
        this.thread = new Thread(this.WaitForMessages)
        {
            IsBackground = true
        };

        this.thread.Start();
    }

    public bool Write(string text, int connectionTimeout = 300)
    {
        using var client = new NamedPipeClientStream(this.NamedPipeName);

        try
        {
            client.Connect(connectionTimeout);
        } catch (Exception e)
        {
            this.Log().Error(e);
            return false;
        }

        if (!client.IsConnected)
        {
            this.Log().Error("The client is not connected");
            return false;
        }

        using var writer = new StreamWriter(client);
        writer.Write(text);
        writer.Flush();

        return true;
    }

    [DoesNotReturn]
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
    }
}
