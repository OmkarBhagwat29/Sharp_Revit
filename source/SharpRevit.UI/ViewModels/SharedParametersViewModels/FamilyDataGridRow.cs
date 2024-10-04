using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRevit.UI.ViewModels.SharedParametersViewModels
{
    public sealed partial class FamilyDataGridRow : ObservableObject
    {
        [ObservableProperty] private bool _isFamilySelected;
        [ObservableProperty] private string _family;
    }
}
