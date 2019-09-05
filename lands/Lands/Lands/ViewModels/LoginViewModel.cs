using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lands.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Attributes

        private string email { get; set; }
        private string password { get; set; }
        private bool isRunning { get; set; }
        private bool isRemembered { get; set; }
        private bool isEnable { get; set; }

        #endregion

        #region Properties

        public string Email { get; set; }

        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    OnPropertyChanged(nameof(Password));
                }

            }
        }

        public bool IsRunning {
            get { return this.isRunning; }
            set
            {
                var backingField = this.isRunning;
                SetValue(ref backingField, value);
                this.isRunning = backingField;
            }
        }

        public bool IsRemembered { get; set; }

        public bool IsEnabled {
            get { return this.isEnable; }
            set
            {
                var backingField = this.isEnable;
                SetValue(ref backingField, value);
                this.isEnable = backingField;
            }
        }

        #endregion

        #region Command

        public ICommand LoginCommand
        {
            get { return new RelayCommand(Login); }
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email) || string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter an email or password",
                    "Accept");

                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            if (this.Email != "ig.flytolive@gmail.com" || this.Password != "admin")
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Email or Password incorrect",
                    "Accept");

                this.Password = string.Empty;

                return;
            }

            await Application.Current.MainPage.DisplayAlert(
                "Ok",
                "Login",
                "Accept");

            this.IsRunning = false;
            this.IsEnabled = true;
        }

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            this.IsRemembered = true;
            this.IsEnabled = true;
        }

        #endregion

    }
}