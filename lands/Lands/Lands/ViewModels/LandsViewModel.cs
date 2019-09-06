using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Lands.Models;
using Lands.Services;
using Xamarin.Forms;

namespace Lands.ViewModels
{
    public class LandsViewModel : BaseViewModel
    {
        #region Services

        private readonly ApiService apiService;

        #endregion
        #region Attributes

        public ObservableCollection<Land> lands { get; set; }


        #endregion

        #region Properties

        public ObservableCollection<Land> Lands
        {
            get { return this.lands; }
            set
            {
                if (this.lands != value)
                {
                    this.lands = value;
                    OnPropertyChanged(nameof(Lands));
                }

            }
        }

        #endregion

        #region Constructor

        public LandsViewModel()
        {
            apiService = new ApiService();
            this.LoadLands();
        }

        #endregion

        #region Methods

        private async void LoadLands()
        {
            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Accept");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }

            var response = await this.apiService.GetList<Land>(
                "https://restcountries.eu"
                , "/rest"
                , "/v2/all");

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }

            var list = (List<Land>)response.Result;
            this.Lands = new ObservableCollection<Land>(list);
        }

        #endregion
    }
}
