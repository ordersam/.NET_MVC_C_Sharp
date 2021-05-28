using MySQLDemo.Models;
using MySQLDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySQLDemo.Controllers
{

    public class bbController : Controller
    {
        // =================== Entity Framework ===================
        // 1.安裝：Connector/NET 8.0.25 (如果有裝MySQL可以一起裝)
        // 2.安裝：MySQL for Visual Studio 1.2.9
        // 3.新增專案 一定要選Entity Framework 4.5.2版
        // 4.NuGet安裝：MySql.Data.EntityFramework 8.0.25 (版本要相同，否則會閃退)
        // 5.建立EF：edmx檔儲存後 專案要重建一次
        // edmx檔編輯：每張表EntitySet 都要設為Schema="" + 去掉 DefiningQuery(如果有的話)
        // Entity Framework list
        public ActionResult Index()
        {
            var db = new elevatorEntities();
            var table = from p in db.baby
                        select p;
            List<Cbaby> list = new List<Cbaby>();
            if (table != null)
            {
                foreach (baby p in table)
                {
                    list.Add(new Cbaby(p));
                }
            }
            return View(list);
        }

        // =================== ADO ===================
        // list
        public ActionResult ado()
        {
            List<Cproduct> list = new CproductFactory().QueryAll();
            return View(list);
        }
        // update
        public ActionResult adoUpdate(int? fId)
        {
            if (fId != null)
            {
                Cproduct product = new CproductFactory().QueryByfId((int)fId);
                if (product != null)
                {
                    return View(product);
                }
                else
                {
                    return RedirectToAction("ado");
                }
            }
            else
            {
                return RedirectToAction("ado");
            }
        }
        [HttpPost]
        public ActionResult adoUpdate(int fId, string fName)
        {
            bool result = new CproductFactory().Update(fId, fName);
            return RedirectToAction("ado");
        }
        // delete
        public ActionResult adoDelete(int fId)
        {
            bool result = new CproductFactory().Delete(fId);
            return RedirectToAction("ado");
        }
        // create
        public ActionResult adoAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult adoAdd(string fName)
        {
            bool result = new CproductFactory().Add(fName);
            return RedirectToAction("ado");
        }
        // query
        public ActionResult adoQuery(int fId)
        {
            Cproduct p = new CproductFactory().QueryByfId2(fId);
            return View(p);
        }
    }
}