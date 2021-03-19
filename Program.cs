using System;
using System.Threading;
using Pipeline.Data;
using Pipeline.Git;
using Pipeline.States;
using SimpleInjector;

namespace Pipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            var container = new Container();
            
            container.Register<Logger>(Lifestyle.Singleton);
            container.Register<ConfigProvider>(Lifestyle.Singleton);
            container.Register<ManifestProvider>(Lifestyle.Singleton);
            container.Register<DataContainer>(Lifestyle.Singleton);

            container.Register<FileService>(Lifestyle.Singleton);
            container.Register<GitService>(Lifestyle.Singleton);
            container.Register<BuildService>(Lifestyle.Singleton);

            var stateMachine = container.GetInstance<StateMachine>();

            stateMachine.Start();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}