这里存放抽出来的行为.

例如某些需要延时执行的逻辑, 那么使用 Behavior 再好不过了. 典型的例子就是头像加载.



- EmailAddressPersonPictureBehavior: 根据邮箱地址设定 PersonPicture 的图像
- WebView2CoreWebViewBehavior: WebView2 CoreWebView2 的行为
   - WebView2AutoHeightBehavior: WebView2 根据内容自动调整高度
   - WebView2RedirectScrollingBehavior: WebView2 将内部的滚动重定向到指定 ScrollViewer