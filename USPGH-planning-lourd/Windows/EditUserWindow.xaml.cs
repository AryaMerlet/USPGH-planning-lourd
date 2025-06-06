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
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Utilisateur introuvable dans la base de données", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Load current roles to check current status
                    db.LoadUserRoles(userToUpdate);

                    // Check role change validation
                    bool shouldBeAdmin = RoleComboBox.SelectedIndex == 0;
                    bool isCurrentlyAdmin = userToUpdate.IsAdmin;

                    // Prevent removing admin privileges
                    if (isCurrentlyAdmin && !shouldBeAdmin)
                    {
                        MessageBox.Show("Impossible de retirer les privilèges d'administrateur à cet utilisateur.\n\n" +
                                      "Pour des raisons de sécurité, vous ne pouvez pas changer un administrateur en salarié.",
                                      "Modification interdite", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        // Use cost factor 12 and $2y$ prefix to match Laravel
                        var hash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password, 12);
                        if (hash.StartsWith("$2a$") || hash.StartsWith("$2b$"))
                        {
                            hash = hash.Replace("$2a$", "$2y$").Replace("$2b$", "$2y$");
                        }
                        userToUpdate.password = hash;
                    }

                    // Save the changes
                    db.SaveChanges();

                    // Update role if changed (only allow promoting to admin, not demoting)
                    if (shouldBeAdmin != isCurrentlyAdmin)
                    {
                        // This should only happen when promoting salarié to admin
                        // (since we blocked admin to salarié above)
                        if (!isCurrentlyAdmin && shouldBeAdmin)
                        {
                            // Remove existing roles
                            foreach (var role in userToUpdate.Roles.ToList())
                            {
                                _userService.RemoveRoleFromUser(userToUpdate.Id, role.Name);
                            }

                            // Add admin role
                            _userService.AssignRoleToUser(userToUpdate.Id, "admin");
                        }
                    }
                }

                MessageBox.Show("Utilisateur modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}