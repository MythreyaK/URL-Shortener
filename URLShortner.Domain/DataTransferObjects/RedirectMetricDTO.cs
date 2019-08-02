using System;
using System.Collections.Generic;
using System.Text;

namespace URLShortner.Domain.DataTransferObjects
{
    public class RedirectMetricDTO
    {
        public string UserAgent { get; set; }


        public DateTime ClickedOn { get; set; }

        public string Referrer { get; set; }

        public string RemoteIP { get; set; }

        //public DateTime Day { get; set; }
    }
}
