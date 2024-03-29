using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using OpenMails.Services;

namespace OpenMails.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        readonly Dictionary<IMailService, ObservableCollection<NavigationViewItemBase>> _cachedNavigationItems = new();

        [ObservableProperty]
        IMailService _selectedMailServices;

        [ObservableProperty]
        ObservableCollection<NavigationViewItemBase> _navigationViewItems;

        public ObservableCollection<IMailService> MailServices { get; } = new();

        async partial void OnSelectedMailServicesChanged(IMailService value)
        {

            
        }
    }
}