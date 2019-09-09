using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace URLShortener.Domain.Models
{
    public class Redirect : IValidatableObject
    {
        [Key]
        public ulong RedirectID { get; set; }

        [RegularExpression(@"[a-zA-Z0-9]", ErrorMessage ="Short URL must satisfy regex" +
            "[a-zA-Z0-9]")]
        [StringLength(10, MinimumLength=1, ErrorMessage = "Length must lie" +
            "in range [1, 10]")]
        [DataType(DataType.Text)]
        public string ShortUrl { get; set; }

        [Required, DataType(DataType.Text), StringLength(20, MinimumLength = 1,
            ErrorMessage = "Name must belong to range [1, 20] (both inclusive)")]
        public string Name { get; set; }

        [Required, Url, DataType(DataType.Url)]
        public string DestinationURL { get; set; }

        [Required, Timestamp, DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Timestamp, DataType(DataType.DateTime)]
        public DateTime? ExpiresOn { get; set; }


        public ulong TotalClicks { get; set; }

        public ICollection<RedirectMetric> Metrics { get; set; }



        [JsonConstructor]
        public Redirect(string destinationURL, string name, DateTime? expiresOn) {

            this.CreatedAt = this.ToNearestMinute(DateTime.UtcNow);
            this.ExpiresOn = expiresOn;
            this.DestinationURL = destinationURL;
            this.Name = name;
        }

        // To remove the milliseconds part.
        private DateTime ToNearestMinute(DateTime inp) {

            return inp.AddTicks(-( inp.Ticks % TimeSpan.TicksPerMinute ));
        }

        // TODO: Use a smarter hashing function
        public void SetHash() {
            this.ShortUrl = this.RedirectID.ToString();
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {

            if (this.ExpiresOn.HasValue && (this.ExpiresOn.Value - this.CreatedAt).TotalHours < 48) {

                yield return new ValidationResult("ExpiresOn must be at the least 48 hours " +
                    "after the creation time.", new[] { "ExpiresOn" });
            }
        }


    }
}
