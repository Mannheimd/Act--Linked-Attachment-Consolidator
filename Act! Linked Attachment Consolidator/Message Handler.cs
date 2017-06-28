using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Message_Handler
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

        private static int _eventViewerLoggingLevel = 2; // Defines what level of error will be logged in Event Viewer; defaults to 2 or lower
        public static int eventViewerLoggingLevel
        {
            set { _eventViewerLoggingLevel = value; }
            get { return _eventViewerLoggingLevel; }
        }

        private static int _applicationLogFileLoggingLevel = 4; // Defines what level of error will be logged in the Application log; defaults to 4 or lower
        public static int applicationLogFileLoggingLevel
        {
            set { _applicationLogFileLoggingLevel = value; }
            get { return _applicationLogFileLoggingLevel; }
        }

        public static void handleMessage(bool showMessagebox, int messageLevel, Exception error, string context)
        {
            MessageLevelDefinition messageDef = MessageLevel.level(messageLevel);

            if (messageLevel >= _eventViewerLoggingLevel)
            {
                logEventViewerEvent(messageDef, error, context);
            }

            if (messageLevel >= _applicationLogFileLoggingLevel)
            {
                logEventViewerEvent(messageDef, error, context);
            }

            if (showMessagebox || _debugMode)
            {
                MessageBox.Show(messageDef.messageBoxIntro + "\n\nContext:\n" + context + "\n\nError message:\n" + error.Message, messageDef.messageBoxTitle);
            }
        }

        private static async void logEventViewerEvent(MessageLevelDefinition messageDef, Exception error, string context)
        {
            // Add code for logging an Event Viewer event
        }

        private static async void logApplicationLogFileEntry(MessageLevelDefinition messageDef, Exception error, string context)
        {
            // Add code for logging an event in the application's log file
        }
    }

    public class MessageLevel
    {
        private static Dictionary<int, MessageLevelDefinition> _messageLevelDict = new Dictionary<int, MessageLevelDefinition>();
        public Dictionary<int, MessageLevelDefinition> messageLevelDict
        {
            get
            {
                if (_messageLevelDict == new Dictionary<int, MessageLevelDefinition>())
                {
                    buildDefaultDict();
                }
                return _messageLevelDict;
            }
        }

        private void buildDefaultDict()
        {
            addMessageDefinition(1, "Fatal", "Fatal error", "A fatal error has occurred.");
            addMessageDefinition(2, "Error", "Application error", "An error has occurred in the application.");
            addMessageDefinition(3, "Warn", "Warning", "The application has generated a warning alert.");
            addMessageDefinition(4, "Info", "Information", "Information has been generated");
            addMessageDefinition(5, "Debug", "Debug alert", "A debug notification was triggered");
            addMessageDefinition(6, "Trace", "Trace alert", "A trace notification was triggered");
        }

        public void addMessageDefinition(int levelNumber, string levelName, string messageBoxTitle, string messageBoxIntro)
        {
            MessageLevelDefinition levelDef = new MessageLevelDefinition();
            levelDef.levelName = levelName;
            levelDef.messageBoxTitle = messageBoxTitle;
            levelDef.messageBoxIntro = messageBoxIntro;

            if (_messageLevelDict.ContainsKey(levelNumber))
            {
                _messageLevelDict[levelNumber] = levelDef;
            }
            else
            {
                _messageLevelDict.Add(levelNumber, levelDef);
            }
        }

        public static MessageLevelDefinition level(int levelNumber)
        {
            if (_messageLevelDict.ContainsKey(levelNumber))
            {
                return _messageLevelDict[levelNumber];
            }
            else
            {
                return null;
            }
        }
    }

    public class MessageLevelDefinition
    {
        public string levelName { get; set; }
        public string messageBoxTitle { get; set; }
        public string messageBoxIntro { get; set; }
    }
}