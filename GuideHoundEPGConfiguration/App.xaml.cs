using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Core.UI.Services;
using GuideHoundEPG.UI.Controllers;
using GuideHoundEPG.UI.Services;

namespace GuideHoundEPG.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            System.Windows.Forms.Application.EnableVisualStyles();

            ServiceContainer.Instance.AddService<IProgressHelper>(new ProgressHelper());

            ConfigurationWindowController configurationWindowController = new ConfigurationWindowController();
            configurationWindowController.Show();

        }
    }
}
