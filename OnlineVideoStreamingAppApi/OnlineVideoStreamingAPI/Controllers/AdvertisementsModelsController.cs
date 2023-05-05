using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVideoStreamingApp.Data;
using OnlineVideoStreamingApp.Models;

namespace OnlineVideoStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsModelsController : ControllerBase
    {
        private readonly OnlineVideoStreamingAppContext _context;

        public AdvertisementsModelsController(OnlineVideoStreamingAppContext context)
        {
            _context = context;
        }

        // GET: api/AdvertisementsModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdvertisementsModel>>> Getadverstisements()
        {
          if (_context.adverstisements == null)
          {
              return NotFound();
          }
            return await _context.adverstisements.ToListAsync();
        }

        // GET: api/AdvertisementsModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdvertisementsModel>> GetAdvertisementsModel(int id)
        {
          if (_context.adverstisements == null)
          {
              return NotFound();
          }
            var advertisementsModel = await _context.adverstisements.FindAsync(id);

            if (advertisementsModel == null)
            {
                return NotFound();
            }

            return advertisementsModel;
        }

        // PUT: api/AdvertisementsModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdvertisementsModel(int id, AdvertisementsModel advertisementsModel)
        {
            if (id != advertisementsModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(advertisementsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvertisementsModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AdvertisementsModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AdvertisementsModel>> PostAdvertisementsModel(AdvertisementsModel advertisementsModel)
        {
          if (_context.adverstisements == null)
          {
              return Problem("Entity set 'OnlineVideoStreamingAppContext.adverstisements'  is null.");
          }
            _context.adverstisements.Add(advertisementsModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdvertisementsModel", new { id = advertisementsModel.Id }, advertisementsModel);
        }

        // DELETE: api/AdvertisementsModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisementsModel(int id)
        {
            if (_context.adverstisements == null)
            {
                return NotFound();
            }
            var advertisementsModel = await _context.adverstisements.FindAsync(id);
            if (advertisementsModel == null)
            {
                return NotFound();
            }

            _context.adverstisements.Remove(advertisementsModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdvertisementsModelExists(int id)
        {
            return (_context.adverstisements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
