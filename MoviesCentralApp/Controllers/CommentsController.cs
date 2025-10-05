using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesCentralApp.Models;

namespace MoviesCentralApp.Controllers
{
    public class CommentsController : Controller
    {
        private readonly MoviesCentralDBContext _context;

        public CommentsController(MoviesCentralDBContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index(int? movieId)
        {
            if (movieId == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments.Include(c => c.Movie).Include(c => c.User).Where(c => c.Movie.Movieid == movieId).ToListAsync();

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            ViewBag.UserId = userId;

            ViewBag.MovieId = movieId;

            return View(comments);

           // var moviesCentralDBContext = _context.Comments.Include(c => c.Movie).Include(c => c.User);
           // return View(await moviesCentralDBContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Movie)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Commentid == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {

            Random rnd = new Random();
            int minCommentId = 400;
            int maxCommentId = 2147483647; // Maximum signed 32-bit integer value
            int commentId;

            do
            {
                commentId = rnd.Next(minCommentId, maxCommentId);
            } while (_context.Comments.Any(c => c.Commentid == commentId));

            ViewBag.CommentId = commentId;
            


            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid");
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Commentid,Userid,Movieid,Comment1")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Comments", new {movieid = comment.Movieid});
            }
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", comment.Movieid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", comment.Userid);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", comment.Movieid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", comment.Userid);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Commentid,Userid,Movieid,Comment1")] Comment comment)
        {
            if (id != comment.Commentid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Commentid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", comment.Movieid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", comment.Userid);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Movie)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Commentid == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "MoviesActors");
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Commentid == id);
        }
    }
}
