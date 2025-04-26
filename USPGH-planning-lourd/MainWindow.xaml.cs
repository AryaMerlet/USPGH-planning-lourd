using System;
using System.Collections.ObjectModel;
using System.Windows;
using USPGH_planning_lourd.classes;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                using (var db = new AppDbContext())
                {
                    Users.Clear();

                    // Add a timeout and handle exceptions for database operations
                    var userList = db.Users
                        .AsNoTracking() // Use AsNoTracking for read-only operations to improve performance
                        .ToList();

                    // Load roles for each user
                    foreach (var user in userList)
                    {
                        try
                        {
                            // Set the role flags
                            db.LoadUserRoles(user);
                            Users.Add(user);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error loading roles for user {user.email}: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                            // Add the user anyway, but without roles
                            Users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}\n\nDetail: {ex.InnerException?.Message}",
                    "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addUserWindow = new AddUserWindow();
                if (addUserWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsersList.SelectedItem is User selectedUser)
                {
                    var editUserWindow = new EditUserWindow(selectedUser);
                    if (editUserWindow.ShowDialog() == true)
                    {
                        LoadUsers();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a user to edit", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            try
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
                            var userToDelete = db.Users.Find(selectedUser.Id);
                            if (userToDelete != null)
                            {
                                db.Users.Remove(userToDelete);
                                db.SaveChanges();
                                LoadUsers();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a user to delete", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}