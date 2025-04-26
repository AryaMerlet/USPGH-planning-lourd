using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd
{
    /// <summary>
    /// Interaction logic for EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        private User _user;

        public EditUserWindow(User user)
        {
            InitializeComponent();
            _user = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            FirstNameTextBox.Text = _user.first_name;
            LastNameTextBox.Text = _user.last_name;
            EmailTextBox.Text = _user.email;

            // Set the role in the combobox
            RoleComboBox.SelectedIndex = _user.IsAdmin ? 0 : 1;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    // Get the current user from the database
                    var userToUpdate = db.Users.Find(_user.Id);

                    if (userToUpdate == null)
                    {
                        MessageBox.Show("User not found in the database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Update user data
                    userToUpdate.first_name = FirstNameTextBox.Text;
                    userToUpdate.last_name = LastNameTextBox.Text;
                    userToUpdate.email = EmailTextBox.Text;
                    userToUpdate.updated_at = DateTime.Now;

                    // Update password if provided
                    if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                    {
                        userToUpdate.password = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);
                    }

                    // Save the changes
                    db.SaveChanges();

                    // Update role if changed
                    bool shouldBeAdmin = RoleComboBox.SelectedIndex == 0;
                    bool shouldBeSalarie = RoleComboBox.SelectedIndex == 1;

                    // Only process role changes if needed
                    if (shouldBeAdmin != _user.IsAdmin || shouldBeSalarie != _user.IsSalarie)
                    {
                        // Remove existing roles
                        db.Database.ExecuteSqlRaw(
                            "DELETE FROM model_has_roles WHERE model_id = {0} AND model_type = {1}",
                            userToUpdate.Id,
                            "App\\Models\\User");

                        // Add the new role
                        int roleId = shouldBeAdmin ? 1 : 2;  // 1 for admin, 2 for salarie

                        db.Database.ExecuteSqlRaw(
                            "INSERT INTO model_has_roles (role_id, model_type, model_id) VALUES ({0}, {1}, {2})",
                            roleId,
                            "App\\Models\\User",
                            userToUpdate.Id);
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}