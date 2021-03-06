﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using ssc.consulting.switchboard.Infactractures;
using ssc.consulting.switchboard.Models;
using ssc.consulting.switchboard.Services;
using ssc.consulting.switchboard.ViewModels;

namespace ssc.consulting.switchboard.Controllers
{
    [RoutePrefix("Admin")]
    public class NewsController : Controller
    {
        //
        // GET: /News/
        private readonly INewsService _iService = new NewsService();
        private readonly IMainCategoryService _iMainCategoryService = new MainCategoryService();
        private readonly IChildCategoryService _iChildCategoryService = new ChildCategoryService();
        private readonly SeoNameService _seonameService = new SeoNameService();
        readonly int _rowsdisplay = int.Parse(WebConfigurationManager.AppSettings["rowsdisplay"].ToString());

        [Route("News"), Route("News/Index")]
        public ActionResult Index(int page = 1)
        {
            var result = _iService.GetList(page, _rowsdisplay);

            var listdata = (from item in result
                let main = _iMainCategoryService.GetEntityById(item.MainCategoryId).Name
                let child = _iChildCategoryService.GetEntityById(item.ChildCategoryId).Name
                select new NewsModel
                {
                    Id = item.Id, Title = item.Title, SeoName = item.SeoName, Position = item.Position, IsActive = item.IsActive, ChildCategoryName = child, MainCategoryName = main
                }).ToList();


            var model = new NewsViewModel
            {
                ListShow = listdata,
                TotalRecord = _iService.TotalRecord()
            };
            return View(model);
        }


        [Route("News/Credit"), Route("News/Credit/{id}")]
        public ActionResult Credit(Guid? id)
        {
            if (id.IsNull())
                return View(new NewsModel { Position = _iService.GetPosition(), ListMainCategory = _iMainCategoryService.GetListActive(), ListChildCategory = _iChildCategoryService.GetListActive()});
            var result = _iService.GetEntityById(id.GetValueOrDefault());
            var model = new NewsModel
            {
                Id = result.Id,
                Description = result.Description,
                Position = result.Position,
                ChildCategoryId = result.ChildCategoryId,
                Contents = result.Contents,
                Image = result.Image,
                IsActive = result.IsActive,
                MainCategoryId = result.MainCategoryId,
                SeoName = result.SeoName,
                Title = result.Title,
                ListChildCategory = _iChildCategoryService.GetListActive(),
                ListMainCategory = _iMainCategoryService.GetListActive()
            };
            return View(model);
        }

        [Route("News/Credit"), Route("News/Credit/{id}")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Credit(NewsModel model, HttpPostedFileBase Image)
        {
            var imgold = string.Empty;
            if (!ModelState.IsValid)
                return View(model);

            var data = new News
            {
                Id = model.Id,
                Description = model.Description,
                Position = model.Position,
                ChildCategoryId = model.ChildCategoryId,
                Contents = model.Contents,
                Image = model.Image,
                IsActive = model.IsActive,
                MainCategoryId = model.MainCategoryId,
                SeoName = model.SeoName,
                Title = model.Title
            };

            if (data.Id.IsGuidEmpty())
            {
                if (_iService.IsExitsPosition(model.Position))
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.PositionExist + _iService.GetPosition());
                    return View(model);
                }

                if (_seonameService.CheckSeoName(model.SeoName))
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.SeoNameExist);
                    return View(model);
                }
            }
            else
            {
                var old = _iService.GetEntityById(model.Id);
                if (old.Position != model.Position && _iService.IsExitsPosition(model.Position))
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.PositionExist + _iService.GetPosition());
                    return View(model);
                }

                if (old.SeoName != model.SeoName && _seonameService.CheckSeoName(model.SeoName))
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.SeoNameExist);
                    return View(model);
                }
                imgold = old.Image;
            }
            
            var resultImage = UploadFileResult.UploadImage(Image);

            if (resultImage.IsSuccess == false && resultImage.Result != "")
            {
                SessionUserHelper.CreateSessionError(resultImage.Result);
                return View(model);
            }
            data.Image = resultImage.FileName;

            if (data.Id.IsGuidEmpty())
            {
                if (_iService.Add(data))
                {
                    SessionUserHelper.CreateSessionSuccess(ConstantStrings.AddSuccess);
                }
                else
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.AddNonSuccess);
                }
            }
            else
            {

                if (resultImage.FileName == "")
                {
                    data.Image = imgold;
                }

                if (_iService.Update(data))
                {
                    SessionUserHelper.CreateSessionSuccess(ConstantStrings.EditSuccess);
                }
                else
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.EditNonSuccess);
                }
            }
            
            return RedirectToAction("Index");
        }

        [Route("News/Delete/{id}")]
        public ActionResult Delete(Guid id)
        {
            if (_iService.Delete(id))
            {
                SessionUserHelper.CreateSessionSuccess(ConstantStrings.DeleteSuccess);
            }
            else
            {
                SessionUserHelper.CreateSessionError(ConstantStrings.DeleteNonSuccess);
            }
            return RedirectToAction("Index");
        }

        [Route("News/Active/{id}")]
        public ActionResult Active(Guid id)
        {
            if (_iService.Active(id))
            {
                SessionUserHelper.CreateSessionSuccess(ConstantStrings.UpdateStatusSuccess);
            }
            else
            {
                SessionUserHelper.CreateSessionError(ConstantStrings.UpdateStatusNonSuccess);
            }
            return RedirectToAction("Index");
        }

        [Route("News/Disable/{id}")]
        public ActionResult Disable(Guid id)
        {
            if (_iService.Disable(id))
            {
                SessionUserHelper.CreateSessionSuccess(ConstantStrings.UpdateStatusSuccess);
            }
            else
            {
                SessionUserHelper.CreateSessionError(ConstantStrings.UpdateStatusNonSuccess);
            }
            return RedirectToAction("Index");
        }
	}
}