using System;
using System.Threading.Tasks;
using Lands.Helpers;
using Lands.Models;
using Lands.Services;
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

        #region Variables

        public static string root_db;
        
        #endregion

        #region Constructors

        public App()
        {
            try
            {
                InitializeComponent();

                if (string.IsNullOrEmpty(Settings.Token))
                {
                    this.MainPage = new NavigationPage(new LoginPage());
                }
                else
                {
                    var mainViewModel = MainViewModel.GetInstance();

                    mainViewModel.Token.AccessToken = Settings.Token;
                    mainViewModel.Token.TokenType = Settings.TokenType;

                    mainViewModel.Lands = new LandsViewModel();
                    this.MainPage = new MasterPage();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public App(string root_DB)
        {
            InitializeComponent();

            //Set root SQLite
            root_db = root_DB;
            
            if (Settings.IsRemembered == "true")
            {
                UserLocal user;
                TokenResponse token;

                using (var conn = new SQLite.SQLiteConnection(App.root_db))
                {
                    conn.CreateTable<UserLocal>();
                    conn.CreateTable<TokenResponse>();

                    user = conn.Table<UserLocal>().FirstOrDefault();
                    token = conn.Table<TokenResponse>().FirstOrDefault();
                }

                if (token != null && token.Expires > DateTime.Now)
                {
                    var mainViewModel = MainViewModel.GetInstance();

                    mainViewModel.User = user;//sqlite
                    mainViewModel.Token = token;

                    mainViewModel.Lands = new LandsViewModel();
                    Application.Current.MainPage = new MasterPage();
                }
                else
                {
                    this.MainPage = new NavigationPage(new LoginPage());
                }
            }
            else
            {
                this.MainPage = new NavigationPage(new LoginPage());
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

        #region Public Methods


        public static Action HideLoginView
        {
            get
            {
                return new Action(() => Application.Current.MainPage =
                    new NavigationPage(new LoginPage()));
            }
        }

        public static async Task NavigateToProfile(Models.FacebookResponse profile)
        {
            if (profile == null)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                return;
            }

            var apiService = new ApiService();

            var apiSecurity = Application.Current.Resources["APISecurity"].ToString();
            var token = await apiService.LoginFacebook(
                apiSecurity,
                "/api",
                "/Users/LoginFacebook",
                profile);

            if (token == null)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                return;
            }

            var user = await apiService.GetUserByEmail(
                apiSecurity,
                "/api",
                "/Users/GetUserByEmail",
                token.TokenType,
                token.AccessToken,
                token.UserName);

            UserLocal userLocal = null;

            if (user != null)
            {
                userLocal = Converter.ToUserLocal(user);

                //Save Local User in SQLite
                using (var conn = new SQLite.SQLiteConnection(App.root_db))
                {
                    conn.CreateTable<UserLocal>();
                    conn.CreateTable<TokenResponse>();

                    conn.Insert(token);
                    conn.Insert(userLocal);
                }

            }

            var mainViewModel = MainViewModel.GetInstance();

            mainViewModel.Token = token;
            mainViewModel.User = userLocal;

            mainViewModel.Lands = new LandsViewModel();
            Application.Current.MainPage = new MasterPage();
            Settings.IsRemembered = "true";

            mainViewModel.Lands = new LandsViewModel();
            Application.Current.MainPage = new MasterPage();
        }

        #endregion
    }
}