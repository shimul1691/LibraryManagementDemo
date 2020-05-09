using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
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
                Microsoft.AspNetCore.Http.HttpContext currentHttpContext = HttpContext;
                string userid = HttpContext.Session.GetString("UserID");

                int result = 0;
                if (!string.IsNullOrEmpty(userid))
                {
                    Random rnd = new Random();
                    int requestCode = rnd.Next(10000000, 99999999);

                    SqlParameter returnCode = new SqlParameter
                    {
                        ParameterName = "@returnCode",
                        SqlDbType = System.Data.SqlDbType.BigInt,
                        Direction = System.Data.ParameterDirection.Output
                    };

                   // int sReturnCode = 0;

                    /*
                    _context.Database.ExecuteSqlCommand("[dbo].[RequestBook] @bookId = {0}, @userId = {1}, @RCode = {2}, @returnCode ={3} OUTPUT",
                        Convert.ToInt32(bookId), Convert.ToInt32(userid), requestCode.ToString(), sReturnCode);
                        */

                   result = _context.Database.ExecuteSqlCommand("[dbo].[RequestBook] @bookId = {0}, @userId = {1}, @RCode = {2}",
                        Convert.ToInt32(bookId), Convert.ToInt32(userid), requestCode.ToString());
                    

               
                    if (result.Equals(2))
                    {
                        json = new JsonResult(requestCode);
                    }
                }
            }

            return json;
        }
    }
}