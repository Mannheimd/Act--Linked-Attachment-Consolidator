using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Act_Database_Integration_Library
{
    public class ActDatabaseTasks
    {
        public static async Task<List<ActDatabase>> getAllActDatabases()
        {
            DataTable actDatabasesDataTable = await ActDatabaseConnectionManager.runActSqlQuery("select name from sys.databases where name != 'master' and name != 'model' and name != 'msdb' and name != 'tempdb' and name != 'ActEmailMessageStore'");

            List<ActDatabase> databaseList = new List<ActDatabase>();

            foreach (DataRow row in actDatabasesDataTable.Rows)
            {
                ActDatabase database = new ActDatabase();
                database.name = row["name"].ToString();

                databaseList.Add(database);
            }

            return databaseList;
        }

        public static async Task<List<ActAttachment>> getAllActAttachments(string databaseName)
        {
            DataTable actAttachmentDataTable = await ActDatabaseConnectionManager.runActSqlQuery("use " + databaseName + " select * from tbl_attachment");

            List<ActAttachment> attachmentList = new List<ActAttachment>();

            foreach (DataRow row in actAttachmentDataTable.Rows)
            {
                ActAttachment attachment = new ActAttachment();
                attachment.activityId = row["ATTACHMENTID"].ToString();
                attachment.workflowDefId = row["WORKFLOWDEFID"].ToString();
                attachment.fileName = row["FILENAME"].ToString();
                attachment.displayName = row["DISPLAYNAME"].ToString();
                attachment.noteId = row["NOTEID"].ToString();
                attachment.activityId = row["ACTIVITYID"].ToString();
                attachment.historyId = row["HISTORYID"].ToString();
                attachment.machineName = row["MACHINENAME"].ToString();
                attachment.fileName = row["FILEPATH"].ToString();

                attachmentList.Add(attachment);
            }

            return attachmentList;
        }
    }

    public class ActDatabaseConnectionManager
    {
        private static SqlConnection act7Connection = new SqlConnection();
        private static string act7ConnectionString = null;
        private static bool connectionConfigured = false;

        public static async Task<bool> configureAct7Connection(string hostMachinePath, bool useWindowsAuth, string sqlUser, string sqlPassword)
        {
            if (useWindowsAuth)
            {
                act7ConnectionString = "Data Source=" + hostMachinePath + "; Initial Catalog=master; Server=" + hostMachinePath + "\\ACT7; Integrated Security=True;";
            }
            else
            {
                act7ConnectionString = "Data Source=" + hostMachinePath + "; Initial Catalog=master; Server=" + hostMachinePath + "\\ACT7; User Id=" + sqlUser + "; Password=" + sqlPassword + ";";
            }

            act7Connection = new SqlConnection(act7ConnectionString);

            try
            {
                // Test the SQL connection
                await act7Connection.OpenAsync();
                act7Connection.Close();

                connectionConfigured = true;

                return true;
            }
            catch(Exception error)
            {
                ActDatabaseMessageHandler.handleError(true, error, "Testing connection to ACT7 instance");

                connectionConfigured = false;

                return false;
            }
        }

        public static async Task<DataTable> runActSqlQuery(string queryString)
        {
            if (!connectionConfigured)
            {
                await ActDatabaseMessageHandler.handleError(true, new Exception(), "Tried running SQL query without configured SQL connection");

                return null;
            }

            DataTable queryOutputDataTable = new DataTable();
            SqlCommand command = new SqlCommand(queryString, act7Connection);
            SqlDataAdapter dataAdaptor = new SqlDataAdapter(command);
            try
            {
                act7Connection.Open();
                dataAdaptor.Fill(queryOutputDataTable);
                act7Connection.Close();
            }
            catch(Exception error)
            {
                await ActDatabaseMessageHandler.handleError(true, error, "Running SQL query on ACT7 instance: " + queryString);
            }
            return queryOutputDataTable;
        }
    }

    public class ActDatabaseMessageHandler
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

    public class ActDatabase
    {
        public string name { get; set; }
        public string datastoreLocation { get; set; }
    }

    public class ActAttachment
    {
        public string attachmentId { get; set; }
        public string workflowDefId { get; set; }
        public string fileName { get; set; }
        public string displayName { get; set; }
        public string noteId { get; set; }
        public string activityId { get; set; }
        public string historyId { get; set; }
        public string machineName { get; set; }
        public string filePath { get; set; }
    }
}