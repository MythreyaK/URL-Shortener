using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortner.DataAccess.Services;
using URLShortner.Domain.Models;
using URLShortner.Domain.DataTransferObjects;
using URLShortner.DataAccess.DataContext;

namespace URLShortner.Controllers
{
    [Route("/api")]
    [ApiController]
    public class RedirectAPIController : ControllerBase
    {
        private readonly AppDBService _dbService;

        public RedirectAPIController(AppDBService dBService) {
            _dbService = dBService;
        }

        // GET: api/shortUrl
        [HttpGet("{shortUrl}")]
        public async Task<ActionResult<RedirectDTO>> GetByHash(string shortUrl) {
            RedirectDTO redirect = await _dbService.GetRedirectByHashAsync(shortUrl);

            if (redirect == null) {
                return this.NotFound();
            }

            return redirect;
        }

        // POST: redirect
        [HttpPost]
        public ActionResult<RedirectDTO> Post(Redirect newRedirect) {
            _dbService.AddRedirectAsync(newRedirect).GetAwaiter().GetResult();

            return CreatedHelper(newRedirect);
        }

        [HttpDelete("{shortUrl}")]
        public async Task<ActionResult> DeleteRedirect(string shortUrl) {
            bool couldDelete = await _dbService.DeleteRedirect(shortUrl);

            if (couldDelete) {
                return NoContent();
            }

            return NotFound();
        }

        private ActionResult CreatedHelper(Redirect newRedirect) {
            return CreatedAtAction(
                nameof(GetByHash),
                new { shortUrl = newRedirect.ShortUrl },
                new RedirectDTO {
                    Name = newRedirect.Name,
                    ShortUrl = newRedirect.ShortUrl,
                    DestinationURL = newRedirect.DestinationURL,
                    CreatedAt = newRedirect.CreatedAt,
                    ExpiresOn = newRedirect.ExpiresOn,
                    TotalClicks = newRedirect.TotalClicks,
                    Metrics = null
                }
            );
        }


    }
}
