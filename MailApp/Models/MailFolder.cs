using System.Collections.Generic;
using System.Text;
using LiteDB;
using MailApp.Abstraction;
using MailApp.Enums;

#nullable enable

namespace MailApp.Models
{
    public record class MailFolder : IIdentifiable, IParentIdentifiable
    {
        [BsonCtor]
        public MailFolder(
            string id,
            string name,
            string description,
            MailFolderIcon icon,
            string parentId,
            bool isCommonFolder,
            bool isEmpty)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
            ParentId = parentId;
            IsCommonFolder = isCommonFolder;
            IsEmpty = isEmpty;
        }

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 图标
        /// </summary>
        public MailFolderIcon Icon { get; set; }

        /// <summary>
        /// 父文件夹 ID
        /// </summary>
        public string ParentId { get; set; } = string.Empty;

        /// <summary>
        /// 是公共文件夹 (例如收件箱, 存档, 垃圾邮箱等非用户自建文件夹)
        /// </summary>
        public bool IsCommonFolder { get; set; }

        /// <summary>
        /// 是空的
        /// </summary>
        public bool IsEmpty { get; set; }

        object IParentIdentifiable.ParentId => ParentId;

        object IIdentifiable.Id => Id;

        public static void Populate(MailFolder from, MailFolder to)
        {
            to.Name = from.Name;
            to.Description = from.Description;
            to.Icon = from.Icon;
            to.ParentId = from.ParentId;
            to.IsCommonFolder = from.IsCommonFolder;
            to.IsEmpty = from.IsEmpty;
        }
    }
}
