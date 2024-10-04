using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRevit.UI.ViewModels.KeynotesCreationViewModel
{
    public partial class CategorizedKeynoteViewModel : ObservableObject
    {
        [ObservableProperty] string _category;
        [ObservableProperty] string _description;
        [ObservableProperty] ObservableCollection<CategorizedKeynoteViewModel> _keynotes = [];

    }
}
