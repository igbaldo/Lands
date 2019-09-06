using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lands.ViewModels
{
    public class MainViewModel
    {
        #region ViewModels

        public LoginViewModel Login { get; set; }

        public LandsViewModel Lands { get; set; }

        public LandViewModel Land { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            instance = this;
            this.Login = new LoginViewModel();
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
