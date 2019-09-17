using System;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Lands.Domain;
using Lands.Helpers;
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
        //private readonly DataService dataService;

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

        public ICommand RegisterCommand
        {
            get
            {
                return new RelayCommand(Register);
            }
        }

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            this.apiService = new ApiService();
            //this.dataService = new DataService();

            this.IsRemembered = true;
            this.IsEnabled = true;
        }

        #endregion

        #region Methods

        private async void Register()
        {
            MainViewModel.GetInstance().Register = new RegisterViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
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

            var apiSecurity = Application.Current.Resources["APISecurity"].ToString();

            TokenResponse token = await this.apiService.GetToken(apiSecurity, this.Email, this.Password);

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

            User user = await this.apiService.GetUserByEmail(apiSecurity, "/api", "/users/GetUserByEmail", this.Email);

            var userLocal = Converter.ToUserLocal(user);
            var mainViewModel = MainViewModel.GetInstance();

            mainViewModel.Token = token.AccessToken;
            mainViewModel.TokenType = token.TokenType;
            mainViewModel.User = userLocal;

            if (this.IsRemembered)
            {
                Settings.Token = token.AccessToken;
                Settings.TokenType = token.TokenType;

                //Save Local User in SQLite
                using (var conn = new SQLite.SQLiteConnection(App.root_db))
                {
                    conn.CreateTable<UserLocal>();
                    conn.Insert(userLocal);
                }
                
            }

            mainViewModel.Lands = new LandsViewModel();
            Application.Current.MainPage = new MasterPage();

            this.IsRunning = false;
            this.IsEnabled = true;

            this.Email = string.Empty;
            this.Password = string.Empty;
        }

        #endregion

    }
}