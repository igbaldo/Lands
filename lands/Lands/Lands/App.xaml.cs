using System;
using Lands.Helpers;
using Lands.ViewModels;
using Lands.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Lands
{
    public partial class App : Application
    {
        #region Properties

        public static NavigationPage Navigator { get; set; }
        public static MasterPage Master { get; set; }

        #endregion

        #region Constructors

        public App()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(Settings.Token))
            {
                this.MainPage = new NavigationPage(new LoginPage());

            }
            else
            {
                var mainViewModel = MainViewModel.GetInstance();

                mainViewModel.Token = Settings.Token;
                mainViewModel.TokenType = Settings.TokenType;

                mainViewModel.Lands = new LandsViewModel();
                this.MainPage = new MasterPage();
            }
        }

        #endregion

        #region Methods

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #endregion
    }
}