using Autofac;
using Dicom.Log;
using Dicom.Network;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XRoute
{
    static class Program
    {
        static void Main()
        {
            LogManager.SetImplementation(new SerilogManager());

            LoadSettings();
            BuildContainer();

            var dicomServers = new List<IDicomServer>();
            try
            {
                foreach (var route in m_Settings.Routes)
                {
                    dicomServers.Add(DicomStoreServer.Create(route));
                }
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred");
            }
            finally
            {
                foreach (var dicomServer in dicomServers)
                {
                    dicomServer?.Dispose();
                }
            }
        }

        static Settings m_Settings;

        private static void LoadSettings()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            m_Settings = config.Get<Settings>();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
        }

        private static void BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register(context => m_Settings).AsSelf();
            builder.RegisterType<RouteService>().AsSelf();
            builder.RegisterType<UploadService>().AsSelf();
            m_Container = builder.Build();
        }

        static IContainer m_Container;

        public static T Resolve<T>() => m_Container.Resolve<T>();
    }
}
