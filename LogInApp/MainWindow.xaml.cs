using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LogInApp
{
    public partial class MainWindow : Window
    {
        Button clickedButton;
        List<Record> records;
        List<string> labels;
        CollectionView view;
        Record selectedRecord;
        bool isSelectable = true;
        string oldValue;
        System.Windows.Input.MouseButtonEventArgs a;

        public MainWindow()
        {
            InitializeComponent();
            Database.Operations.OpenConn();
            records = Database.Records.GetItems();
            recordList.ItemsSource = records;
            view = (CollectionView)CollectionViewSource.GetDefaultView(recordList.ItemsSource);
            view.Filter = RecordFilter;
            labels = Database.Records.GetLabels();
            Label.ItemsSource = labels;
            additionPage.Content = new AddRecord();
        }

        private void searchTextChanged(object sender, TextChangedEventArgs e)
        {
            isSelectable = false;
            CollectionViewSource.GetDefaultView(recordList.ItemsSource).Refresh();
            isSelectable = true;
        }

        private void SelectedLabelChanged(object sender, SelectionChangedEventArgs e)
        {
            isSelectable = false;
            CollectionViewSource.GetDefaultView(recordList.ItemsSource).Refresh();
            isSelectable = true;
        }

        private bool RecordFilter(object item)
        {
            if (String.IsNullOrEmpty(searchText.Text)) {
                if (Label.SelectedItem != null && !string.IsNullOrEmpty(Label.SelectedItem.ToString()))
                {
                    return ((item as Record).Labels.IndexOf(Label.SelectedItem.ToString(), StringComparison.Ordinal) >= 0);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (Label.SelectedItem != null && !string.IsNullOrEmpty(Label.SelectedItem.ToString()))
                {
                    return ((item as Record).Labels.IndexOf(Label.SelectedItem.ToString(), StringComparison.Ordinal) >= 0) && ((item as Record).Site.IndexOf(searchText.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                return ((item as Record).Site.IndexOf(searchText.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void recordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isSelectable)
            {
                selectedRecord = recordList.SelectedItem as Record;
                tbSite.Text = selectedRecord.Site;
                tbEMail.Text = selectedRecord.EMail;
                bSite.Visibility = Visibility.Visible;
                bEMail.Visibility = Visibility.Visible;

                string username = selectedRecord.Username;
                if (string.IsNullOrEmpty(username))
                {
                    aUsername.Visibility = Visibility.Visible;
                    bUsername.Visibility = Visibility.Hidden;
                }
                else
                {
                    aUsername.Visibility = Visibility.Hidden;
                    bUsername.Visibility = Visibility.Visible;
                }
                tbUsername.Text = username;

                string hint = selectedRecord.Hint;
                if (string.IsNullOrEmpty(hint))
                {
                    aHint.Visibility = Visibility.Visible;
                    bHint.Visibility = Visibility.Hidden;
                }
                else
                {
                    aHint.Visibility = Visibility.Hidden;
                    bHint.Visibility = Visibility.Visible;
                }
                tbHint.Text = hint;

                string labels = selectedRecord.Labels;
                if (string.IsNullOrEmpty(labels))
                {
                    aLabels.Visibility = Visibility.Visible;
                    bLabels.Visibility = Visibility.Hidden;
                }
                else
                {
                    aLabels.Visibility = Visibility.Hidden;
                    bLabels.Visibility = Visibility.Visible;
                }
                tbLabels.Text = labels;

                additionPage.Visibility = Visibility.Hidden;
                details.Visibility = Visibility.Visible;
                
            }
            HideEdit();
            ShowDetailsInfo();
            CancelButtonClick(sender, e);
            isSelectable = true;
        }

        private Button GeneratePropertyButton()
        {
            Button button = new Button();
            button.Content = "+";
            button.Width = 20;
            button.Height = 20;
            return button;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clickedButton = sender as Button;
            Clipboard.SetText(clickedButton.Content.ToString());
            ShowNotification(cNotification, "Panoya Kopyalandı.");
        }

        private void AddingButtonClick(object sender, RoutedEventArgs e)
        {
            details.Visibility = Visibility.Hidden;
            recordList.SelectedItem = recordList.Items.IndexOf(-1);
            additionPage.Visibility = Visibility.Visible;
        }

        public void AddtoRecordList(Record item)
        {
            records.Add(item);
            view.Refresh();
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (recordList.SelectedIndex >= 0)
            {
                dNotification.Text = "Kayıt Silinsin Mi?";
                confirmation.Visibility = Visibility.Visible;
            }
            else{
                ShowNotification(dNotification, "Silmek İstediğiniz Kaydı Seçin.");
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            confirmation.Visibility = Visibility.Hidden;
            dNotification.Text = "";
        }

        private void OkayButtonClick(object sender, RoutedEventArgs e)
        {
            isSelectable = false;
            Database.Records.DeleteFromTableMD5(selectedRecord.Hash);
            records.Remove(selectedRecord);
            view.Refresh();
            CancelButtonClick(sender, e);
            ShowNotification(dNotification, "Silindi.");
            isSelectable = true;
        }

        private async Task ShowNotification(TextBlock tbNotification, string notification)
        {
            tbNotification.Text = notification;
            await Task.Delay(2000);
            tbNotification.Text = "";
        }

        private void clearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchText.Text = "";
        }

        private void clearLabel_Click(object sender, RoutedEventArgs e)
        {
            Label.Text = "";
        }

        private void editMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            a = e;
            TextBlock t = sender as TextBlock;
            string name = t.Name;                                        // tb...
            if (recordList.SelectedIndex < 0 || string.IsNullOrEmpty(name))
            {
                ShowNotification(dNotification, "Değiştirmek İstediğiniz Kaydı Seçin.");
                return;
            }
            StackPanel sp = details.FindName("edit"+name) as StackPanel; // edittb...
            name = sp.Tag.ToString();                                    // ...
            TextBox tb = details.FindName(name) as TextBox;
            StackPanel s = details.FindName("sp" + name) as StackPanel;
            oldValue = t.Text;
            tb.Text = oldValue;
            sp.Visibility = Visibility.Visible;
            s.Visibility = Visibility.Hidden;
        }

        private void cancelEdit_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            StackPanel sp = details.FindName("edit" + tag) as StackPanel;
            StackPanel s = details.FindName("sp" + tag.Substring(2)) as StackPanel;
            sp.Visibility = Visibility.Hidden;
            s.Visibility = Visibility.Visible;
        }

        private void acceptEdit_Click(object sender, RoutedEventArgs e)
        {
            bool dontSave = false;
            TextBox tb = (sender as Button).Tag as TextBox;
            string name = tb.Name;
            StackPanel sp = details.FindName("edittb" + name) as StackPanel;
            TextBlock block = details.FindName("tb" + name) as TextBlock;
            StackPanel s = details.FindName("sp" + name) as StackPanel;
            string value = tb.Text;
            
            sp.Visibility = Visibility.Hidden;
            s.Visibility = Visibility.Visible;
            
            Button a = details.FindName("a" + name) as Button;
            Button b = details.FindName("b" + name) as Button;
            
            

            if (value.Equals(oldValue))
            {
                ShowNotification(dNotification, "Değer Aynı.");
                dontSave = true;
            }
            
            if (string.IsNullOrWhiteSpace(value) && (name.Equals("Site") || name.Equals("Mail")))
            {
                ShowNotification(dNotification, "Bu Alan Boş Olamaz.");
                value = "1";
                dontSave = true;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                value = "";
                a.Visibility = Visibility.Visible;
                b.Visibility = Visibility.Hidden;
            }
            else
            {
                a.Visibility = Visibility.Hidden;
                b.Visibility = Visibility.Visible;
            }

            if (dontSave) return;
            
            try
            {
                selectedRecord.setValue(name, value);
                block.Text = value;
                if(name.Equals("Site")) view.Refresh();
            }
            catch (Exception)
            {
                ShowNotification(dNotification, "Kayıt Mevcut.");
            }
        }

        private void HideDetails()
        {
            HideEdit();
            details.Visibility = Visibility.Hidden;
        }
        private void HideEdit()
        {
            edittbSite.Visibility = Visibility.Hidden;
            edittbEMail.Visibility = Visibility.Hidden;
            edittbUsername.Visibility = Visibility.Hidden;
            edittbHint.Visibility = Visibility.Hidden;
            edittbLabels.Visibility = Visibility.Hidden;
        }
        private void HideDetailsInfo()
        {
            spSite.Visibility = Visibility.Hidden;
            spEMail.Visibility = Visibility.Hidden;
            spUsername.Visibility = Visibility.Hidden;
            spHint.Visibility = Visibility.Hidden;
            spLabels.Visibility = Visibility.Hidden;
        }

        private void ShowDetailsInfo()
        {
            spSite.Visibility = Visibility.Visible;
            spEMail.Visibility = Visibility.Visible;
            spUsername.Visibility = Visibility.Visible;
            spHint.Visibility = Visibility.Visible;
            spLabels.Visibility = Visibility.Visible;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            string name = "t" + (sender as Button).Name;
            string value = (details.FindName(name) as TextBlock).Text;
            Clipboard.SetText(value);
            ShowNotification(dNotification, "Panoya Kopyalandı.");
        }

        private void addPropertyClick(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (sender as Button).Tag as TextBlock;
            editMouseRightButtonDown(tb, a);
        }
    }
}
