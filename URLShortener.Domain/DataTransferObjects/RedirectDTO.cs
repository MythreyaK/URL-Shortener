using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.Models;

namespace URLShortener.Domain.DataTransferObjects
{
    public class RedirectDTO
    {

        public string ShortUrl { get; set; }

        public string Name { get; set; }

        public string DestinationURL { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresOn { get; set; }

        public ulong TotalClicks { get; set; }

        public IEnumerable<RedirectMetricDTO> Metrics { get; set; }
    }
}
