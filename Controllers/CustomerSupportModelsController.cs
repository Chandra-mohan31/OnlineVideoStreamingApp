using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineVideoStreamingApp.Areas.Identity.Data;
using OnlineVideoStreamingApp.Data;
using OnlineVideoStreamingApp.Models;

namespace OnlineVideoStreamingApp.Controllers
{
    public class CustomerSupportModelsController : Controller
    {
        private readonly OnlineVideoStreamingAppContext _context;
        private readonly UserManager<OnlineVideoStreamingAppUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        public CustomerSupportModelsController(OnlineVideoStreamingAppContext context,UserManager<OnlineVideoStreamingAppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: CustomerSupportModels
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(this.User);
            var user = _userManager.Users.FirstOrDefault(U => U.Id == userId);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if (role.Equals("Admin"))
                {
                    ViewData["Role"] = role;
                    return _context.customerSupportTable != null ?
                          View(await _context.customerSupportTable.Include(u => u.QueryPostedUser).Where(cu => cu.Reply == null || cu.Reply.Length == 0).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ContactDetails'  is null.");
                }
            }
            return _context.customerSupportTable != null ? 
                          View(await _context.customerSupportTable.Where(u => u.QueryPostedUser.Id == userId).ToListAsync()) :
                          Problem("Entity set 'OnlineVideoStreamingAppContext.customerSupportTable'  is null.");
        }

        // GET: CustomerSupportModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.customerSupportTable == null)
            {
                return NotFound();
            }

            var customerSupportModel = await _context.customerSupportTable
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerSupportModel == null)
            {
                return NotFound();
            }

            return View(customerSupportModel);
        }

        // GET: CustomerSupportModels/Create
      

        // POST: CustomerSupportModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostQuery(string query)
        {
            CustomerSupportModel customerSupportModel = new CustomerSupportModel();
            customerSupportModel.Query = query;
            var userId = _userManager.GetUserId(this.User);
            var user = _userManager.Users.FirstOrDefault(U => U.Id == userId);
            customerSupportModel.QueryPostedUser = user;
            _context.Add(customerSupportModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
            //return View(customerSupportModel);
        }

        // GET: CustomerSupportModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.customerSupportTable == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(this.User);
            var user = _userManager.Users.FirstOrDefault(U => U.Id == userId);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if (role.Equals("Admin"))
                {
                    ViewData["Role"] = role;
                }
            }
            
            var customerSupportModel = await _context.customerSupportTable.FindAsync(id);
            
            if (customerSupportModel == null)
            {
                return NotFound();
            }
            return View(customerSupportModel);
        }

        // POST: CustomerSupportModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Query,Status,Reply")] CustomerSupportModel customerSupportModel)
        {
            if (id != customerSupportModel.Id)
            {
                return NotFound();
            }

           
                try
                {
              

                    _context.Update(customerSupportModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerSupportModelExists(customerSupportModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            //return View(customerSupportModel);
        }

        // GET: CustomerSupportModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.customerSupportTable == null)
            {
                return NotFound();
            }

            var customerSupportModel = await _context.customerSupportTable
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerSupportModel == null)
            {
                return NotFound();
            }

            return View(customerSupportModel);
        }

        // POST: CustomerSupportModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.customerSupportTable == null)
            {
                return Problem("Entity set 'OnlineVideoStreamingAppContext.customerSupportTable'  is null.");
            }
            var customerSupportModel = await _context.customerSupportTable.FindAsync(id);
            if (customerSupportModel != null)
            {
                _context.customerSupportTable.Remove(customerSupportModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerSupportModelExists(int id)
        {
          return (_context.customerSupportTable?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
