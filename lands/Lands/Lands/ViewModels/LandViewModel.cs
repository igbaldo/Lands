using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Lands.Models;

namespace Lands.ViewModels
{
    public class LandViewModel :BaseViewModel
    {
        #region Attributtes

        private ObservableCollection<Border> borders;
        private ObservableCollection<Currency> currencies;
        private ObservableCollection<Language> languages;

        #endregion

        #region Properties

        public Land Land { get; set; }

        public ObservableCollection<Border> Borders
        {
            get { return this.borders; }
            set
            {
                if (this.borders != value)
                {
                    this.borders = value;
                    OnPropertyChanged(nameof(Borders));
                }

            }
        }

        public ObservableCollection<Currency> Currencies
        {
            get { return this.currencies; }
            set
            {
                if (this.currencies != value)
                {
                    this.currencies = value;
                    OnPropertyChanged(nameof(Currencies));
                }

            }
        }

        public ObservableCollection<Language> Languages
        {
            get { return this.languages; }
            set
            {
                if (this.languages != value)
                {
                    this.languages = value;
                    OnPropertyChanged(nameof(Languages));
                }

            }
        }

        #endregion

        #region Constructors

        public LandViewModel(Land land)
        {
            this.Land = land;
            this.LoadBorders();
            this.Currencies = new ObservableCollection<Currency>(this.Land.Currencies);
            this.Languages = new ObservableCollection<Language>(this.Land.Languages);
        }

        #endregion

        #region Methods


        private void LoadBorders()
        {   
            this.Borders = new ObservableCollection<Border>();

            foreach (var border in this.Land.Borders)
            {
                var land = MainViewModel.GetInstance().LandsList.FirstOrDefault(x => x.Alpha3Code == border);

                if (land != null)
                {
                    this.Borders.Add(
                        new Border()
                    {
                            Code = land.Alpha3Code,
                            Name = land.Name
                    });
                }

            }
        }

        #endregion

    }
}
