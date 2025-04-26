using System;
using System.Windows;
using USPGH_planning_lourd.services;

namespace USPGH_planning_lourd
{
    public partial class AddUserWindow : Window
    {
        private readonly UserService _userService;

        public AddUserWindow()
        {
            InitializeComponent();
            _userService = new UserService();

            // Set the default role
            RoleComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string selectedRole = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "salarie";

                // Create the user
                _userService.CreateUser(
                    FirstNameTextBox.Text,
                    LastNameTextBox.Text,
                    EmailTextBox.Text,
                    PasswordBox.Password,
                    selectedRole
                );

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}