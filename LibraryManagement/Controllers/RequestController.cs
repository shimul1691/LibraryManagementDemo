using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class RequestController : Controller
    {
        private readonly LibraryManagementContext _context;

        public RequestController(LibraryManagementContext context)
        {
            _context = context;
        }
        [HttpGet]
        public JsonResult Index()
        {
            JsonResult json = new JsonResult("yougothere");
            return json;
        }

        [HttpPost]
        public JsonResult Create(string bookId)
        {
            JsonResult json = new JsonResult("Request Failed");

            if (string.IsNullOrEmpty(bookId))
            {
                json = new JsonResult("bad request");
            }
            else
            {
                //var result = _context.Request.FromSqlRaw("[dbo].[RequestBook] @bookId = {0}, @userId = {1}", Convert.ToInt32(bookId), 1);


                var result = _context.Database.ExecuteSqlCommand("[dbo].[RequestBook] @bookId = {0}, @userId = {1}", Convert.ToInt32(bookId), 1);



                if (result.Equals(1))
                {
                    json = new JsonResult("Success");
                }
            }

            return json;
        }
    }
}