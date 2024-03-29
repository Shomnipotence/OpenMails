using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using OpenMails.Services;

#nullable enable

namespace OpenMails.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        readonly Dictionary<IMailService, ObservableCollection<NavigationViewItemBase>> _cachedNavigationItems = new();

        [ObservableProperty]
        IMailService? _selectedMailService = null;

        [ObservableProperty]
        ObservableCollection<NavigationViewItemBase>? _navigationViewItems;

        public ObservableCollection<IMailService> MailServices { get; } = new();

        async partial void OnSelectedMailServiceChanged(IMailService? value)
        {
            if (value is null)
                return;

            if (!_cachedNavigationItems.TryGetValue(value, out var navigationViewItems))
                navigationViewItems = _cachedNavigationItems[value] = new();
            NavigationViewItems = navigationViewItems;

            await foreach (var folder in value.GetAllFoldersAsync(default))
            {
                navigationViewItems.Add(
                    new NavigationViewItem()
                    {
                        Content = folder.Name,
                        Tag = folder,
                    });
            }
        }
    }
}