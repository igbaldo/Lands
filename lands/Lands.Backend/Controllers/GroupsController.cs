using System;
using System.Collections;
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
    [Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Groups
        public async Task<ActionResult> Index()
        {
            return View(await db.Groups.ToListAsync());
        }

        // GET: Groups/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            foreach (var match in group.Matches)
            {
                match.DateTime = match.DateTime.ToLocalTime();
            }

            return View(group);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "GroupId,Name")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "GroupId,Name")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Group group = await db.Groups.FindAsync(id);
            db.Groups.Remove(group);
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

        public async Task<ActionResult> AddTeam(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Group group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            var groupTeam = new GroupTeam()
            {
                GroupId = group.GroupId
            };

            var teams = new List<Team>();
            foreach (var team in group.GroupTeams)
            {
                teams.Add(team.Team);
            }

            ViewBag.TeamId = new SelectList(db.Teams.OrderBy(x => x.Name), "TeamId", "Name");

            return View(groupTeam);
        }

        [HttpPost]
        public async Task<ActionResult> AddTeam(GroupTeam groupTeam)
        {
            if (ModelState.IsValid)
            {
                db.GroupTeams.Add(groupTeam);
                await db.SaveChangesAsync();
                return RedirectToAction(String.Format("Details/{0}",groupTeam.GroupId));
            }
            
            ViewBag.TeamId = new SelectList(db.Teams, "TeamId", "Name", groupTeam.TeamId);
            return View(groupTeam);
        }

        public async Task<ActionResult> DeleteTeam(int id)
        {
            GroupTeam groupTeam = await db.GroupTeams.FindAsync(id);

            if (groupTeam == null)
            {
                return HttpNotFound();
            }

            db.GroupTeams.Remove(groupTeam);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> AddMatch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Group group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            var match = new Match()
            {
                GroupId = group.GroupId,
                DateTime = DateTime.Today
            };

            var teams = await this.GetTeamsGroup(group);
            ViewBag.LocalId = new SelectList(teams.OrderBy(t => t.Name), "TeamId", "Name");
            ViewBag.VisitorId = new SelectList(teams.OrderBy(t => t.Name), "TeamId", "Name");

            return View(match);
        }

        [HttpPost]
        public async Task<ActionResult> AddMatch(Match match)
        {
            if (ModelState.IsValid)
            {
                match.StatusMatchId = 1;
                match.DateTime = match.DateTime.ToUniversalTime();

                db.Matches.Add(match);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var group = await db.Groups.FindAsync(match.GroupId);
            var teams = await this.GetTeamsGroup(group);

            ViewBag.LocalId = new SelectList(teams.OrderBy(t => t.Name), "TeamId", "Name", match.LocalId);
            ViewBag.VisitorId = new SelectList(teams.OrderBy(t => t.Name), "TeamId", "Name", match.VisitorId);

            return RedirectToAction(String.Format("Details/{0}", group.GroupId));
        }

        private async Task<List<Team>> GetTeamsGroup(Group group)
        {
            var teams = new List<Team>();
            foreach (var item in group.GroupTeams)
            {
                var team = await db.Teams.FindAsync(item.TeamId);
                if (team != null)
                {
                    teams.Add(team);
                }
            }

            return teams;
        }

        public async Task<ActionResult> EditMatch(int? id)
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

            var group = await db.Groups.FindAsync(match.GroupId);
            var teams = await this.GetTeamsGroup(group);

            ViewBag.LocalId = new SelectList(teams.OrderBy(t => t.Name), "TeamId", "Name", match.LocalId);
            ViewBag.VisitorId = new SelectList(teams.OrderBy(t => t.Name), "TeamId", "Name", match.VisitorId);


            return View(match);
        }
    }
}
