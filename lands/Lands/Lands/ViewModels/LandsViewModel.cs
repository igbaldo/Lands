using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
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

        private ObservableCollection<LandItemViewModel> lands { get; set; }
        private bool isRefreshing { get; set; }
        private string filter { get; set; }
        private List<Land> landsList { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<LandItemViewModel> Lands
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

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set
            {
                if (this.isRefreshing != value)
                {
                    this.isRefreshing = value;
                    OnPropertyChanged(nameof(IsRefreshing));
                }

            }
        }

        public string Filter
        {
            get { return this.filter; }
            set
            {
                if (this.filter != value)
                {
                    this.filter = value;
                    OnPropertyChanged(nameof(Filter));
                    this.Search();
                }

            }
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadLands);
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(Search);
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
            this.IsRefreshing = true;
            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRefreshing = false;
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
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }

            this.landsList = (List<Land>)response.Result;
            this.Lands = new ObservableCollection<LandItemViewModel>(this.ToLandItemViewModel());
            this.IsRefreshing = false;
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(this.Filter))
            {
                this.Lands = new ObservableCollection<LandItemViewModel>(this.ToLandItemViewModel());
            }
            else
            {
                this.Lands = new ObservableCollection<LandItemViewModel>(
                    this.ToLandItemViewModel().Where(x => x.Name.ToLower().Contains(this.Filter.ToLower()) 
                                           || x.Capital.ToLower().Contains(this.Filter.ToLower())));
            }
        }

        private IEnumerable<LandItemViewModel> ToLandItemViewModel()
        {
            return MainViewModel.GetInstance().Lands.landsList.Select(l => new LandItemViewModel
            {
                Alpha2Code = l.Alpha2Code,
                Alpha3Code = l.Alpha3Code,
                AltSpellings = l.AltSpellings,
                Area = l.Area,
                Borders = l.Borders,
                CallingCodes = l.CallingCodes,
                Capital = l.Capital,
                Cioc = l.Cioc,
                Currencies = l.Currencies,
                Demonym = l.Demonym,
                Flag = l.Flag,
                Gini = l.Gini,
                Languages = l.Languages,
                Latlng = l.Latlng,
                Name = l.Name,
                NativeName = l.NativeName.Trim(),
                NumericCode = l.NumericCode,
                Population = l.Population,
                Region = l.Region,
                RegionalBlocs = l.RegionalBlocs,
                Subregion = l.Subregion,
                Timezones = l.Timezones,
                TopLevelDomain = l.TopLevelDomain,
                Translations = l.Translations,
            });
        }

        #endregion
    }
}
