一些应该处理的东西

1. 消息查看页面的 WebView2 滚动时没有动画, 因为调用 ScrollViewer 的 ChangeView 并允许动画的时候, 滚动手感不好
   所以关闭了, 我们应该用某种 hack 的方式, 引发 ScrollViewer 的 "PointerWheel" 事件, 然后让 ScrollViewer 自己滚
2. 计划支持 Focus/Other 选项卡
3. 邮件编辑
4. 邮件排序
5. 邮件搜索