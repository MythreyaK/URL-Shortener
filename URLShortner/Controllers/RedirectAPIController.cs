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
        private readonly AppDBService _orderService;

        public RedirectAPIController(AppDBService dBService) {
            _orderService = dBService;
        }

        // GET: api/UrlHash
        [HttpGet("{urlHash}")]
        public async Task<ActionResult<RedirectDTO>> GetByHash(string urlHash) {
            RedirectDTO redirect = await _orderService.GetRedirectByHashAsync(urlHash);

            if (redirect == null) {
                return this.NotFound();
            }

            return redirect;
        }

        // POST: redirect
        [HttpPost]
        public ActionResult<RedirectDTO> Post(Redirect newRedirect) {
            _orderService.AddRedirectAsync(newRedirect).GetAwaiter().GetResult();

            return CreatedAtAction(
                nameof(GetByHash),
                new { urlHash = newRedirect.ShortURL },
                new RedirectDTO {
                    Name = newRedirect.Name,
                    ShortURL = newRedirect.ShortURL,
                    DestinationURL = newRedirect.DestinationURL,
                    CreatedAt = newRedirect.CreatedAt,
                    ExpiresOn = newRedirect.ExpiresOn,
                    Metrics = null
                }
            );
        }


    }
}
