using System.Collections.Generic;
using System.Text;
using OpenMails.Enums;

#nullable enable

namespace OpenMails.Models
{
    public record class MailFolder
    {
        public MailFolder(
            string id,
            string name,
            string description,
            MailFolderIcon icon,
            string parentFolderId,
            bool isEmpty)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
            ParentFolderId = parentFolderId;
            IsEmpty = isEmpty;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public MailFolderIcon Icon { get; }

        public string ParentFolderId { get; }
        public bool IsEmpty { get; }
    }
}
