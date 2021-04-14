using System.Net;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Shell;
using SimpleScribe.Data;
using SimpleScribe.Helpers;
using SimpleScribe.simplenote;

namespace SimpleScribe
{
    public partial class App : Application
    {
        public static int APP_VERSION = 2;

        public static Simplenote simpleNote { get; set; }

        private static ViewModelNotes viewModel = null;
        public static ViewModelNotes ViewModel
        {
            get
            {
                if (viewModel == null)
                    viewModel = new ViewModelNotes();

                return viewModel;
            }
        }

        public PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            UnhandledException += Application_UnhandledException;
            InitializeComponent();
            InitializePhoneApplication();

            using (NotesContext db = new NotesContext(NotesContext.DBConnectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    // Create database
                    db.CreateDatabase();

                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                    dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                    dbUpdater.Execute();
                }
                else
                {
                    // Check whether a database update is needed.
                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();

                    if (dbUpdater.DatabaseSchemaVersion < APP_VERSION)
                    {
                        db.DeleteDatabase();
                        db.CreateDatabase();

                        dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                        dbUpdater.Execute();
                    }
                }

                WebRequest.RegisterPrefix("https://simple-note.appspot.com/", SharpGIS.WebRequestCreator.GZip);
            }

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                //Application.Current.Host.Settings.EnableRedrawRegions = true;
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        private void Application_Launching(object sender, LaunchingEventArgs e) { }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            App.ViewModel.GetNotes();
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
            //ViewModel.FlushCache();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

            GlobalLoading.Instance.Initialize(RootFrame);
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}