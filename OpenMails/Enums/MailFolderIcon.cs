namespace OpenMails.Enums;

public enum MailFolderIcon
{
    // 默认图标, 例如用户自建文件夹
    Default,       

    /// <summary>
    /// 收件箱
    /// </summary>
    Inbox, 

    /// <summary>
    /// 归档
    /// </summary>
    Archive, 

    /// <summary>
    /// 已删除
    /// </summary>
    DeletedItems, 

    /// <summary>
    /// 已发送
    /// </summary>
    SentItems, 

    /// <summary>
    /// 垃圾邮件
    /// </summary>
    JunkEmail, 

    /// <summary>
    /// 草稿箱
    /// </summary>
    Drafts, 

    /// <summary>
    /// 同步问题
    /// </summary>
    SyncIssues
}