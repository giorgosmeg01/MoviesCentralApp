using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesCentralApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text.Json;


namespace MoviesCentralApp.Controllers
{


    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

    public class MoviesActorsController : Controller
    {
        private readonly MoviesCentralDBContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;



        public MoviesActorsController(MoviesCentralDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            
            _webHostEnvironment = webHostEnvironment;
        }


        



       




        // GET: MoviesActors
        public async Task<IActionResult> Index(string movieGenre, string searchString, string sortOrder)
        {
            var moviesCentralDBContext = _context.MoviesActors.Include(m => m.Actor).Include(m => m.Movie).Include(m => m.Movie.Ratings);

            if(moviesCentralDBContext == null)
            {
                return NotFound();
            }

            IQueryable<string> genreQuery = from m in moviesCentralDBContext.Include(m => m.Movie)
                                            where m.Movie != null
                                            orderby m.Movie.Genre
                                            select m.Movie.Genre;

            var movies = from m in moviesCentralDBContext.Include(m => m.Movie)
                         where m.Movie != null
                         select m;



            if (!string.IsNullOrEmpty(searchString))
            {
                    movies = from m in moviesCentralDBContext.Include(m => m.Movie)
                             where m.Movie != null && m.Movie.Title.Contains(searchString)
                             select m;

                    if (sortOrder == "Highest Rating")
                    {
                    movies = movies.OrderByDescending(m => m.Movie.Ratings.Any() ? m.Movie.Ratings.Average(r => (double)r.Rating1) : 0);
                    }


            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = from m in moviesCentralDBContext.Include(m => m.Movie)
                         where m.Movie != null && m.Movie.Genre == movieGenre
                         select m;

                if (sortOrder == "Highest Rating")
                {
                    movies = movies.OrderByDescending(m => m.Movie.Ratings.Any() ? m.Movie.Ratings.Average(r => (double)r.Rating1) : 0);
                }

            }

            if(!string.IsNullOrEmpty(movieGenre) && !string.IsNullOrEmpty(searchString))
            {
                movies = from m in moviesCentralDBContext.Include(m => m.Movie)
                         where m.Movie != null && m.Movie.Title.Contains(searchString) && m.Movie.Genre == movieGenre
                         select m;

                if (sortOrder == "Highest Rating")
                {
                    movies = movies.OrderByDescending(m => m.Movie.Ratings.Any() ? m.Movie.Ratings.Average(r => (double)r.Rating1) : 0);
                }
            }

            if (sortOrder == "Highest Rating" && string.IsNullOrEmpty(movieGenre) && string.IsNullOrEmpty(searchString))
            {
                movies = movies.OrderByDescending(m => m.Movie.Ratings.Any() ? m.Movie.Ratings.Average(r => (double)r.Rating1) : 0);
            }


            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            ViewBag.SelectedGenre = movieGenre;
            ViewBag.SearchString = searchString;

            ViewBag.UserId = userId;

            


            return View(await movies.ToListAsync());
        }






        

        // GET: MoviesActors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var moviesActor = await _context.MoviesActors
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Maid == id);
            if (moviesActor == null)
            {
                return NotFound();
            }

            // Get all actors for the same movie
            var otherActors = await _context.MoviesActors
                .Where(ma => ma.Movieid == moviesActor.Movieid && ma.Maid != moviesActor.Maid)
                .Select(ma => ma.Actor)
                .ToListAsync();

            // Pass the list of other actors to the view
            ViewBag.OtherActors = otherActors;



            string movieTitle = moviesActor.Movie.Title;
            string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "MovieTrailers");
            string[] trailerFiles = Directory.GetFiles(directoryPath, "*.txt");

            string trailerUrl = null;
            foreach (var file in trailerFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.Equals(movieTitle, StringComparison.OrdinalIgnoreCase))
                {
                    // Found the matching file, read the trailer URL
                    trailerUrl = System.IO.File.ReadAllText(file);
                    break;
                }
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;


            ViewBag.UserId = userId;

            if (trailerUrl == null && movieTitle == "The Lord of the Rings: The Fellowship of the Ring")
            {
                string trailerPath = Path.Combine(_webHostEnvironment.WebRootPath, "MovieTrailers", "The Lord of the Rings The Fellowship of the Ring.txt");

                string fileContent = System.IO.File.ReadAllText(trailerPath);

                ViewBag.TrailerUrl = fileContent;
            }
            else
            {
                // Pass the movie and trailer URL to the view
                ViewBag.TrailerUrl = trailerUrl;
            }

          

            // Get recommended movies with the same genre
            var recommendedMovies = _context.Movies.Where(m => m.Genre == moviesActor.Movie.Genre && m.Movieid != moviesActor.Movie.Movieid).ToList();
            ViewBag.RecommendedMovies = recommendedMovies;

            return View(moviesActor);
        }

        // GET: MoviesActors/Create
        public IActionResult Create()
        {

            var random = new Random();
            int maId;

            do
            {
                // Generate a random MaId between 301 and Int32.MaxValue
                maId = random.Next(301, int.MaxValue);
            } while (_context.MoviesActors.Any(a => a.Maid == maId)); // Check if the generated MaId already exists

            // Pass the generated MaId to the view
            ViewBag.MaId = maId;

            ViewData["Actorid"] = new SelectList(_context.Actors, "Actorid", "Actorid");
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid");
            return View();
        }

        // POST: MoviesActors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Maid,Movieid,Actorid")] MoviesActor moviesActor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(moviesActor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Actorid"] = new SelectList(_context.Actors, "Actorid", "Actorid", moviesActor.Actorid);
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", moviesActor.Movieid);
            return View(moviesActor);
        }

        // GET: MoviesActors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moviesActor = await _context.MoviesActors.FindAsync(id);
            if (moviesActor == null)
            {
                return NotFound();
            }
            ViewData["Actorid"] = new SelectList(_context.Actors, "Actorid", "Actorid", moviesActor.Actorid);
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", moviesActor.Movieid);
            return View(moviesActor);
        }

        // POST: MoviesActors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Maid,Movieid,Actorid")] MoviesActor moviesActor)
        {
            if (id != moviesActor.Maid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(moviesActor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MoviesActorExists(moviesActor.Maid))
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
            ViewData["Actorid"] = new SelectList(_context.Actors, "Actorid", "Actorid", moviesActor.Actorid);
            ViewData["Movieid"] = new SelectList(_context.Movies, "Movieid", "Movieid", moviesActor.Movieid);
            return View(moviesActor);
        }

        // GET: MoviesActors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moviesActor = await _context.MoviesActors
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Maid == id);
            if (moviesActor == null)
            {
                return NotFound();
            }

            return View(moviesActor);
        }

        // POST: MoviesActors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var moviesActor = await _context.MoviesActors.FindAsync(id);
            if (moviesActor != null)
            {
                _context.MoviesActors.Remove(moviesActor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MoviesActorExists(int id)
        {
            return _context.MoviesActors.Any(e => e.Maid == id);
        }


        public IActionResult DirectorDetails(string? directorName)
        {
            // Retrieve the director's bio from the text file based on the director's name
            string webRootPath = _webHostEnvironment.WebRootPath;
            string bioFilePath = Path.Combine(webRootPath, "DirectorsBio", $"{directorName}.txt");
            string bio = System.IO.File.ReadAllText(bioFilePath);


            ViewBag.DirectorsName = directorName;
            ViewBag.PicturePath = $"~/DirectorImages/{directorName}.jpg";
            ViewBag.Bio = bio;

            var movies = _context.Movies.Where(m => m.Director == directorName).ToList();

            ViewBag.Movies = movies;



            return View();
        }

        public IActionResult ActorDetails(string? actorName)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string bioFilePath = Path.Combine(webRootPath, "ActorsBio", $"{actorName}.txt");
            string bio = System.IO.File.ReadAllText(bioFilePath);

            ViewBag.ActorName = actorName;


            // Get the actor's photo URL based on the actor's name
            var actor = _context.Actors.FirstOrDefault(a => a.Fullname == actorName);

            // Get actor's ID
            int actorId = actor.Actorid;

            // Get movies featuring the actor
            var moviesFeaturingActor = _context.MoviesActors
                .Where(ma => ma.Actorid == actorId)
                .Select(ma => new { ma.Movie.Title, ma.Movie.Posturl })
                .ToList();


            ViewBag.Movies = moviesFeaturingActor;

            string photoUrl = actor != null ? actor.Photourl : string.Empty;

            ViewBag.PhotoUrl = photoUrl;

            ViewBag.ActorBio = bio;

            return View();
        }





        public IActionResult AddToWatchlist(int movieId)
        {
            // Retrieve user ID from session
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Construct session variable name
            string watchlistSessionKey = $"List_{userId}";

            // Retrieve watchlist from session, or create a new one if it doesn't exist
            List<int> watchlist = HttpContext.Session.GetObject<List<int>>(watchlistSessionKey) ?? new List<int>();

            // Check if the movie is already in the watchlist
            if (!watchlist.Contains(movieId))
            {
                // Add movie ID to watchlist
                watchlist.Add(movieId);

                // Update watchlist in session
                HttpContext.Session.SetObject(watchlistSessionKey, watchlist);

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        public IActionResult Watchlist()
        {
            // Retrieve user ID from session
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Construct session variable name
            string watchlistSessionKey = $"List_{userId}";

            // Retrieve watchlist from session, or create a new one if it doesn't exist
            List<int> watchlist = HttpContext.Session.GetObject<List<int>>(watchlistSessionKey) ?? new List<int>();

            // Query the database to get movie details based on the IDs in the watchlist
            var moviesInWatchlist = _context.Movies.Where(m => watchlist.Contains(m.Movieid)).ToList();

            // Pass the list of movies to the view
            return View(moviesInWatchlist);
        }






    }
}
