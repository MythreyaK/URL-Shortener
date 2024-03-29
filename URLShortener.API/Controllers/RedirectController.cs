using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener.DataAccess.Services;
using URLShortener.Domain.DataTransferObjects;
using URLShortener.Domain.Models;

namespace URLShortener.API.Controllers
{
    [Route("/")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly AppDBService _dbService;

        public RedirectController(AppDBService dBService) {
            _dbService = dBService;
        }

        // GET: /hash
        [HttpGet("{shortUrl}", Name = "Get")]
        public async Task<ActionResult> ResolveRedirect(string shortUrl)
        {
            // Call the handler. Returns a URL if hash exists in DB,
            // else returns null
            Redirect redirect = await _dbService.RedirectAccessedAsync(
                shortUrl,
                Request.Headers["User-Agent"].ToString(),
                Request.Headers["Referer"],
                Request.Host.ToUriComponent()
            );

            if (redirect == null) {
                return this.NotFound();
            }

            return Redirect(redirect.DestinationURL);

        }

    }
}
