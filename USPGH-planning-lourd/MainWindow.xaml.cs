using System.Collections.ObjectModel;
using System.Windows;
using USPGH_planning_lourd.classes;
using System.Linq;

namespace USPGH_planning_lourd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<User> Users { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Users = new ObservableCollection<User>();
            LoadUsers();
            DataContext = this;
        }

        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                Users.Clear();
                var userList = db.Users.ToList();

                // Load roles for each user
                foreach (var user in userList)
                {
                    try
                    {
                        // Set the role flags
                        db.LoadUserRoles(user);
                        Users.Add(user);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error loading roles for user {user.email}: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        // Add the user anyway, but without roles
                        Users.Add(user);
                    }
                }
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow();
            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersList.SelectedItem is User selectedUser)
            {
                var editUserWindow = new EditUserWindow(selectedUser);
                if (editUserWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersList.SelectedItem is User selectedUser)
            {
                // Check if the user is an admin
                if (selectedUser.IsAdmin)
                {
                    MessageBox.Show("You cannot delete an administrator!",
                        "Operation not allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        db.Users.Remove(selectedUser);
                        db.SaveChanges();
                    }
                    LoadUsers();
                }
            }
        }
    }
}