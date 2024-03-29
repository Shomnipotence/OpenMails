using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenMails.Services
{
    public class NavigationService
    {
        Frame _rootFrame;

        public NavigationService()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame is null)
            {
                rootFrame = new();
                rootFrame.NavigationFailed += RootFrame_NavigationFailed;

                Window.Current.Content = rootFrame;
            }

            _rootFrame = rootFrame;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void RootFrame_NavigationFailed(object sender, Windows.UI.Xaml.Navigation.NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        public void NavigateToLoginPage()
        {

        }

        public void NavigateToMainPage()
        {

        }
    }
}
