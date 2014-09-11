﻿using System;
using System.Linq;
using System.Web.Mvc;

public class ArticleController : Controller
{
    public ActionResult Index()
    {
        using (var db = new MentorDb())
        {
            // var model = db.Articles.ToList<Article>();
            return View();
        }
    }

    [HttpGet]
    public ActionResult Edit(int? id)
    {
        using (var db = new MentorDb())
        {
            var model = id.HasValue
                            ? db.Articles.Single(x => x.Id == id.Value)
                            : new Article {Modified = DateTime.Now};
            return View(model);
        }
    }

    [HttpPost]
    public ActionResult Edit(int? id, FormCollection form)
    {
        using (var db = new MentorDb())
        {
            var model = db.Articles.SingleOrDefault(x => x.Id == id)
                        ?? db.Add(new Article());
            if (form["_action"] == "Delete")
            {
                if (id.HasValue) db.Delete(model);
            }
            else
            {
                TryUpdateModel(model);
            }
            db.Commit();
        }
        return RedirectToAction("Index");
    }
};
