using Microsoft.Xaml.Interactivity;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

#nullable enable

namespace MailApp.Behaviors
{
    public class WebView2CoreWebViewBehavior : Behavior<WebView2>
    {

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject.CoreWebView2 is not null)
            {
                OnCoreWebView2Ok(AssociatedObject.CoreWebView2);
            }
            else
            {
                AssociatedObject.CoreWebView2Initialized += AssociatedObject_CoreWebView2Initialized;
            }
        }

        private void AssociatedObject_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            OnCoreWebView2Ok(sender.CoreWebView2);
        }

        protected virtual void OnCoreWebView2Ok(CoreWebView2 coreWebView2)
        {

        }
    }
}
