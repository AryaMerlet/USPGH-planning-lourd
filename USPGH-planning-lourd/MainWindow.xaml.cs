using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using USPGH_planning_lourd.classes;
using System;

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
            foreach (var user in db.Users.ToList())
            {
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
    }

    private void DeleteUser_Click(object sender, RoutedEventArgs e)
    {
        if (UsersList.SelectedItem is User selectedUser)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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