using SharpRevit.UI.Models.Keynotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRevit.UI.ViewModels.KeynotesCreationViewModel
{
    public sealed partial class FamilyInfo_ViewModel : ObservableObject
    {

        [ObservableProperty]private string _familyTypeName;

        [ObservableProperty] private string _keynoteCode;

        [ObservableProperty] private string _familyName;

        [ObservableProperty] private string _categoryName;

        [ObservableProperty] private ElementId _elementId;
    }
}
