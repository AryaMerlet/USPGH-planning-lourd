// USPGH-planning-lourd/MainWindow.xaml.cs
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd;

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

            // Get all users with their roles
            var users = db.Users.ToList();

            foreach (var user in users)
            {
                // Find role for this user
                var assignedRole = db.AssignedRoles
                    .Include(ar => ar.Role)
                    .FirstOrDefault(ar => ar.EntityId == user.Id && ar.EntityType == "App\\Models\\User");

                if (assignedRole != null)
                {
                    user.Role = assignedRole.Role.Name;
                }
                else
                {
                    user.Role = "No role";
                }

                Users.Add(user);
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
        else
        {
            MessageBox.Show("Please select a user to edit.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void DeleteUser_Click(object sender, RoutedEventArgs e)
    {
        if (UsersList.SelectedItem is User selectedUser)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete user {selectedUser.first_name} {selectedUser.last_name}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        // First delete any role assignments
                        var roleAssignments = db.AssignedRoles
                            .Where(ar => ar.EntityId == selectedUser.Id && ar.EntityType == "App\\Models\\User");

                        db.AssignedRoles.RemoveRange(roleAssignments);

                        // Then delete the user
                        var user = db.Users.Find(selectedUser.Id);
                        if (user != null)
                        {
                            db.Users.Remove(user);
                            db.SaveChanges();

                            LoadUsers();
                            MessageBox.Show("User deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("Please select a user to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}