using Microsoft.Xaml.Interactivity;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Microsoft.Web.WebView2.Core;
using System.Threading.Tasks;
using System;

#nullable enable

namespace OpenMails.Behaviors
{

    public class WebView2AutoHeightBehavior : WebView2CoreWebViewBehavior
    {
        /// <summary>
        /// 默认高度 (每当 WebView2 开始导航时, 都重置为此高度)
        /// </summary>
        public double DefaultHeight { get; set; } = 800;

        /// <summary>
        /// 额外高度 (最终为 WebView2 设置高度时, 会加上这个值)
        /// </summary>
        public double ExtraHeight { get; set; } = 10;

        protected override void OnCoreWebView2Ok(CoreWebView2 coreWebView2)
        {
            base.OnCoreWebView2Ok(coreWebView2);

            AssociatedObject.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            AssociatedObject.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject.CoreWebView2 is not null)
            {
                AssociatedObject.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
                AssociatedObject.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
            }
        }

        /// <summary>
        /// 导航开始, 重置高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            if (args.Cancel == true)
                return;

            AssociatedObject.Height = DefaultHeight;
        }

        /// <summary>
        /// 导航结束, 设置高度
        /// </summary>
        /// <param name="coreWebView2"></param>
        /// <param name="args"></param>
        private async void CoreWebView2_NavigationCompleted(Microsoft.Web.WebView2.Core.CoreWebView2 coreWebView2, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            var height = await GetCoreWebView2BodyHeight(coreWebView2);
            AssociatedObject.Height = height + ExtraHeight;
        }

        /// <summary>
        /// 获取网页内容高度
        /// </summary>
        /// <param name="coreWebView2"></param>
        /// <returns></returns>
        private async Task<double> GetCoreWebView2BodyHeight(CoreWebView2 coreWebView2)
        {
            var asyncOperation = coreWebView2.ExecuteScriptAsync("document.body.scrollHeight");
            var heightString = await asyncOperation.AsTask();

            return double.TryParse(heightString, out var height) ? height : double.NaN;
        }
    }
}
