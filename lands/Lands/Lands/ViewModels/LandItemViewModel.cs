using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Lands.Models;
using Lands.Views;
using Xamarin.Forms;

namespace Lands.ViewModels
{
    public class LandItemViewModel : Land
    {
        #region Command

        public ICommand SelectLandCommand
        {
            get
            {
                return new RelayCommand(SelectLand);
            }
        }

        #endregion

        #region Methods

        private async void SelectLand()
        {
            MainViewModel.GetInstance().Land = new LandViewModel(this);
            await App.Navigator.PushAsync(new LandTabbedPage());
        }

        #endregion
    }
}
