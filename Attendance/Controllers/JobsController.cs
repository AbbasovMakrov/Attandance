using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attendance.Data;
using Microsoft.EntityFrameworkCore;
using Attendance.Models;
using Attendance.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Attendance.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public JobsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var jobs = await _dbContext.Jobs
                .OrderByDescending(j => j.CreatedAt)
                .Include(j => j.Discounts)
                .ToArrayAsync();
            return View(jobs);
        }
        [HttpGet]
        [ActionName("Create")]
        public async  Task<IActionResult> Create()
        {
            var vm = new JobViewModel
            {
                Discounts = await _dbContext.Discounts.ToArrayAsync(),            
            };
            return View(vm);
        }
        public async Task<IActionResult> Details([FromRoute]Guid id)
        {
            var job = await _dbContext.Jobs
                .Include(x => x.Discounts)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (job is null) return NotFound();
            return View(job);
        }
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Create([
            Bind(include: new [] {
            nameof(JobViewModel.JobModel),
            nameof(JobViewModel.JobModel.MinSalary) ,
            nameof(JobViewModel.JobModel.StartTime) ,
            nameof(JobViewModel.JobModel.MaxSalary),
            nameof(JobViewModel.DiscountsIds),
            nameof(JobViewModel.JobModel.Name),
        })] JobViewModel vm)
        {
            if (vm.JobModel.MinSalary > vm.JobModel.MaxSalary)
                ModelState.AddModelError(nameof(JobViewModel.JobModel.MinSalary),"Min salary can not be bigger than the max salary");
            if (vm.JobModel.MaxSalary < vm.JobModel.MinSalary)
                ModelState.AddModelError(nameof(JobViewModel.JobModel.MaxSalary), "Max salary can not be less than the min salary");
         
            if (!ModelState.IsValid)
            {
                vm.Discounts = await _dbContext.Discounts.ToArrayAsync();
                return View(vm);
            }
            vm.JobModel.Discounts = await _dbContext.Discounts.Where(d => vm.DiscountsIds.Contains(d.Id)).ToArrayAsync();
            await _dbContext.Jobs.AddAsync(vm.JobModel);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute]Guid id)
        {
            var job = await _dbContext.Jobs.FindAsync(id);
            if (job is null) return NotFound();
            var discounts = await _dbContext.Discounts.ToArrayAsync();
            return View(new JobViewModel { Discounts = discounts,JobModel = job });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit([
            Bind(include: new [] {
            nameof(JobViewModel.JobModel),
            nameof(JobViewModel.JobModel.MinSalary) ,
            nameof(JobViewModel.JobModel.StartTime) ,
            nameof(JobViewModel.JobModel.MaxSalary),
            nameof(JobViewModel.JobModel.Name),
            nameof(JobViewModel.JobModel.Id),
            nameof(JobViewModel.JobModel.CreatedAt),
            nameof(JobViewModel.DiscountsIds),
        })] JobViewModel vm)
        {
            if (!await _dbContext.Jobs.AnyAsync(x => x.Id == vm.JobModel.Id))
               return NotFound();
            if (!ModelState.IsValid)
            {
                vm.Discounts = await _dbContext.Discounts.ToArrayAsync();
                return View(vm);
            }
            vm.JobModel.Discounts = await _dbContext.Discounts.Where(d => vm.DiscountsIds.Contains(d.Id)).ToArrayAsync();

            _dbContext.Jobs.Update(vm.JobModel);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var job = await _dbContext.Jobs.FindAsync(id);
            if (job is null) return NotFound();
            _dbContext.Jobs.Remove(job);
           await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
