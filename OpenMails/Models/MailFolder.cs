using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace OpenMails.Models
{
    public class MailFolder
    {
        public MailFolder(
            string id,
            string name,
            string description,
            string parentFolderId)
        {
            Id = id;
            Name = name;
            Description = description;
            ParentFolderId = parentFolderId;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }

        public string? ParentFolderId { get; }
    }
}
