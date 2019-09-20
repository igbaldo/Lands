using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lands.Backend.Models;
using Lands.Domain;

namespace Lands.Backend.Controllers
{
    public class MatchesController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Matches
        public async Task<ActionResult> Index()
        {
            var matches = db.Matches.Include(m => m.Group).Include(m => m.Local).Include(m => m.StatusMatch).Include(m => m.Visitor);
            return View(await matches.ToListAsync());
        }

        // GET: Matches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // GET: Matches/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Name");
            ViewBag.LocalId = new SelectList(db.Teams, "TeamId", "Name");
            ViewBag.StatusMatchId = new SelectList(db.StatusMatches, "StatusMatchId", "Name");
            ViewBag.VisitorId = new SelectList(db.Teams, "TeamId", "Name");
            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MatchId,GroupId,LocalId,VisitorId,StatusMatchId,LocalGoals,VisitorGoals,DateTime")] Match match)
        {
            if (ModelState.IsValid)
            {
                db.Matches.Add(match);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Name", match.GroupId);
            ViewBag.LocalId = new SelectList(db.Teams, "TeamId", "Name", match.LocalId);
            ViewBag.StatusMatchId = new SelectList(db.StatusMatches, "StatusMatchId", "Name", match.StatusMatchId);
            ViewBag.VisitorId = new SelectList(db.Teams, "TeamId", "Name", match.VisitorId);
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Name", match.GroupId);
            ViewBag.LocalId = new SelectList(db.Teams, "TeamId", "Name", match.LocalId);
            ViewBag.StatusMatchId = new SelectList(db.StatusMatches, "StatusMatchId", "Name", match.StatusMatchId);
            ViewBag.VisitorId = new SelectList(db.Teams, "TeamId", "Name", match.VisitorId);
            return View(match);
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MatchId,GroupId,LocalId,VisitorId,StatusMatchId,LocalGoals,VisitorGoals,DateTime")] Match match)
        {
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Name", match.GroupId);
            ViewBag.LocalId = new SelectList(db.Teams, "TeamId", "Name", match.LocalId);
            ViewBag.StatusMatchId = new SelectList(db.StatusMatches, "StatusMatchId", "Name", match.StatusMatchId);
            ViewBag.VisitorId = new SelectList(db.Teams, "TeamId", "Name", match.VisitorId);
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Match match = await db.Matches.FindAsync(id);
            db.Matches.Remove(match);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
