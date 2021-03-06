﻿using System.Linq;
using System.Net;
using System.Web.Mvc;
using Common;

namespace Mentor
{
    [Authorize(Roles = "Admin")]
    public class AgencyController : Controller
    {
        private readonly AgencyService _agencies;
        private readonly CodeService _codes;

        public AgencyController(AgencyService agencyService, CodeService codeService)
        {
            _agencies = agencyService;
            _codes = codeService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ListAgencies()
        {
            var agencies = _agencies.Query().ToList();
            ViewData["_Success"] = TempData["_Success"];
            return View("ListAgencies", agencies);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult EditAgency(int? id)
        {
            var agency = _agencies.Find(id) ?? _agencies.Create();
            if (Request.IsPost())
            {
                TryUpdateModel(agency);
                _agencies.Save(agency);
                _agencies.SaveLogo(Request.Files["LogoFile"], agency);
                return RedirectToAction("ListAgencies");
            }
            ViewBag.Codes = _codes.Query().ToList();
            return View(agency);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete(int? id)
        {
            _agencies.Delete(id);
            return RedirectToAction("ListAgencies");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportAgencies()
        {
            var model = (ImportAgencies)DependencyResolver.Current.GetService(typeof(ImportAgencies));
            var file = Request.Files["InputFile"];
            if (file != null && file.InputStream != null && file.ContentLength > 0)
            {
                TempData["_Success"] = "Imported " + model.Import(file.InputStream) + " Agencies";
            }
            return RedirectToAction("ListAgencies");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ExportAgencies()
        {
            var model = (ExportAgencies)DependencyResolver.Current.GetService(typeof(ExportAgencies));
            return model.Download();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Geocode(int id, decimal latitude, decimal longitude)
        {
            var agency = _agencies.Find(id);
            agency.Latitude = latitude;
            agency.Longitude = longitude;
            _agencies.Save(agency);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    };
}
