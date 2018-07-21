using Xarivu.Coder.Service;
using Xarivu.Coder.Utilities;
using Xarivu.Coder.ViewModel.Dialog;
using Xarivu.CoderTest.Service;
using System.Windows;

namespace Xarivu.CoderTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RegisterServices();
            RegisterViewModels();
        }

        void RegisterServices()
        {
            var notificationService = new NotificationService();
            var sharedDataService = new SharedDataService();
            DependencyContainer.RegisterSingleton<NotificationService>(notificationService);
            DependencyContainer.RegisterSingleton<SharedDataService>(sharedDataService);
        }

        void RegisterViewModels()
        {
            // Singleton view models.
            DependencyContainer.RegisterSingleton<DialogServiceViewModel>(new DialogServiceViewModel());
        }
    }
}
