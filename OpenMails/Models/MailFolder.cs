using System;
using System.Collections.Generic;
using System.Text;

namespace OpenMails.Models
{
    public class MailFolder
    {
        public MailFolder(
            string id,
            string name,
            string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
    }
}
