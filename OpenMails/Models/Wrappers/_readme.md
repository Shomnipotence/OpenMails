当前文件夹用来存储 Model 或者其他东西的包装类, 一般用于 UI 使用.

1. MailAuthServiceWrapper: IMailAuthService 虽提供了登陆方法, 但给 UI 使用时, 一般是提供一个 Command, 执行时将其保存到当前登陆状态,
   所以在这里对齐进行包装

2. MailFolderWrapper: MailFolder 只有邮件文件夹的最基本信息, 不包含父子关系, 当需要加载父子关系的时候, 使用当前包装. 