using AutoMapper;
using Store.Model;
using Store.Service;
using Store.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Store.Model.Models;

namespace Store.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IGadgetService _gadgetService;

        public HomeController(ICategoryService categoryService, IGadgetService gadgetService)
        {
            _categoryService = categoryService;
            _gadgetService = gadgetService;
        }

        // GET: Home
        public ActionResult Index(string category = null)
        {
            var categories = _categoryService.GetCategories(category).ToList();
            var viewModelGadgets = Mapper.Map<IEnumerable<Category>, IEnumerable<CategoryViewModel>>(categories);

            return View(viewModelGadgets);
        }

        public ActionResult Filter(string category, string gadgetName)
        {
            var gadgets = _gadgetService.GetCategoryGadgets(category, gadgetName);
            var viewModelGadgets = Mapper.Map<IEnumerable<Gadget>, IEnumerable<GadgetViewModel>>(gadgets);

            return View(viewModelGadgets);
        }

        [HttpPost]
        public ActionResult Create(GadgetFormViewModel newGadget)
        {
            if (newGadget?.File != null)
            {
                var gadget = Mapper.Map<GadgetFormViewModel, Gadget>(newGadget);
                _gadgetService.CreateGadget(gadget);

                var gadgetPicture = System.IO.Path.GetFileName(newGadget.File.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/images/"), gadgetPicture);
                newGadget.File.SaveAs(path);
                _gadgetService.SaveGadget();
            }

            var category = _categoryService.GetCategory(newGadget.GadgetCategory);
            return RedirectToAction("Index", new { category = category.Name });
        }
    }
}