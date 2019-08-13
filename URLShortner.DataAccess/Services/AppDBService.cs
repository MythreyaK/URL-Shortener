using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortner.DataAccess.DataContext;
using URLShortner.Domain.DataTransferObjects;
using URLShortner.Domain.Models;

namespace URLShortner.DataAccess.Services
{
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


    }
}
