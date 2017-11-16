﻿using ElevenNote.Models;
using ElevenNote.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElevenNote2.Web.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private NoteService CreateNoteService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var svc = new NoteService(userId);

            return svc;
        }

        // GET: Notes
        public ActionResult Index()
        {
            var model = CreateNoteService().GetNotes();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new NoteCreateModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NoteCreateModel model)
        {
            //important
            if (!ModelState.IsValid) return View(model);


            if (!CreateNoteService().CreateNote(model))
            {
                ModelState.AddModelError("", "Unable to create note");
                return View(model);
            }

            TempData["SaveResult"] = "Note Created";
                
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var model = CreateNoteService().GetNoteById(id);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var detailModel = CreateNoteService().GetNoteById(id);
            var editModel =
                new NoteEditModel
                {
                    NoteId = detailModel.NoteId,
                    Title = detailModel.Title,
                    Content = detailModel.Content
                };

            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, NoteEditModel model)
        {
            if(model.NoteId != id)
            {
                ModelState.AddModelError("", "Nice try!");
                model.NoteId = id;
                return View(model);
            }

            if (!ModelState.IsValid) return View(model);

            if (!CreateNoteService().UpdateNote(model))
            {
                ModelState.AddModelError("", "Unable to update note");
                return View(model);
            }

            TempData["SaveResult"] = "Edit Saved";

            return RedirectToAction("Index");
        }

        [ActionName("Delete")]
        public ActionResult DeleteGet(int id)
        {
            var model = CreateNoteService().GetNoteById(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeletePost(int id)
        {
            CreateNoteService().DeleteNote(id);

            TempData["SaveResult"] = "Note Deleted";

            return RedirectToAction("Index");
        }
    }
}