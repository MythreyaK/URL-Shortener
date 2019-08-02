using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortner.DataAccess.Services;
using URLShortner.Domain.DataTransferObjects;
using URLShortner.Domain.Models;

namespace URLShortner.API.Controllers
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
        [HttpGet("{urlHash}", Name = "Get")]
        public async Task<ActionResult> ResolveRedirect(string urlHash)
        {
            // Call the handler. Returns a URL if hash exists in DB,
            // else returns null
            Redirect redirect = await _dbService.RedirectAccessedAsync(
                urlHash,
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
