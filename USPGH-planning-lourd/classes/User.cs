using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USPGH_planning_lourd.classes
{
    [Table("users")]
    public class User : INotifyPropertyChanged
    {
        private int id;
        private string firstName;
        private string lastName;
        private string emailAddress;
        private DateTime? emailVerifiedAt;
        private string userPassword;
        private DateTime? createdAt;
        private DateTime? updatedAt;
        private string role;

        [Key]
        [Column("id")]
        public int Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        [Column("first_name")]
        public string first_name
        {
            get => firstName;
            set
            {
                if (firstName != value)
                {
                    firstName = value;
                    OnPropertyChanged(nameof(first_name));
                }
            }
        }

        [Column("last_name")]
        public string last_name
        {
            get => lastName;
            set
            {
                if (lastName != value)
                {
                    lastName = value;
                    OnPropertyChanged(nameof(last_name));
                }
            }
        }

        [Column("email")]
        public string email
        {
            get => emailAddress;
            set
            {
                if (emailAddress != value)
                {
                    emailAddress = value;
                    OnPropertyChanged(nameof(email));
                }
            }
        }

        [Column("email_verified_at")]
        public DateTime? email_verified_at
        {
            get => emailVerifiedAt;
            set
            {
                if (emailVerifiedAt != value)
                {
                    emailVerifiedAt = value;
                    OnPropertyChanged(nameof(email_verified_at));
                }
            }
        }

        [Column("password")]
        public string password
        {
            get => userPassword;
            set
            {
                if (userPassword != value)
                {
                    userPassword = value;
                    OnPropertyChanged(nameof(password));
                }
            }
        }

        [Column("created_at")]
        public DateTime? created_at
        {
            get => createdAt;
            set
            {
                if (createdAt != value)
                {
                    createdAt = value;
                    OnPropertyChanged(nameof(created_at));
                }
            }
        }

        [Column("updated_at")]
        public DateTime? updated_at
        {
            get => updatedAt;
            set
            {
                if (updatedAt != value)
                {
                    updatedAt = value;
                    OnPropertyChanged(nameof(updated_at));
                }
            }
        }

        [NotMapped]
        public string Role
        {
            get => role;
            set
            {
                if (role != value)
                {
                    role = value;
                    OnPropertyChanged(nameof(Role));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}