﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using OpenMails.Services;
using OpenMails.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenMails.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private readonly NavigationService _navigationService;

        public LoginPage(
            LoginPageViewModel viewModel,
            NavigationService navigationService)
        {
            _navigationService = navigationService;

            DataContext = this;
            ViewModel = viewModel;

            this.InitializeComponent();

        }

        public LoginPageViewModel ViewModel { get; }

        public void OnLoginCompleted()
        {
            _navigationService.NavigateToMainPage();
        }
    }
}
