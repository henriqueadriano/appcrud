using System;
using System.Windows.Forms;
using SimpleInjector;
using DataAccess;
using Helper;
using log4net;
using SimpleInjector.Lifestyles;

namespace AppCRUD
{
    static class Program
    {
        private static Container container;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Dependency Injection Configuration
            Bootstrap();

            //Config log4net
            log4net.Config.XmlConfigurator.Configure();

            Application.Run(container.GetInstance<Form1>());
        }

        private static void Bootstrap()
        {
            // Create the container as usual.
            container = new Container();

            // Register your types, for instance:
            container.Register<IDataBaseService, DataBaseService>(Lifestyle.Singleton);
            container.Register<IFormHelper, FormHelper>(Lifestyle.Singleton);
            container.Register<ISqliteDataAccess, SqliteDataAccess>(Lifestyle.Singleton);
            container.Register<ILog>(()=>LogManager.GetLogger(typeof(object)),Lifestyle.Singleton);
            container.Register<Form1>(Lifestyle.Singleton);
        }
    }
}
