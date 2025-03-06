using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplicationMenu.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplicationMenu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //call TEST procedure
            GetLastMainMenuItem();

            return View(SetMenu(_context.SiteApplications.OrderBy(d => d.TopicOrder).ToList()));
        }

        [HttpPost]
        public ActionResult GetPartialView(string name, bool hasParent = false, int id = 0, string newMenu = "")
        {
            var model = new List<MenuViewModel>();
            switch (name)
            {
                case "UP":
                    MoveUp(id);
                    break;
                case "DOWN":
                    MoveDown(id);
                    break;
                case "RIGHT":
                    MoveRight(id);
                    break;
                case "LEFT":
                    MoveLeft(id);
                    break;
                case "DELETE":
                    Delete(id);
                    break;
                case "NEW":
                    AddMenu(id, newMenu);
                    break;
            }


            var items = _context.SiteApplications.ToList().OrderBy(d => d.TopicOrder).Select(d => new MenuViewModel()
            {
                Id = d.ApplicationID,
                Name = d.Title,
                Intent = d.Name,
                Order = d.TopicIndent,
                ParentId = d.ParentId
            }).ToList();

            return PartialView("_PartialMenu", SetMenu(_context.SiteApplications.OrderBy(d => d.TopicOrder).ToList(), id));
        }

        [HttpPost]
        public ActionResult GetSelectPartialView(int id = 0)
        {
            var model = new List<MenuViewModel>();
            var items = _context.SiteApplications.ToList().OrderBy(d => d.TopicOrder).Select(d => new MenuViewModel()
            {
                Id = d.ApplicationID,
                Name = d.Title,
                Intent = d.Name,
                Order = d.TopicIndent,
                ParentId = d.ParentId
            }).ToList();
            foreach (var item in items)
                if (item.Id == id)
                {
                    item.Selected = true;
                }


            return PartialView("_PartialMenu", SetMenu(_context.SiteApplications.OrderBy(d => d.TopicOrder).ToList(), id));
        }

        private List<MenuViewModel> SetMenu(List<SiteApplication> items, int id = 0)
        {
            var result = new List<MenuViewModel>();
            var mainItems = items.Where(d => d.ParentId == 0).ToList();
            foreach (var item in mainItems)
            {
                result.Add(new MenuViewModel()
                {
                    Id = item.ApplicationID,
                    Name = item.Title,
                    Intent = item.Name,
                    Order = item.TopicIndent,
                    ParentId = item.ParentId,
                    Selected = item.ApplicationID == id ? true : false,
                    Items = GetChildren(items, item.ApplicationID, id).ToList()
                });
            }

            return result;
        }


        public List<MenuViewModel> GetChildren(List<SiteApplication> items, int parentId, int id = 0)
        {
            return items
                    .Where(c => c.ParentId == parentId)
                    .Select(c => new MenuViewModel
                    {
                        Id = c.ApplicationID,
                        Name = c.Title,
                        Intent = c.Name,
                        Order = c.TopicIndent,
                        ParentId = c.ParentId,
                        Selected = c.ApplicationID == id ? true : false,
                        Items = GetChildren(items, c.ApplicationID, id)
                    })
                    .ToList();
        }

        private void IntentUp(int id)
        {
            var toUpdate = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
            if (toUpdate.ParentId > 0)
            {
                var parent = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == toUpdate.ParentId);
                if (parent != null && parent.ParentId > 0)
                {
                    toUpdate.ParentId = parent.ParentId;
                }
                else
                {
                    toUpdate.ParentId = 0;
                }
                _context.SiteApplications.Update(toUpdate);
                _context.SaveChanges();
            }
        }

        private void IntentDown(int id)
        {
            var toUpdate = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
            if (toUpdate.ParentId > 0)
            {
                var parent = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == toUpdate.ParentId);
                if (parent != null && parent.ParentId > 0)
                {
                    toUpdate.ParentId = parent.ParentId;
                }
                else
                {
                    toUpdate.ParentId = 0;
                }
                _context.SiteApplications.Update(toUpdate);
                _context.SaveChanges();
            }
        }

        private void MoveUp(int id)
        {
            try
            {
                var toUpdate = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
                if (toUpdate != null)
                {
                    if (toUpdate.ParentId > 0)
                    {
                        var sieblings = _context.SiteApplications.Where(d => d.ParentId == toUpdate.ParentId);
                        if (sieblings.Count() > 1)
                        {
                            var prevSiebling = sieblings.Where(d => d.TopicOrder <= toUpdate.TopicOrder && d.ApplicationID != id).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                            if (prevSiebling != null)
                            {
                                var orderPrevSiebling = prevSiebling.TopicOrder;
                                toUpdate.TopicOrder = orderPrevSiebling;
                                prevSiebling.TopicOrder = orderPrevSiebling + 1;
                                _context.SiteApplications.Update(prevSiebling);
                                _context.SiteApplications.Update(toUpdate);
                                _context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        if (toUpdate.TopicOrder > 1)
                        {
                            var prev = _context.SiteApplications.Where(d => d.TopicOrder < toUpdate.TopicOrder && d.ParentId == 0).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                            if (prev != null)
                            {
                                int prevNo = prev.TopicOrder;
                                prev.TopicOrder = toUpdate.TopicOrder;
                                toUpdate.TopicOrder = prevNo;
                                _context.SiteApplications.Update(prev);
                                _context.SiteApplications.Update(toUpdate);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            //already on top
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void MoveDown(int id)
        {
            try
            {
                var toUpdate = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
                if (toUpdate != null)
                {
                    if (toUpdate.ParentId > 0)
                    {
                        var sieblings = _context.SiteApplications.Where(d => d.ParentId == toUpdate.ParentId);
                        if (sieblings.Count() > 1)
                        {
                            var nextSiebling = sieblings.Where(d => d.TopicOrder >= toUpdate.TopicOrder && d.ApplicationID != id).OrderBy(d => d.TopicOrder).FirstOrDefault();
                            if (nextSiebling != null)
                            {
                                var orderNextSiebling = nextSiebling.TopicOrder;
                                toUpdate.TopicOrder = orderNextSiebling;
                                nextSiebling.TopicOrder = orderNextSiebling - 1;
                                _context.SiteApplications.Update(nextSiebling);
                                _context.SiteApplications.Update(toUpdate);
                                _context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        if (toUpdate.TopicOrder > 0)
                        {
                            var next = _context.SiteApplications.Where(d => d.TopicOrder > toUpdate.TopicOrder).OrderBy(d => d.TopicOrder).FirstOrDefault();
                            if (next != null)
                            {
                                int nextNo = next.TopicOrder;
                                next.TopicOrder = toUpdate.TopicOrder;
                                toUpdate.TopicOrder = nextNo;
                                _context.SiteApplications.Update(next);
                                _context.SiteApplications.Update(toUpdate);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            //already on top
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void MoveRight(int id)
        {
            try
            {
                var toUpdate = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
                if (toUpdate != null)
                {
                    if (toUpdate.ParentId > 0)
                    {
                        var sieblings = _context.SiteApplications.Count(d => d.ParentId == toUpdate.ParentId);
                        if (sieblings > 1)
                        {
                            var siebling = _context.SiteApplications.Where(d => d.ParentId == toUpdate.ParentId && d.ApplicationID != toUpdate.ApplicationID && d.TopicOrder <= toUpdate.TopicOrder).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                            if (siebling == null)
                                siebling = _context.SiteApplications.Where(d => d.ParentId == toUpdate.ParentId && d.ApplicationID != toUpdate.ApplicationID).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                            toUpdate.ParentId = siebling.ApplicationID;
                            toUpdate.TopicOrder = siebling.TopicOrder + 1;
                            _context.SiteApplications.Update(toUpdate);
                            _context.SaveChanges();
                        }
                        else
                        { //no sieblings to intent
                        }

                    }
                    else
                    {
                        //main menu
                        var prev = _context.SiteApplications.Where(d => d.TopicOrder < toUpdate.TopicOrder && d.ParentId == 0).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                        if (prev != null)
                        {
                            toUpdate.ParentId = prev.ApplicationID;
                            toUpdate.TopicOrder = _context.SiteApplications.Count(d => d.ParentId == prev.ParentId) + 1;
                            _context.SiteApplications.Update(toUpdate);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void MoveLeft(int id)
        {
            try
            {
                var toUpdate = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
                if (toUpdate != null)
                {
                    if (toUpdate.ParentId > 0)
                    {
                        var parent = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == toUpdate.ParentId);
                        if (parent != null)
                        {
                            var sieblingParent = _context.SiteApplications.Where(d => d.ParentId == parent.ParentId).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                            if (sieblingParent != null)
                            {
                                toUpdate.TopicOrder = sieblingParent.TopicOrder + 1;
                                toUpdate.ParentId = sieblingParent.ParentId;
                                _context.SiteApplications.Update(toUpdate);
                                _context.SaveChanges();
                            }
                        }
                        else
                        { //no sieblings to intent
                        }

                    }
                    else
                    {
                        //main menu
                        //ALREADY MAIN MENU
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void AddMenu(int id, string name)
        {
            try
            {
                if (!String.IsNullOrEmpty(name))
                {
                    var selectedMenu = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
                    if (selectedMenu != null)
                    {
                        if (selectedMenu.ParentId > 0)
                        {
                            var last = _context.SiteApplications.Where(d => d.ParentId == selectedMenu.ParentId).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                            selectedMenu.TopicOrder = last != null ? (last.TopicOrder + 1) : 0;
                            selectedMenu.Title = name;
                            selectedMenu.Heading = name;
                            selectedMenu.Name = name;
                            selectedMenu.ApplicationID = 0;
                            _context.SiteApplications.Add(selectedMenu);
                            _context.SaveChanges();
                        }
                        else
                        {
                            var last = _context.SiteApplications.Where(d => d.ParentId == 0).OrderByDescending(d => d.TopicOrder).FirstOrDefault();
                            selectedMenu.TopicOrder = last != null ? (last.TopicOrder + 1) : 0;
                            selectedMenu.Title = name;
                            selectedMenu.Heading = name;
                            selectedMenu.Name = name;
                            selectedMenu.ApplicationID = 0;
                            _context.SiteApplications.Add(selectedMenu);
                            _context.SaveChanges();
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Delete(int id)
        {
            try
            {
                var toDelete = _context.SiteApplications.FirstOrDefault(d => d.ApplicationID == id);
                if (toDelete != null)
                {
                    _context.SiteApplications.Remove(toDelete);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private SiteApplication GetLastMainMenuItem()
        {
            //CREATE PROCEDURE [dbo].[SiteApplicationsLastMainMenu]AS BEGIN  SELECT TOP 1 * FROM dbo.SiteApplications where ParentId=0 order by TopicOrder desc;END
            SiteApplication item;
            string sql = "EXEC dbo.SiteApplicationsLastMainMenu";
            var result = _context.SiteApplications.FromSqlRaw<SiteApplication>(sql).AsEnumerable();
            return result.FirstOrDefault();
        }

    }
}
