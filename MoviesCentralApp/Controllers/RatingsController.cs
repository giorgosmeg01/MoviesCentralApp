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
    public class RatingsController : Controller
    {
        private readonly MoviesCentralDBContext _context;

        public RatingsController(MoviesCentralDBContext context)
        {
            _context = context;
        }

        // GET: Ratings
        public async Task<IActionResult> Index()
        {
            

            var moviesCentralDBContext = _context.Ratings.Include(r => r.Movie).Include(r => r.User);
            return View(await moviesCentralDBContext.ToListAsync());
        }

        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Ratingid == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Ratings/Create
        public IActionResult Create()
        {
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid");
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Pass the user ID to the view
            ViewBag.UserId = userId;

            Random rnd = new Random();
            int minRatingId = 500;
            int maxRatingId = 2147483647; // Maximum signed 32-bit integer value
            int ratingId;

            // Generate a unique ratingId
            do
            {
                ratingId = rnd.Next(minRatingId, maxRatingId);
            }
            while (_context.Ratings.Any(r => r.Ratingid == ratingId));

            ViewBag.RatingId = ratingId;



           

            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ratingid,Movieid,Userid,Rating1")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rating);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "MoviesActors");
            }
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", rating.Movieid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", rating.Userid);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", rating.Movieid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", rating.Userid);
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ratingid,Movieid,Userid,Rating1")] Rating rating)
        {
            if (id != rating.Ratingid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingExists(rating.Ratingid))
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
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", rating.Movieid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", rating.Userid);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Ratingid == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Ratingid == id);
        }
    }
}
