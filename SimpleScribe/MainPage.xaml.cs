using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using SimpleScribe.Data;
using SimpleScribe.simplenote;

namespace SimpleScribe
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void ApplicationBarMenuSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainListBox.SelectedIndex == -1) return;
            Note n = MainListBox.SelectedItem as Note;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + n.Id, UriKind.Relative));
            MainListBox.SelectedIndex = -1;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            AppSettings settings = new AppSettings();
            if (settings.GetValueOrDefault("SNEnabled", false))
            {
                App.simpleNote = new Simplenote(App.ViewModel);
            }
            base.OnNavigatedTo(e);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.GetNotes();
            Utils.RemindReview();
        }

        private void ApplicationBarAdd_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));
        }
    }
}