﻿using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ssc.consulting.switchboard.Infactractures;
using ssc.consulting.switchboard.Models;
using ssc.consulting.switchboard.Services;
using ssc.consulting.switchboard.ViewModels;

namespace ssc.consulting.switchboard.Controllers
{
    [RoutePrefix("Admin")]
    public class BannerController : Controller
    {
        private readonly IBannerService _iService = new BannerService();
        readonly int _rowsdisplay = int.Parse(WebConfigurationManager.AppSettings["rowsdisplay"].ToString());
        //
        // GET: /Banner/
        [Route("Banner"), Route("Banner/Index")]
        public ActionResult Index(int page = 1)
        {
            var data = new BannerViewModel
            {
                ListShow = _iService.GetList(page, _rowsdisplay),
                TotalRecord = _iService.TotalRecord()
            };
            return View(data);
        }


        [Route("Banner/Credit"), Route("Banner/Credit/{id}")]
        public ActionResult Credit(Guid? id)
        {
            if(id.IsNull())
                return View(new Banner { Position = _iService.GetPosition() });
            var result = _iService.GetEntityById(id.GetValueOrDefault());
            return View(result);
        }

        [Route("Banner/Credit"), Route("Banner/Credit/{id}")]
        [HttpPost]
        public ActionResult Credit(Banner model, HttpPostedFileBase ImageUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultImage = UploadFileResult.UploadImage(ImageUrl);

            if (resultImage.IsSuccess == false && resultImage.Result != "")
            {
                SessionUserHelper.CreateSessionError(resultImage.Result);
                return View(model);
            }
            model.ImageUrl = resultImage.FileName;

            if (model.Id.IsGuidEmpty())
            {
                if (_iService.IsExitsPosition(model.Position))
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.PositionExist + _iService.GetPosition());
                    return View(model);
                }
                if (_iService.Add(model))
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
                var old = _iService.GetEntityById(model.Id);
                if (old.Position != model.Position && _iService.IsExitsPosition(model.Position))
                {
                    SessionUserHelper.CreateSessionError(ConstantStrings.PositionExist + _iService.GetPosition());
                    return View(model);
                }

                if (resultImage.FileName == "")
                {
                    model.ImageUrl = old.ImageUrl;
                }

                if (_iService.Update(model))
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

        [Route("Banner/Delete/{id}")]
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

        [Route("Banner/Active/{id}")]
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

        [Route("Banner/Disable/{id}")]
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