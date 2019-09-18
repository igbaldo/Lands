using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Lands.Annotations;
using Lands.Helpers;
using Lands.Models;
using Lands.Services;
using Xamarin.Forms;

namespace Lands.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        #region Services

        private ApiService apiService;

        #endregion

        #region Attributes

        private bool isRunning;
        private bool isEnabled;

        #endregion

        #region Properties

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string Confirm { get; set; }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }

            }
        }

        public bool IsRunning
        {
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

        #endregion

        #region Commands

        public ICommand ChangePasswordCommand
        {
            get
            {
                return new RelayCommand(ChangePassword);
            }
        }

        #endregion

        #region Constructor

        public ChangePasswordViewModel()
        {
            this.apiService = new ApiService();
            this.isEnabled = true;
        }

        #endregion

        #region Methods

        private async void ChangePassword()
        {
            if (string.IsNullOrEmpty(this.NewPassword) || string.IsNullOrEmpty(this.CurrentPassword))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation,
                    Languages.Accept);
                return;
            }

            if (this.NewPassword.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation2,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Confirm))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConfirmValidation,
                    Languages.Accept);
                return;
            }

            if (this.CurrentPassword != MainViewModel.GetInstance().User.Password)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordError,
                    Languages.Accept);
                return;
            }

            if (this.NewPassword != this.Confirm)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConfirmValidation2,
                    Languages.Accept);
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
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            var request = new ChangePasswordRequest
            {
                CurrentPassword = this.CurrentPassword,
                Email = MainViewModel.GetInstance().User.Email,
                NewPassword = this.NewPassword,
            };

            var apiSecurity = Application.Current.Resources["APISecurity"].ToString();
            var response = await this.apiService.ChangePassword(
                apiSecurity,
                "/api",
                "/Users/ChangePassword",
                MainViewModel.GetInstance().Token.TokenType,
                MainViewModel.GetInstance().Token.AccessToken,
                request);

            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ErrorChangingPassword,
                    Languages.Accept);
                return;
            }

            MainViewModel.GetInstance().User.Password = this.NewPassword;

            using (var conn = new SQLite.SQLiteConnection(App.root_db))
            {
                conn.Update(MainViewModel.GetInstance().User);
            }

            this.IsRunning = false;
            this.IsEnabled = true;

            await Application.Current.MainPage.DisplayAlert(
                Languages.ConfirmLabel,
                Languages.ChagePasswordConfirm,
                Languages.Accept);
            await App.Navigator.PopAsync();
        }

        #endregion
        
    }
}
