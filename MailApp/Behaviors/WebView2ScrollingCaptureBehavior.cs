using Microsoft.Xaml.Interactivity;
using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.Web.WebView2.Core;
using Windows.UI.Xaml.Controls;
using System.Text.Json;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls.Primitives;
using MailApp.Models.Events;

#nullable enable

namespace MailApp.Behaviors
{
    public class WebView2ScrollingCaptureBehavior : WebView2CoreWebViewBehavior
    {
        const string MessageKind = "Scroll";

        protected override void OnCoreWebView2Ok(CoreWebView2 coreWebView2)
        {
            coreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            _ = coreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                $$$"""
                document.body.addEventListener("wheel", e => {
                    window.chrome.webview.postMessage({
                        Kind: "{{{MessageKind}}}",
                        DeltaX = e.deltaX,
                        DeltaY = e.deltaY
                    });
                });
                """);
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject.CoreWebView2 is not null)
            {
                AssociatedObject.CoreWebView2.WebMessageReceived -= CoreWebView2_WebMessageReceived;
            }
        }

        private void CoreWebView2_WebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            try
            {
                var message = JsonSerializer.Deserialize<Message>(args.WebMessageAsJson);
                if (message.Kind != MessageKind)
                    return;

                Wheel?.Invoke(this, new WebView2WheelEventArgs(message.DeltaX, message.DeltaY));
            }
            catch { }
        }

        public event EventHandler<WebView2WheelEventArgs>? Wheel;

        struct Message
        {
            public string Kind;
            public double DeltaX;
            public double DeltaY;
        }
    }
}
