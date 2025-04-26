// EditUserWindow.xaml.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using USPGH_planning_lourd.classes;
using USPGH_planning_lourd.services;

namespace USPGH_planning_lourd
{
    public partial class EditUserWindow : Window
    {
        private readonly UserService _userService;
        private readonly User _user;
        private string _currentRole;

        public EditUserWindow(User user)
        {
            InitializeComponent();
            _userService = new UserService();
            _user = user;

            // Populate the form with user data
            FirstNameTextBox.Text = _user.first_name;
            LastNameTextBox.Text = _user.last_name;
            EmailTextBox.Text = _user.email;

            // Get the user's role and select it in the combobox
            using (var db = new AppDbContext())
            {
                var assignedRole = db.AssignedRoles
                    .Include(ar => ar.Role)
                    .FirstOrDefault(ar => ar.EntityId == _user.Id);

                if (assignedRole != null)
                {
                    _currentRole = assignedRole.Role.Name;
                    if (_currentRole == "admin")
                    {
                        RoleComboBox.SelectedIndex = 1;
                    }
                    else
                    {
                        RoleComboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    RoleComboBox.SelectedIndex = 0;
                    _currentRole = "salarie";
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string selectedRole = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "salarie";

                // Update user information
                using (var db = new AppDbContext())
                {
                    var user = db.Users.Find(_user.Id);
                    if (user != null)
                    {
                        user.first_name = FirstNameTextBox.Text;
                        user.last_name = LastNameTextBox.Text;
                        user.email = EmailTextBox.Text;
                        user.updated_at = DateTime.Now;

                        // If password is provided, update it
                        if (!string.IsNullOrEmpty(PasswordBox.Password))
                        {
                            user.password = UserService.BCrypt.HashPassword(PasswordBox.Password);
                        }

                        db.SaveChanges();
                    }
                }

                // Update role if changed
                if (_currentRole != selectedRole)
                {
                    _userService.RemoveRoleFromUser(_user.Id, _currentRole);
                    _userService.AssignRoleToUser(_user.Id, selectedRole);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}