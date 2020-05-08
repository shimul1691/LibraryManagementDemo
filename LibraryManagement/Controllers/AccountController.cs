using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryManagementContext _context;

        public AccountController(LibraryManagementContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> RegisterAccount()
        {
            return View();
        }
        


        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAccount([Bind("FirstName,LastName,Nidno,PassportNo,Email,MobileNo,HomeAddress,City,Country,DateOfBirth,Uname,Upassword,ConfirmPassword")] LibraryUserRegistrationRequest libraryUserRequest)
        {
            if (ModelState.IsValid)
            {
                _context.LibraryUserRegistrationRequest.Add(libraryUserRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libraryUserRequest);
        }

        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryUser = await _context.LibraryUser.FindAsync(id);
            if (libraryUser == null)
            {
                return NotFound();
            }
            return View(libraryUser);
        }

        
        private bool LibraryUserExists(int id)
        {
            return _context.LibraryUser.Any((System.Linq.Expressions.Expression<Func<LibraryUser, bool>>)(e => e.UserId == id));
        }
    }
}
