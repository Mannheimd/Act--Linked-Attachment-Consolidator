using Act_Database_Integration_Library;
using Message_Handler;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Act__Linked_Attachment_Consolidator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MessageHandler.debugMode = true;

            startupTasks();
        }

        public async void startupTasks()
        {
            await ActDatabaseConnectionManager.configureAct7Connection("(local)", true, null, null);
            databaseSelection_ListView.ItemsSource = await ActDatabaseTasks.getAllActDatabases();
        }

        public void consolidateAttachments_Button_Click(object sender, RoutedEventArgs e)
        {
            if (databaseSelection_ListView.SelectedIndex > -1)
            {
                runDatabaseAttachmentConsolidation(databaseSelection_ListView.SelectedItem as ActDatabase);
            }
        }

        public async void runDatabaseAttachmentConsolidation(ActDatabase database)
        {
            consolidateAttachments_Button.IsEnabled = false;

            List<ActAttachment> attachmentList = await ActDatabaseTasks.getAllActAttachments(database.name);
        }
    }
}
