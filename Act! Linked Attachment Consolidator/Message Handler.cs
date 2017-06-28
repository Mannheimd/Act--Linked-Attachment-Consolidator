using System;
using System.Threading.Tasks;
using System.Windows;

namespace MessageHandler
{
    public class MessageHandler
    ///<summary>
    /// This handles any messages being passed by the application including warnings, alerts and fatal errors.
    /// 
    /// Key for logging levels:
    /// 1 - Fatal   Data loss is likely to have occurred, or is likely to occur, as a result of whatever just happened
    /// 2 - Error   Application cannot function correctly following this event, and will terminate
    /// 3 - Warn    Application was stopped from doing something, but can keep running (maybe switched to a backup, or required information is missing)
    /// 4 - Info    Useful information about what just happened, maybe a service started or a connection was established
    /// 5 - Debug   Information useful for technicians or sysadmins to troubleshoot an issue
    /// 6 - Trace   Application has an itch on its nose that the developer might want to know about
    ///</summary>
    {
        private static bool _debugMode = false; // Enabling Debug Mode causes message boxes to appear for every little thing, including Trace
        public static bool debugMode
        {
            set { _debugMode = value; }
            get { return _debugMode; }
        }

        private static int _eventViewerLoggingLevel = 2;
        public static int eventViewerLoggingLevel
        {
            set { _eventViewerLoggingLevel = value; }
            get { return _eventViewerLoggingLevel; }
        }

        private static int _applicationLogFileLoggingLevel = 4;
        public static int applicationLogFileLoggingLevel
        {
            set { _applicationLogFileLoggingLevel = value; }
            get { return _applicationLogFileLoggingLevel; }
        }

        public static async Task handleError(bool isQuiet, Exception error, string context) // Deprecated in favour of HandleMessage, which includes ErrorLevel
        {
            ///<summary>
            /// Any errors occurring within Act_Database_Integration_Library should call this function.
            /// Currently this only creates a messagebox, but will be expanded in future to also log to Event Viewer and possibly an application-specific log file.
            ///</summary>

            if (!isQuiet || _debugMode)
            {
                MessageBox.Show("An error has occurred.\n\nError context:\n" + context + "\n\nError message:\n" + error.Message, "Act! Database Integration Error");
            }
        }

        public static async Task handleMessage(bool showMessagebox, int errorLevel, Exception error, string context)
        {
            if (errorLevel >= _eventViewerLoggingLevel)
            {
                logEventViewerEvent(errorLevel, error, context);
            }

            if (errorLevel >= _applicationLogFileLoggingLevel)
            {
                logEventViewerEvent(errorLevel, error, context);
            }

            if (showMessagebox || _debugMode)
            {
                MessageBox.Show("An error has occurred.\n\nError context:\n" + context + "\n\nError message:\n" + error.Message, "Application Error");
            }
        }

        private static async void logEventViewerEvent(int errorLevel, Exception error, string context)
        {
            // Add code for logging an Event Viewer event
        }

        private static async void logApplicationLogFileEntry(int errorLevel, Exception error, string context)
        {
            // Add code for logging an event in the application's log file
        }
    }
}