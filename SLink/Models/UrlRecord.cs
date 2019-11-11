using System;

namespace SLink.Models
{
    public class UrlRecord
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string OriginalUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
