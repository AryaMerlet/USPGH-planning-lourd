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
                            MessageBox.Show($"Erreur lors du chargement des rôles pour l'utilisateur {user.email}: {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

                            // Add the user anyway, but without roles
                            Users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de connexion à la base de données: {ex.Message}\n\nDétail: {ex.InnerException?.Message}",
                    "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Erreur lors de l'ajout d'un utilisateur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Veuillez sélectionner un utilisateur à modifier", "Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification de l'utilisateur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Vous ne pouvez pas supprimer un administrateur !",
                            "Opération non autorisée", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet utilisateur ?",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

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
                    MessageBox.Show("Veuillez sélectionner un utilisateur à supprimer", "Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression de l'utilisateur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}