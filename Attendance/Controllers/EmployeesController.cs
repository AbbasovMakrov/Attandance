using Attendance.Data;
using Attendance.Data.Entities;
using Attendance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
        public async Task<IActionResult> Index()
        {
            var employees = await  _dbContext.Employees
                .Include(e => e.Job)
                .ToArrayAsync();
            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            return View(new EmployeeViewModel
            {
                Jobs = await _dbContext.Jobs.ToArrayAsync(),
                EmployeeModel = new Employee()
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: new [] { 
          nameof(EmployeeViewModel.EmployeeModel),
          nameof(EmployeeViewModel.EmployeeModel.JobId),
          nameof(EmployeeViewModel.EmployeeModel.Name),
          nameof(EmployeeViewModel.EmployeeModel.ExcelId),
            nameof(EmployeeViewModel.EmployeeModel.Salary),
            nameof(EmployeeViewModel.EmployeeModel.WorkDaysCount)
        })] EmployeeViewModel vm)
        {
            
            if(!ModelState.IsValid)
            {
                vm.Jobs = await _dbContext.Jobs.ToArrayAsync();
                return View(vm);
            }
            if(await _dbContext.Employees.AnyAsync(e => e.ExcelId == vm.EmployeeModel.ExcelId))
            {
                     ModelState.AddModelError(nameof(vm.EmployeeModel.ExcelId), "please try putting anothor excel id ");
                    vm.Jobs = await _dbContext.Jobs.ToArrayAsync();
                    return View(vm);
            }

            vm.EmployeeModel.Budget = new Budget {Amount = 0};
            await _dbContext.Employees.AddAsync(vm.EmployeeModel);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.Job)
                .Include(e => e.MoneyTransactions)
                .Include(e => e.Budget)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id);
            if (employee is null) return NotFound();
            return View(employee);
        }

       
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            
            var employee = await _dbContext.Employees.Include(e => e.Job)
                 .FirstOrDefaultAsync(e => e.Id == id);
            if (employee is null) return NotFound();
            return View(new EmployeeViewModel
            {
                 EmployeeModel = employee,
                 Jobs = await _dbContext.Jobs.ToArrayAsync()
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(include: new [] {
          nameof(EmployeeViewModel.EmployeeModel),
          nameof(EmployeeViewModel.EmployeeModel.JobId),
          nameof(EmployeeViewModel.EmployeeModel.Name),
          nameof(EmployeeViewModel.EmployeeModel.ExcelId),
          nameof(EmployeeViewModel.EmployeeModel.Salary),
          nameof(EmployeeViewModel.EmployeeModel.CreatedAt),
          nameof(EmployeeViewModel.EmployeeModel.WorkDaysCount)


        })] EmployeeViewModel vm)
        {
            
            if (!await _dbContext.Employees.AnyAsync(e => e.Id == vm.EmployeeModel.Id && e.ExcelId == vm.EmployeeModel.ExcelId ) ) return NotFound("Employee not found");
            if(!ModelState.IsValid)
            {
                vm.Jobs = await _dbContext.Jobs.ToArrayAsync();
                return View(vm);
            }
            
            _dbContext.Employees.Update(vm.EmployeeModel);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee is null) return NotFound();
            _dbContext.Remove(employee);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
