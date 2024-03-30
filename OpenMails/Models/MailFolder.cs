using System.Collections.Generic;
using System.Text;

#nullable enable

namespace OpenMails.Models
{
    public record class MailFolder
    {
        public MailFolder(
            string id,
            string name,
            string description,
            string parentFolderId,
            bool isEmpty)
        {
            Id = id;
            Name = name;
            Description = description;
            ParentFolderId = parentFolderId;
            IsEmpty = isEmpty;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }

        public string ParentFolderId { get; }
        public bool IsEmpty { get; }
    }
}
