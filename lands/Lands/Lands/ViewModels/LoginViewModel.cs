using System;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Lands.Models;
using Lands.Services;
using Lands.Views;
using Xamarin.Forms;

namespace Lands.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Services

        private readonly ApiService apiService;

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
                    Resources.Resource.Error,
                    Resources.Resource.EmailOrPasswordValidator,
                    Resources.Resource.Accept);

                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(
                    Resources.Resource.Error,
                    connection.Message,
                    Resources.Resource.Accept);
                return;
            }

            TokenResponse token = await this.apiService.GetToken("https://landsapi2019.azurewebsites.net", this.Email, this.Password);

            if (token == null)
            {
                this.IsRunning = false;
                this.IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(
                    Resources.Resource.Error,
                    Resources.Resource.SomethingWrong,
                    Resources.Resource.Accept);
                return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                this.IsRunning = false;
                this.IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(
                    Resources.Resource.Error,
                    token.ErrorDescription,
                    Resources.Resource.Accept);

                this.Password = String.Empty;

                return;
            }

            var mainViewModel = MainViewModel.GetInstance();

            mainViewModel.Token = token;
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

            this.Email = "admin@admin.com";
            this.Password = "123456";
        }

        #endregion

    }
}