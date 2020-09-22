using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmniCRM_Web.GenericClasses;
using OmniCRM_Web.Models;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StringConstant.TeleCaller + "," + StringConstant.RelationshipManager + "," + StringConstant.Admin)]

    public class ProductMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public ProductMastersController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/ProductMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductMaster>>> GetProductMaster()
        {
            return await _context.ProductMaster.ToListAsync();
        }

        // GET: api/ProductMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductMaster>> GetProductMaster(int id)
        {
            var productMaster = await _context.ProductMaster.FindAsync(id);

            if (productMaster == null)
            {
                return NotFound();
            }

            return productMaster;
        }

        // PUT: api/ProductMasters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductMaster(int id, ProductMaster productMaster)
        {
            if (id != productMaster.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(productMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductMasterExists(id))
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

        // POST: api/ProductMasters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ProductMaster>> PostProductMaster(ProductMaster productMaster)
        {
            _context.ProductMaster.Add(productMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductMaster", new { id = productMaster.ProductId }, productMaster);
        }

        // DELETE: api/ProductMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductMaster>> DeleteProductMaster(int id)
        {
            var productMaster = await _context.ProductMaster.FindAsync(id);
            if (productMaster == null)
            {
                return NotFound();
            }

            _context.ProductMaster.Remove(productMaster);
            await _context.SaveChangesAsync();

            return productMaster;
        }

        private bool ProductMasterExists(int id)
        {
            return _context.ProductMaster.Any(e => e.ProductId == id);
        }
    }
}
