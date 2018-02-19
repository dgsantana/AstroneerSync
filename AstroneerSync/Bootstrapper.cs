using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using LibGit2Sharp;

namespace AstroneerSync
{
    public class Bootstrapper : BootstrapperBase
    {
        private TaskbarIcon _sysTaskbarIcon;
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            var app = sender as App;

            Debug.Assert(app != null, "We are not running a App.");

            var astroSaves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Astro", "Saved", "SaveGames");
            if (Directory.Exists(astroSaves))
            {
                var astroSavesRepo = new Repository(astroSaves);
                _container.RegisterInstance(typeof(Repository), "AstroneerRepo", astroSavesRepo);
            }

            _sysTaskbarIcon = (TaskbarIcon) app.FindResource("MainSystemTrayIcon");

        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            _sysTaskbarIcon.Dispose();
            _container.GetInstance<Repository>("AstroneerRepo").Dispose();
            base.OnExit(sender, e);
        }
    }
}
