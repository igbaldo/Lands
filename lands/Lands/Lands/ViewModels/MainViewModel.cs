using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using Lands.Domain;
using Lands.Models;

namespace Lands.ViewModels
{
    public class MainViewModel
    {
        #region Properties

        public List<Land> LandsList { get; set; }

        public string Token { get; set; }

        public string TokenType { get; set; }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        public User User { get; set; }

        #endregion

        #region ViewModels

        public LoginViewModel Login { get; set; }

        public LandsViewModel Lands { get; set; }

        public LandViewModel Land { get; set; }

        public RegisterViewModel Register { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            instance = this;
            this.Login = new LoginViewModel();
            this.LoadMenu();
        }


        #endregion

        #region Methods

        private void LoadMenu()
        {
            this.Menus = new ObservableCollection<MenuItemViewModel>();

            this.Menus.Add(new MenuItemViewModel()
            {
                Icon = "ic_settings",
                PageName = "MyProfilePage",
                Title = Helpers.Languages.MyProfile
            });

            this.Menus.Add(new MenuItemViewModel()
            {
                Icon = "ic_insert_chart",
                PageName = "StaticsPage",
                Title = Helpers.Languages.Statics
            });

            this.Menus.Add(new MenuItemViewModel()
            {
                Icon = "ic_exit_to_app",
                PageName = "LoginPage",
                Title = Helpers.Languages.LogOut
            });

        }

        #endregion

        #region Singleton

        private static MainViewModel instance { get; set; }

        public static MainViewModel GetInstance()
        {
            if(instance == null)
                return new MainViewModel();

            return instance;
        }

        #endregion
    }
}
