using System;
using System.Reflection;
using System.Threading;

namespace MovieList.Infrastructure
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

            try
            {
                bool hasHandle = mutex.WaitOne(5000, false);
                if (!hasHandle)
                {
                    throw new TimeoutException("Timeout waiting for exclusive access on the mutex.");
                }
            } catch (AbandonedMutexException)
            {
                // ignore for now
            }

            return mutex;
        }

        private static void SendArgumentAndExit()
        {
            try
            {
                var namePipeManager = new NamedPipeManager(Assembly.GetExecutingAssembly().FullName);

                string message = Environment.GetCommandLineArgs().Length > 1
                    ? Environment.GetCommandLineArgs()[1]
                    : String.Empty;

                namePipeManager.Write(message);
            } finally
            {
                Environment.Exit(0);
            }
        }
    }
}
