using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;

namespace MailApp.Services
{
    public class ToastService
    {
        public ToastService()
        {

        }

        public void Toast(string title, string content)
        {
            var strings = App.Host.Services.GetRequiredService<I18nStrings>();

            var dialog = new ContentDialog()
            {
                Title = title,
                Content = content,
                CloseButtonText = strings.StringOk,
            };

            _ = dialog.ShowAsync();
        }

        public void ToastSucceed(string content)
        {
            var strings = App.Host.Services.GetRequiredService<I18nStrings>();
            var title = strings.StringSucceed;

            Toast(title, content);
        }

        public void ToastWarning(string content)
        {
            var strings = App.Host.Services.GetRequiredService<I18nStrings>();
            var title = strings.StringWarning;

            Toast(title, content);
        }

        public void ToastError(string content)
        {
            var strings = App.Host.Services.GetRequiredService<I18nStrings>();
            var title = strings.StringError;

            Toast(title, content);
        }
    }
}
