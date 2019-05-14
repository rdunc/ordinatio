using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ordinatio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Ordinatio.Models.Identity;
using Microsoft.AspNetCore.Http;

namespace Ordinatio.Controllers
{
    [Authorize]
    public class BulletinController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BulletinController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Boolean> IsBoardOwner(int boardId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == boardId);

            if (user.BoardId == boardId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // GET: Bulletin
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Bulletins.Include(b => b.Board);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: Bulletin/Details/5
        //public async task<iactionresult> details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return notfound();
        //    }

        //    var bulletin = await _context.bulletins
        //        .include(b => b.board)
        //        .singleordefaultasync(m => m.id == id);
        //    if (bulletin == null)
        //    {
        //        return notfound();
        //    }

        //    return view(bulletin);
        //}

        // GET: Bulletin/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["BoardId"] = new SelectList(_context.Boards.Where(m => m.User.Id == user.Id), "Id", "Id");
            return View();
        }

        // POST: Bulletin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BoardId,Name,Description")] Bulletin bulletin)
        {
            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == bulletin.BoardId);
            bool boardOwner = await IsBoardOwner(board.Id);
            if (bulletin == null || board == null || !boardOwner)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Add(bulletin);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Board", new { @id = bulletin.BoardId });
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["BoardId"] = new SelectList(_context.Boards.Where(m => m.User.Id == user.Id), "Id", "Id", bulletin.BoardId);
            return View(bulletin);
        }

        // GET: Bulletin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bulletin = await _context.Bulletins.SingleOrDefaultAsync(m => m.Id == id);
            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == bulletin.BoardId);
            bool boardOwner = await IsBoardOwner(board.Id);
            if (bulletin == null || board == null || !boardOwner)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["BoardId"] = new SelectList(_context.Boards.Where(m => m.User.Id == user.Id), "Id", "Id", bulletin.BoardId);
            return View(bulletin);
        }

        // POST: Bulletin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BoardId,Name,Description")] Bulletin bulletin)
        {
            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == bulletin.BoardId);
            bool boardOwner = await IsBoardOwner(board.Id);
            if (bulletin == null || board == null || !boardOwner)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bulletin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BulletinExists(bulletin.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Details", "Board", new { @id = bulletin.BoardId });
            }

            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Id", bulletin.BoardId);
            return View(bulletin);
        }

        // GET: Bulletin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bulletin = await _context.Bulletins
                .Include(b => b.Board)
                .SingleOrDefaultAsync(m => m.Id == id);

            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == bulletin.BoardId);
            bool boardOwner = await IsBoardOwner(board.Id);
            if (bulletin == null || board == null || !boardOwner)
            {
                return NotFound();
            }

            return View(bulletin);
        }

        // POST: Bulletin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bulletin = await _context.Bulletins.SingleOrDefaultAsync(m => m.Id == id);
            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == bulletin.BoardId);
            bool boardOwner = await IsBoardOwner(board.Id);
            if (bulletin == null || board == null || !boardOwner)
            {
                return NotFound();
            }

            _context.Bulletins.Remove(bulletin);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Board", new { @id = bulletin.BoardId });
        }

        private bool BulletinExists(int id)
        {
            return _context.Bulletins.Any(e => e.Id == id);
        }
    }
}
