using System;
using System.Reflection;
using System.Threading;

namespace Cineaste.Infrastructure
{
    public static class SingleInstanceManager
    {
        public static Mutex TryAcquireMutex()
        {
            var mutex = new Mutex(false, $"Global\\{Assembly.GetExecutingAssembly().FullName}", out bool createdNew);

            if (!createdNew)
            {
                SendArgumentAndExit();
            }

            bool hasHandle = mutex.WaitOne(5000, false);
            if (!hasHandle)
            {
                throw new TimeoutException("Timeout waiting for exclusive access on the mutex");
            }

            return mutex;
        }

        private static void SendArgumentAndExit()
        {
            try
            {
                var namedPipeManager = new NamedPipeManager(
                    Assembly.GetExecutingAssembly()?.GetName()?.Name ?? String.Empty);

                string message = Environment.GetCommandLineArgs().Length > 1
                     ? Environment.GetCommandLineArgs()[1]
                     : String.Empty;

                namedPipeManager.Write(message);
            } finally
            {
                Environment.Exit(0);
            }
        }
    }
}
