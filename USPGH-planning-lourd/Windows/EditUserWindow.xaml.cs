using System;
using System.Windows;
using USPGH_planning_lourd.classes;
using USPGH_planning_lourd.services;

namespace USPGH_planning_lourd
{
    /// <summary>
    /// Interaction logic for EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        private User _user;
        private UserService _userService;

        public EditUserWindow(User user)
        {
            InitializeComponent();
            _user = user;
            _userService = new UserService();
            LoadUserData();
        }

        private void LoadUserData()
        {
            FirstNameTextBox.Text = _user.first_name;
            LastNameTextBox.Text = _user.last_name;
            EmailTextBox.Text = _user.email;

            // Make sure the IsAdmin and IsSalarie properties are properly initialized
            using (var db = new AppDbContext())
            {
                db.LoadUserRoles(_user);
            }

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
                    bool isCurrentlyAdmin = userToUpdate.IsAdmin;

                    // Load current roles
                    db.LoadUserRoles(userToUpdate);

                    // Only process role changes if needed
                    if (shouldBeAdmin != isCurrentlyAdmin)
                    {
                        // Remove existing roles
                        foreach (var role in userToUpdate.Roles.ToList())
                        {
                            _userService.RemoveRoleFromUser(userToUpdate.Id, role.Name);
                        }

                        // Add the new role
                        string newRole = shouldBeAdmin ? "admin" : "salarie";
                        _userService.AssignRoleToUser(userToUpdate.Id, newRole);
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