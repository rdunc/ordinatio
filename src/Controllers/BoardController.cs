using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ordinatio.Models;
using Ordinatio.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Ordinatio.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BoardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
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

        // GET: Board
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(await _context.Boards.Where(m => m.User.Id == user.Id).ToListAsync());
        }

        // GET: Board/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            bool boardOwner = await IsBoardOwner((int)id);
            if (id == null || !boardOwner)
            {
                return NotFound();
            }

            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            var bulletin = await _context.Bulletins.Where(m => m.BoardId == id).ToListAsync();
            Tuple<Board, List<Bulletin>> bulletinBoards = new Tuple<Board, List<Bulletin>>(board, bulletin);

            return View(bulletinBoards);
        }

        // GET: Board/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Board/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Board board)
        {
            if (ModelState.IsValid)
            {
                _context.Add(board);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(board);
        }

        // GET: Board/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            bool boardOwner = await IsBoardOwner((int)id);
            if (id == null || !boardOwner)
            {
                return NotFound();
            }

            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Board/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Board board)
        {
            bool boardOwner = await IsBoardOwner(id);
            if (id != board.Id || !boardOwner)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(board);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardExists(board.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }

            return View(board);
        }

        // GET: Board/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            bool boardOwner = await IsBoardOwner((int)id);
            if (id == null || !boardOwner)
            {
                return NotFound();
            }

            var board = await _context.Boards
                .SingleOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Board/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool boardOwner = await IsBoardOwner(id);
            if (!boardOwner)
            {
                return NotFound();
            }

            var board = await _context.Boards.SingleOrDefaultAsync(m => m.Id == id);
            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BoardExists(int id)
        {
            return _context.Boards.Any(e => e.Id == id);
        }
    }
}
