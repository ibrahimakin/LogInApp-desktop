using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogInApp
{
    /// <summary>
    /// Interaction logic for AddRecord.xaml
    /// </summary>
    public partial class AddRecord : UserControl
    {
        List<string> emails;
        List<string> usernames;
        List<string> hints;
        List<string> labelses;
        public AddRecord()
        {
            InitializeComponent();
            emails = Database.Records.GetEMails();
            usernames = Database.Records.GetUsernames();
            hints = Database.Records.GetHints();
            labelses = Database.Records.GetLabels();
            Email.ItemsSource = emails;
            Username.ItemsSource = usernames;
            Hint.ItemsSource = hints;
            Labels.ItemsSource = labelses;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (e.Key == Key.Return)
            {
                Button_Click(sender, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string site = Site.Text; string email = Email.Text;
            if (string.IsNullOrWhiteSpace(site))
            {
                ShowNotification(aNotification, "Site Boş Olamaz.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                ShowNotification(aNotification, "e-mail Boş Olamaz.");
                return;
            }
            string username = Username.Text; string hint = Hint.Text; string labels = Labels.Text;
            string value = Sync.MD5Operations.GetMd5Hash(site + email);
            string now = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            try
            {
                Database.Records.AddToTable(Site.Text, Email.Text, Username.Text, Hint.Text, labels, now, value);
                Record r = new Record(-1, site, email, username, hint, labels, now, now, 2, value);
                MainWindow mw = (MainWindow)Application.Current.MainWindow;
                mw.AddtoRecordList(r);
                ShowNotification(aNotification, "Eklendi.");
                ((IUpdateCount)System.Windows.Application.Current.MainWindow).UpadateCount();  // Update number of Records in MainWindow through the IUpdateCount interface
            }
            catch (Exception)
            {
                ShowNotification(aNotification, "Kayıt Mevcut.");
            }
        }

        private async Task ShowNotification(TextBlock tbNotification, string notification)
        {
            tbNotification.Text = notification;
            await Task.Delay(2000);
            tbNotification.Text = "";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Site.Text = "";
            Email.Text = "";
            Username.Text = "";
            Hint.Text = "";
            Labels.Text = "";
        }
        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Hide_Click(sender, e);
        }
    }
}
