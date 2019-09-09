using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.DataAccess.DataContext;
using URLShortener.Domain.DataTransferObjects;
using URLShortener.Domain.Models;

namespace URLShortener.DataAccess.Services
{
    public enum DBServiceResult {
        NotAllowed,
        Successful,
        NotFound,
    }

    public class AppDBService
    {
        private readonly AppDbContext _context;

        public AppDBService(AppDbContext context) {
            _context = context;
        }

        private IQueryable<Redirect> GetRedirectByHash(string shortUrl, bool track, bool withMetrics) {

            var redirectItem = _context.RedirectsSet
                .AsNoTracking()
                .Where(ri => ri.ShortUrl == shortUrl);

            redirectItem = track ? redirectItem.AsTracking() : redirectItem.AsNoTracking();
            redirectItem = withMetrics ? redirectItem.Include(ri => ri.Metrics) : redirectItem;

            return redirectItem;

            // Some comments for info
            //.AsNoTracking().Where(ri => ri.ShortURL == shortUrl);

            //if (withMetrics) {
            //    redirectItem = _context.RedirectsSet.AsNoTracking()
            //        .Where(ri => ri.ShortURL == shortUrl)
            //        .Include(ri => ri.Metrics);
            //}

            //else {
            //    redirectItem = _context.RedirectsSet.AsNoTracking()
            //        .Where(ri => ri.ShortURL == shortUrl);
            //}

        }

        private RedirectDTO GenerateDTO(Redirect redirectItem) {
            // TODO: Use a mapper
            return new RedirectDTO {
                Name = redirectItem.Name,
                ShortUrl = redirectItem.ShortUrl,
                DestinationURL = redirectItem.DestinationURL,
                CreatedAt = redirectItem.CreatedAt,
                ExpiresOn = redirectItem.ExpiresOn,
                TotalClicks = redirectItem.TotalClicks,
                Metrics = (
                    from redirectMetricItem in redirectItem.Metrics
                    select new RedirectMetricDTO {
                        UserAgent = redirectMetricItem.UserAgent,
                        ClickedOn = redirectMetricItem.ClickedOn,
                        Referrer = redirectMetricItem.Referrer,
                        RemoteIP = redirectMetricItem.RemoteIP
                    }
                )
            };
        }


        public Task<RedirectDTO> GetRedirectByHashAsync(string shortUrl) {
            return (
                from redirectItem in GetRedirectByHash(shortUrl, false, true)
                select GenerateDTO(redirectItem)
            ).FirstOrDefaultAsync();
        }

        public Task<List<RedirectDTO>> GetAllRedirectsAsync() {
            return (
                from redirectItem in _context.RedirectsSet.AsNoTracking()
                select GenerateDTO(redirectItem)
            ).ToListAsync();

        }

        public async Task<Redirect> AddRedirectAsync(Redirect newRedirect) {
            await _context.AddAsync(newRedirect);
            newRedirect.SetHash();
            await _context.SaveChangesAsync();
            return newRedirect;
        }

        public async Task<Redirect> AddRedirectWithHashAsync(string shortUrl, Redirect newRedirect) {
            await _context.AddAsync(newRedirect);
            newRedirect.ShortUrl = shortUrl;
            await _context.SaveChangesAsync();
            return newRedirect;
        }

        public async Task<Redirect> RedirectAccessedAsync(string shortUrl, string ua, string referrer, string rip) {
            var item = GetRedirectByHash(shortUrl, true, true).FirstOrDefault();

            if (item != null) {
                item.TotalClicks++;
                item.Metrics.Add(
                    new RedirectMetric {
                        ClickedOn = DateTime.UtcNow,
                        Referrer = referrer,
                        UserAgent = ua,
                        RemoteIP = rip
                    }
                );
            }

            await _context.SaveChangesAsync();
            return item;
        }


        public async Task<bool> DeleteRedirect(string shortUrl) {
            var rItem = await GetRedirectByHash(shortUrl, true, false).FirstOrDefaultAsync();

            if (rItem != null) {
                _context.RedirectsSet.Remove(rItem);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<DBServiceResult> UpdateRedirect(string shortUrl, Redirect updatedRedirectItem) {
            var rItem = await GetRedirectByHash(shortUrl, true, false).FirstOrDefaultAsync();

            if (rItem == null) {
                return DBServiceResult.NotFound;
            }
            else {
                // Only if the new redirect has the same destination
                // URL, update info
                if (rItem.DestinationURL.Equals(updatedRedirectItem.DestinationURL)){
                    // This line can raise a validation error that is
                    // sent to the user as a 400 BadRequest
                    rItem.ExpiresOn = updatedRedirectItem.ExpiresOn;
                    rItem.Name = updatedRedirectItem.Name;
                    await _context.SaveChangesAsync();
                    return DBServiceResult.Successful;
                }
                // Could not update as DestinationURL changed
                else {
                    return DBServiceResult.NotAllowed;
                }
            }
        }
    }
}
