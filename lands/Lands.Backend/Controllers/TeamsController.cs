using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lands.Backend.Helpers;
using Lands.Backend.Models;
using Lands.Domain;

namespace Lands.Backend.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeamsController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Teams
        public async Task<ActionResult> Index()
        {
            return View(await db.Teams.ToListAsync());
        }

        // GET: Teams/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TeamView teamView)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Teams";

                if (teamView.ImageFile != null)
                {
                    pic = FilesHelper.UploadPhoto(teamView.ImageFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                var team = this.ToTeam(teamView);
                team.ImagePath = pic;

                db.Teams.Add(team);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(teamView);
        }

        private Team ToTeam(TeamView teamView)
        {
            return new Team()
            {
                ImagePath = teamView.ImagePath,
                Name = teamView.Name,
                TeamId = teamView.TeamId
            };
        }

        // GET: Teams/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            var teamView = this.ToTeamView(team);

            return View(teamView);
        }

        private TeamView ToTeamView(Team team)
        {
            return new TeamView()
            {
                ImagePath = team.ImagePath,
                Name = team.Name,
                TeamId = team.TeamId
            };
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TeamView teamView)
        {
            if (ModelState.IsValid)
            {
                var pic = teamView.ImagePath;
                var folder = "~/Content/Teams";

                if (teamView.ImageFile != null)
                {
                    pic = FilesHelper.UploadPhoto(teamView.ImageFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                var team = this.ToTeam(teamView);
                team.ImagePath = pic;

                db.Entry(team).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(teamView);
        }

        // GET: Teams/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Team team = await db.Teams.FindAsync(id);
            db.Teams.Remove(team);
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
