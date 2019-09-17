using System;
using System.Windows.Input;
using Android;
using Android.Content;
using Android.Content.PM;
using GalaSoft.MvvmLight.Command;
using Lands.Domain;
using Lands.Helpers;
using Lands.Models;
using Lands.Services;
using Lands.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Lands.ViewModels
{
    public class MyProfileViewModel : BaseViewModel
    {
        #region Services

        private ApiService apiService;

        #endregion

        #region Attributes
        private bool isRunning;
        private bool isEnabled;
        private ImageSource imageSource;
        private MediaFile file;
        #endregion

        #region Properties

        public UserLocal User { get; set; }

        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set
            {
                if (this.imageSource != value)
                {
                    this.imageSource = value;
                    OnPropertyChanged(nameof(ImageSource));
                }

            }
        }

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

        #region Constructor

        public MyProfileViewModel()
        {
            this.apiService = new ApiService();
            this.User = MainViewModel.GetInstance().User;
            this.ImageSource = this.User.ImageFullPath;
            this.IsEnabled = true;

        }

        #endregion

        #region Commands

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        public ICommand ChangePasswordCommand
        {
            get
            {
                return new RelayCommand(ChangePassword);
            }
        }

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }

        #endregion

        #region Private Methods

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.User.FirstName))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.FirstNameValidation,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.User.LastName))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.LastNameValidation,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.User.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EmailValidation,
                    Languages.Accept);
                return;
            }

            if (!RegexUtilities.IsValidEmail(this.User.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EmailValidation2,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.User.Telephone))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PhoneValidation,
                    Languages.Accept);
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var checkConnetion = await this.apiService.CheckConnection();
            if (!checkConnetion.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    checkConnetion.Message,
                    Languages.Accept);
                return;
            }

            byte[] imageArray = null;

            if (this.file != null)
                imageArray = FilesHelper.ReadFully(this.file.GetStream());

            var userDomain = Converter.ToUserDomain(this.User, imageArray);

            var apiSecurity = Application.Current.Resources["APISecurity"].ToString();

            var response = await this.apiService.Put(
                apiSecurity,
                "/api",
                "/Users",
                MainViewModel.GetInstance().Token.TokenType,
                MainViewModel.GetInstance().Token.AccessToken,
                userDomain);

            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            User user = await this.apiService.GetUserByEmail(apiSecurity,
                "/api",
                "/users/GetUserByEmail",
                MainViewModel.GetInstance().Token.TokenType,
                MainViewModel.GetInstance().Token.AccessToken,
                this.User.Email);

            var userLocal = Converter.ToUserLocal(user);

            MainViewModel.GetInstance().User = userLocal;

            using (var conn = new SQLite.SQLiteConnection(App.root_db))
            {
                conn.Update(userLocal);
            }
            
            this.IsRunning = false;
            this.IsEnabled = true;

            await App.Navigator.PopAsync();
        }

        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable &&
                CrossMedia.Current.IsTakePhotoSupported)
            {
                var source = await Application.Current.MainPage.DisplayActionSheet(
                    Languages.SourceImageQuestion,
                    Languages.Cancel,
                    null,
                    Languages.FromGallery,
                    Languages.FromCamera);

                if (source == null || source == Languages.Cancel)
                {
                    this.file = null;
                    return;
                }

                if (source == Languages.FromCamera)
                {
                    this.file = await CrossMedia.Current.TakePhotoAsync(
                        new StoreCameraMediaOptions
                        {
                            Directory = "Sample",
                            Name = "test.jpg",
                            PhotoSize = PhotoSize.Small,
                        }
                    );
                }
                else
                {
                    this.file = await CrossMedia.Current.PickPhotoAsync();
                }
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
        }

        private async void ChangePassword()
        {
            MainViewModel.GetInstance().ChangePassword = new ChangePasswordViewModel();

            await App.Navigator.PushAsync(new ChangePasswordPage());
        }

        #endregion
    }
}
