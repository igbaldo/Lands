using System;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Lands.Services;
using Lands.Views;
using Xamarin.Forms;

namespace Lands.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiService;
        #endregion

        #region Attributes

        private string email { get; set; }
        private string password { get; set; }
        private bool isRunning { get; set; }
        private bool isRemembered { get; set; }
        private bool isEnable { get; set; }

        #endregion

        #region Properties

        public string Email {
            get { return this.email; }
            set
            {
                if (this.email != value)
                {
                    this.email = value;
                    OnPropertyChanged(nameof(Email));
                }

            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool IsRunning {
            get { return this.isRunning; }
            set
            {
                if (this.isRunning != value)
                {
                    this.isRunning = value;
                    OnPropertyChanged(nameof(IsRunning));
                }
            }
        }

        public bool IsRemembered { get; set; }

        public bool IsEnabled {
            get { return this.isEnable; }
            set
            {
                if (this.isEnable != value)
                {
                    this.isEnable = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        #endregion

        #region Command

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
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

            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Accept");
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Lands = new LandsViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new LandsPage());

            this.IsRunning = false;
            this.IsEnabled = true;

            this.Email = string.Empty;
            this.Password = string.Empty;

        }

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            this.apiService = new ApiService();

            this.IsRemembered = true;
            this.IsEnabled = true;

            this.Email = "ig.flytolive@gmail.com";
            this.Password = "admin";
        }

        #endregion

    }
}