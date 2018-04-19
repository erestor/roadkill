using System;

namespace Roadkill.Core.Models
{
    public class Page
    {
        public Guid ObjectId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string Tags { get; set; }

        public bool IsLocked { get; set; }
    }
}