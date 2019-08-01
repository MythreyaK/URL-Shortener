using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortner.Domain.Models
{
    public class RedirectMetric
    {
        [Key]
        public ulong MetricID { get; set; }

        public ulong RedirectID { get; set; }

        public Redirect Redirect { get; set; }

        public string UserAgent { get; set; }

        //[Timestamp, DataType(DataType.DateTime)]
        //public DateTime Day { get; set; }

        [Timestamp, DataType(DataType.Date)]
        public DateTime ClickedOn { get; set; }

        [Url, DataType(DataType.Url)]
        public string Referrer { get; set; }

    }
}
