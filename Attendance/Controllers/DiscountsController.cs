using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attendance.Data;
using Attendance.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Attendance.Controllers
{
    [Authorize]
    public class DiscountsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DiscountsController> _logger;

        public DiscountsController(ApplicationDbContext dbContext,ILogger<DiscountsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var discounts = await _dbContext.Discounts.OrderByDescending(x => x.UpdatedAt)
                .ToArrayAsync();
            return View(discounts);
        }

        public IActionResult Create() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _dbContext.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation(model.UpdatedAt.ToString());
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var discount = await _dbContext.Discounts.FindAsync(id);
            if (discount is null) return NotFound();
            return View(discount);
        }
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var discount = await _dbContext.Discounts.FindAsync(id);
            if (discount is null) return NotFound();
            return View(discount);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] Guid id ,
            [Bind(include: new string[] { nameof(Discount.Amount) , nameof(Discount.Type) })] Discount model)
        {
            if (!ModelState.IsValid) return View(model);
            if (!await _dbContext.Discounts.AnyAsync(x => x.Id == id)) return NotFound();
            model.Id = id;
            _dbContext.Discounts.Update(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var discount = await _dbContext.Discounts.FindAsync(id);
            if (discount is null) return NotFound();
            _dbContext.Discounts.Remove(discount);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
