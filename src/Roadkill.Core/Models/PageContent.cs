using System;

namespace Roadkill.Core.Models
{
    public class PageContent
    {
        public Guid Id { get; set; }

        public Page Page { get; set; }

        public string Text { get; set; }

        public string EditedBy { get; set; }

        public DateTime EditedOn { get; set; }

        public int VersionNumber { get; set; }
    }
}