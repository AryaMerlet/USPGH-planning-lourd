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
                // Update user data
                _user.first_name = FirstNameTextBox.Text;
                _user.last_name = LastNameTextBox.Text;
                _user.email = EmailTextBox.Text;
                _user.updated_at = DateTime.Now;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    _user.password = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);
                }

                using (var db = new AppDbContext())
                {
                    db.Users.Update(_user);
                    db.SaveChanges();
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