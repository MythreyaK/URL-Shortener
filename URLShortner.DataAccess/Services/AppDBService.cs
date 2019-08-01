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

        private IQueryable<Redirect> GetRedirectByHash(string urlHash, bool isTracked, bool withMetrics) {

            IQueryable<Redirect> redirectItem = _context.RedirectsSet;

            redirectItem = isTracked ? redirectItem.AsTracking() : redirectItem.AsNoTracking();
            redirectItem = withMetrics ? redirectItem.Include(ri => ri.Metrics) : redirectItem;

            return redirectItem;
         
            //.AsNoTracking().Where(ri => ri.ShortURL == urlHash);


            //if (withMetrics) {
            //    redirectItem = _context.RedirectsSet.AsNoTracking()
            //        .Where(ri => ri.ShortURL == urlHash)
            //        .Include(ri => ri.Metrics);
            //}

            //else {
            //    redirectItem = _context.RedirectsSet.AsNoTracking()
            //        .Where(ri => ri.ShortURL == urlHash);
            //}

        }


        public async Task<RedirectDTO> GetRedirectByHashAsync(string urlHash) {
            RedirectDTO redirect = await (
                //from redirectItem in this._context.RedirectsSet.AsNoTracking()
                //where redirectItem.ShortURL == urlHash
                from redirectItem in GetRedirectByHash(urlHash, false, true)
                select new RedirectDTO {
                    Name = redirectItem.Name,
                    ShortURL = redirectItem.ShortURL,
                    DestinationURL = redirectItem.DestinationURL,
                    CreatedAt = redirectItem.CreatedAt,
                    ExpiresOn = redirectItem.ExpiresOn,
                    Metrics = (
                        from redirectMetricItem in redirectItem.Metrics
                        select new RedirectMetricDTO {
                            UserAgent = redirectMetricItem.UserAgent,
                            ClickedOn = redirectMetricItem.ClickedOn
                        }
                    )
                }
            ).FirstOrDefaultAsync();

            return redirect;
        }

        public Task<List<RedirectDTO>> GetAllRedirectsAsync() {
            return (
                from redirectItem in _context.RedirectsSet.AsNoTracking()
                select new RedirectDTO {
                    Name = redirectItem.Name,
                    ShortURL = redirectItem.ShortURL,
                    DestinationURL = redirectItem.DestinationURL,
                    CreatedAt = redirectItem.CreatedAt,
                    ExpiresOn = redirectItem.ExpiresOn,
                    Metrics = (
                        from redirectMetricItem in redirectItem.Metrics
                        select new RedirectMetricDTO {
                            UserAgent = redirectMetricItem.UserAgent,
                            ClickedOn = redirectMetricItem.ClickedOn
                        }
                    )
                }
            ).ToListAsync();

        }

        public async Task<Redirect> AddRedirectAsync(Redirect newRedirect) {
            await _context.AddAsync(newRedirect);
            newRedirect.SetHash();
            await _context.SaveChangesAsync();
            return newRedirect;
        }

        public async Task<Redirect> ResolveAndUpdateRedirectAsync(string urlHash, string ua, string referrer) {
            var item = GetRedirectByHash(urlHash, true, false).FirstOrDefault();

            if (item != null) {
                item.Metrics.Add(
                    new RedirectMetric {
                        ClickedOn = DateTime.UtcNow,
                        Referrer = referrer,
                        UserAgent = ua
                    }
                );
            }

            await _context.SaveChangesAsync();
            return item;
        }


        

    }
}
