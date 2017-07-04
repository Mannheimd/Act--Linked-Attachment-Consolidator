using Message_Handler;
using System.Windows;

namespace Act__Linked_Attachment_Consolidator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageHandler.handleMessage(true, 2, e.Exception, "Unhandled exception");
            Application.Current.Shutdown();
            e.Handled = true;
        }
    }
}
