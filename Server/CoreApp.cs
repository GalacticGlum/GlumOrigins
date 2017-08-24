using System;
using GlumOrigins.Common.Logging;

namespace GlumOrigins.Server
{
    public delegate void TickEventHandler(TickEventArgs args);
    public class TickEventArgs : EventArgs
    {
        public float DeltaTime { get; }
        public TickEventArgs(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }

    public static class CoreApp
    {
        public static GameServer Server { get; private set; }
        public static event TickEventHandler Tick;

        private const double FixedTimestep = 1000000000.0 / 60.0;

        private static void Run()
        {
            Logger.Initialize(false);

            Server = new GameServer();
            GameController.Create();

            Server.Start();

            double deltaTime = 0;
            long lastFrameTime = TimeHelper.NanoTime;

            while (Server.IsRunning)
            {
                long timeNow = TimeHelper.NanoTime;
                deltaTime += (timeNow - lastFrameTime) / FixedTimestep;
                lastFrameTime = timeNow;

                while (deltaTime >= 1)
                {
                    Server.Listen();
                    Tick?.Invoke(new TickEventArgs((float)deltaTime));
                    deltaTime--;
                }
            }

            Exit();
        }

        private static void Exit()
        {
            Logger.FlushMessageBuffer();
            Tick = null;
        }

        private static void Main(string[] args)
        {
            Run();
        }
    }
}
