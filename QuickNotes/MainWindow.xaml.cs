using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> InboundNotes { get; set; }
        public ObservableCollection<string> OutboundNotes { get; set; }
        public ObservableCollection<string> EscalationNotes { get; set; }
        public ObservableCollection<string> FollowUpNotes { get; set; }
        public ObservableCollection<string> EmailNotes { get; set; }
        public ObservableCollection<string> OfflineNotes { get; set; }
        private Button _activeButton;
        private readonly string JsonFilePath;
        private ObservableCollection<string> _currentCollection;

        public MainWindow()
        {
            InitializeComponent();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = System.IO.Path.Combine(appDataPath, "QuickNotes");
            Directory.CreateDirectory(appFolder);
            JsonFilePath = System.IO.Path.Combine(appFolder, "Notes.json");


            InboundNotes = new ObservableCollection<string> { "Listening to customer's problem with", "Supported customer by reviewing their requirements of how", "Developed plan of action to reach resolution.", "Testing to see if issue resolved.", "Implementing a possible solution.", "Confirmed issue is fully resolved." };
            OutboundNotes = new ObservableCollection<string> { "Called customer to continue support.", "Reviewed with customer the", "Educated customer on" };
            EscalationNotes = new ObservableCollection<string> { "Assisted the user with regaining login access to work systems.", "Assisted the user with resolving error issue in work systems." };
            FollowUpNotes = new ObservableCollection<string> { "Worked with customer to continue support to resolution." };
            EmailNotes = new ObservableCollection<string> { "Reached out to customer to continue support.", "Followed up with customer regarding " };
            OfflineNotes = new ObservableCollection<string> { "Reviewed client's request for support.", "Supported customer by developing plan to reach resolution.\r\n" };
            LoadNotes();
            SetActiveButton(btn_Inbound);
            DisplayNotes(InboundNotes);

        }

        private void SetActiveButton(Button button)
        {
            if (_activeButton != null)
            {
                _activeButton.Background = Brushes.White;
                _activeButton.Foreground = Brushes.Black;
            }

            _activeButton = button;
            _activeButton.Background = new SolidColorBrush(Color.FromRgb(255, 0, 255)); // Magenta
            _activeButton.Foreground = Brushes.White;
        }

        private void DisplayNotes(ObservableCollection<string> notes)
        {
            _currentCollection = notes;
            NotesItemsControl.ItemsSource = notes;
        }

        private void btn_Inbound_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btn_Inbound);
            DisplayNotes(InboundNotes);
        }

        private void btn_Outbound_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btn_Outbound);
            DisplayNotes(OutboundNotes);
        }

        private void btn_Offline_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btn_Offline);
            DisplayNotes(OfflineNotes);
        }

        private void btn_FollowUp_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btn_FollowUp);
            DisplayNotes(FollowUpNotes);
        }

        private void btn_Escalation_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btn_Escalation);
            DisplayNotes(EscalationNotes);
        }

        private void btn_Email_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btn_Email);
            DisplayNotes(EmailNotes);
        }


        private void NoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is TextBlock textBlock)
            {
                Clipboard.SetText(textBlock.Text);
            }
        }

        private void btn_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.Parent is ContextMenu contextMenu &&
                contextMenu.PlacementTarget is Button button &&
                button.Content is TextBlock textBlock)
            {
                string currentContent = textBlock.Text;
                string newContent = Microsoft.VisualBasic.Interaction.InputBox("Edit the note:", "Edit Note", currentContent);

                if (!string.IsNullOrWhiteSpace(newContent) && newContent != currentContent)
                {
                    int index = _currentCollection.IndexOf(currentContent);
                    if (index != -1)
                    {
                        _currentCollection[index] = newContent;
                        SaveNotes();
                    }
                }
            }
        }


        private void DragRegion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.Parent is ContextMenu contextMenu &&
                contextMenu.PlacementTarget is Button button &&
                button.Content is TextBlock textBlock)
            {
                string content = textBlock.Text;
                if (MessageBox.Show($"Are you sure you want to delete this note?\n\n{content}", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _currentCollection.Remove(content);
                    SaveNotes();
                }
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            string newNote = Microsoft.VisualBasic.Interaction.InputBox("Enter a new note:", "Add Note", "");
            if (!string.IsNullOrWhiteSpace(newNote))
            {
                _currentCollection.Add(newNote);
                SaveNotes();
            }
        }

        private void LoadNotes()
        {
            if (File.Exists(JsonFilePath))
            {
                string jsonString = File.ReadAllText(JsonFilePath);
                var notes = JsonSerializer.Deserialize<NotesData>(jsonString);
                InboundNotes = new ObservableCollection<string>(notes.InboundNotes);
                OutboundNotes = new ObservableCollection<string>(notes.OutboundNotes);
                EscalationNotes = new ObservableCollection<string>(notes.EscalationNotes);
                FollowUpNotes = new ObservableCollection<string>(notes.FollowUpNotes);
                EmailNotes = new ObservableCollection<string>(notes.EmailNotes);
                OfflineNotes = new ObservableCollection<string>(notes.OfflineNotes);
            }
            else
            {
                InboundNotes = new ObservableCollection<string>();
                OutboundNotes = new ObservableCollection<string>();
                EscalationNotes = new ObservableCollection<string>();
                FollowUpNotes = new ObservableCollection<string>();
                EmailNotes = new ObservableCollection<string>();
                OfflineNotes = new ObservableCollection<string>();
            }
        }

        private void SaveNotes()
        {
            var notes = new NotesData
            {
                InboundNotes = InboundNotes.ToArray(),
                OutboundNotes = OutboundNotes.ToArray(),
                EscalationNotes = EscalationNotes.ToArray(),
                FollowUpNotes = FollowUpNotes.ToArray(),
                EmailNotes = EmailNotes.ToArray(),
                OfflineNotes = OfflineNotes.ToArray()
            };

            string jsonString = JsonSerializer.Serialize(notes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(JsonFilePath, jsonString);
        }

        private void btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_CloseButton_Click_1(object sender, RoutedEventArgs e)
        {
            SaveNotes();
            Application.Current.Shutdown();
        }
    }



    public class NotesData
    {
        public string[] InboundNotes { get; set; }
        public string[] OutboundNotes { get; set; }
        public string[] EscalationNotes { get; set; }
        public string[] FollowUpNotes { get; set; }
        public string[] EmailNotes { get; set; }
        public string[] OfflineNotes { get; set; }
    }

}