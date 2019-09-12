using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortener.DataAccess.Services;
using URLShortener.Domain.Models;
using URLShortener.Domain.DataTransferObjects;
using URLShortener.DataAccess.DataContext;

namespace URLShortener.Controllers
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
        public async Task<ActionResult<RedirectDTO>> Post(Redirect newRedirect) {
            await _dbService.AddRedirectAsync(newRedirect);
            return CreatedHelper(newRedirect);
        }

        [HttpPut("{shortUrl}")]
        public async Task<ActionResult<RedirectDTO>> PutRedirect(string shortUrl, Redirect redirect) {
            var couldPut = await _dbService.UpdateRedirect(shortUrl, redirect);

            if (couldPut == DBServiceResult.Successful) {
                return NoContent();
            }
            else if (couldPut == DBServiceResult.NotAllowed) {
                return BadRequest(new {
                    errors = new {
                        destinationURL = new string [] {
                            "The destination of the URL cannot be changed."
                        }
                    },
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    title = "Vaidation error occurred",
                    status = 400
                });
            }
            else {
                // Create a new redirect object at the given shortUrl
                // At this point, we do not need to check for duplicates
                await _dbService.AddRedirectWithHashAsync(shortUrl, redirect);
                return CreatedHelper(redirect);
            }
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
