using Microsoft.Web.WebView2.Core;

#nullable enable

namespace MailApp.Behaviors
{
    public class WebView2DisableBodyScrollBarBehavior : WebView2CoreWebViewBehavior
    {
        protected override void OnCoreWebView2Ok(CoreWebView2 coreWebView2)
        {
            coreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
        }

        private void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            _ = sender.ExecuteScriptAsync(
                """
                document.body.style.overflow = "hidden";
                """);
        }
    }
}
