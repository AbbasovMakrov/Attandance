using Attendance.Data;
using Attendance.Data.Entities;
using Attendance.Models;
using Attendance.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Attendance.Configurations;
using Attendance.Extenstions;
using Attendance.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace Attendance.Controllers
{

    public class ExcelUploadsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment env;
        private readonly IFileService fileService;
        private readonly ILogger<ExcelUploadsController> _logger;

        public ExcelUploadsController(ApplicationDbContext dbContext, IWebHostEnvironment env, IFileService fileService,
            ILogger<ExcelUploadsController> logger)
        {
            this.dbContext = dbContext;
            this.env = env;
            this.fileService = fileService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var files = await dbContext.ExcelFiles
                .OrderByDescending(e => e.UploadedAt)
                .ToArrayAsync();
            return View(files);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UploadExcelViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string fileName = StringExtensions.Random(44) + Path.GetExtension(vm.File.FileName);
            await fileService.Upload(vm.File, new Services.FileOptions(fileName, env.WebRootPath + "/ExcelFiles"));
            await dbContext.ExcelFiles.AddAsync(new ExcelFile
            {
                FilePath = "ExcelFiles/" + fileName,
                IsParsed = false,
                UploadedAt = DateTime.Now
            });
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(HomeController.Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Parse([FromRoute] Guid id,[FromServices] IOptions<ExcelColumnsConfigurations> excelOptions)
        {

            var excelFile = await dbContext.ExcelFiles.Where(e => !e.IsParsed && e.Id == id).FirstOrDefaultAsync();
            if (excelFile is null)
            {
                return NotFound();
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var attendances = new List<AttendanceExcel>();
            try
            {
                await using (var fs = new FileStream(Path.Combine(env.WebRootPath, excelFile.FilePath), FileMode.Open))
                {
                    using (var excel = new ExcelPackage(fs))
                    {

                        var sheet = excel.Workbook.Worksheets[0];
                        Enumerable.Range(excelOptions.Value.RowStartIndex, sheet.Dimension.End.Row).ToList().ForEach(i =>
                        {
                            var employeeId = sheet.Cells[i, excelOptions.Value.IdIndex];
                            employeeId.DataValidation.AddIntegerDataValidation();
                            var date = sheet.Cells[i, excelOptions.Value.DateIndex];
                            date.DataValidation.AddDateTimeDataValidation();
                            var signInTime = sheet.Cells[i, excelOptions.Value.SignInTimeIndex];
                            var signOutTime = sheet.Cells[i, excelOptions.Value.SignOutTimeIndex];
                            signInTime.DataValidation.AddTimeDataValidation();
                            signOutTime.DataValidation.AddTimeDataValidation();

                            if (employeeId.Text != string.Empty)
                            {
                                bool.TryParse(sheet.Cells[i, excelOptions.Value.IsAbsentIndex].Text, out var isAbsent);
                                var attendance = new AttendanceExcel
                                {
                                    Id = int.Parse(employeeId.Text),
                                    Date = Convert.ToDateTime(date.Text),
                                    SignInTime = isAbsent || signInTime.Text == string.Empty ? TimeSpan.Zero : TimeSpan.Parse(signInTime.Text),
                                    SignOutTime = isAbsent || signOutTime.Text == string.Empty ? TimeSpan.Zero : TimeSpan.Parse(signOutTime.Text),
                                    IsAbsent = isAbsent
                                };
                               // _logger.LogTrace($"date : {attendance.Date} is absent ? : {isAbsent}");
                                attendances.Add(attendance);
                            }
                        });

                    }
                }
                foreach (var iteration in attendances.GroupBy(a => a.Id))
                {
                    var employee = await  dbContext.Employees
                        .Include(e => e.Job)
                            .ThenInclude(j => j.Discounts)
                        .Include(e => e.Budget)
                        .Include(e => e.MoneyTransactions)
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(e => e.ExcelId == iteration.FirstOrDefault().Id);
                    if (employee is not null)
                    {
                        var lastTransaction = employee.MoneyTransactions.LastOrDefault(x => x.Type == MoneyTransaction.TransactionType.In);
                        if (lastTransaction is not null)
                        {
                            var deltaTransactionTimes = (int)(DateTime.Now - lastTransaction.CreatedAt).Value.TotalDays;
                            if (deltaTransactionTimes >= employee.WorkDaysCount)
                            {
                                employee.MoneyTransactions.Add(new MoneyTransaction("Salary Paid", MoneyTransaction.TransactionType.In, employee.Salary));
                                employee.Budget.Amount += employee.Salary;
                            }
                        }
                        else
                        {
                            employee.MoneyTransactions.Add(new MoneyTransaction("Salary Paid", MoneyTransaction.TransactionType.In, employee.Salary));
                            employee.Budget.Amount += employee.Salary;
                        }

                        foreach (var attendanceExcel in iteration)
                        {
                            if (attendanceExcel.IsAbsent)
                            {
                                var discount = employee.Job.Discounts.FirstOrDefault(d => d.Type == "absent" || d.Type == "day");
                                employee.Budget.Amount -= discount.Amount;
                                employee.MoneyTransactions.Add(new MoneyTransaction("Absent for one day", MoneyTransaction.TransactionType.Out, discount.Amount));
                            }
                            else
                            {
                                var discount =
                                    employee.Job.Discounts.FirstOrDefault(d => d.Type.Contains("hour") || d.Type.Contains("late"));
                                var deltaTimes = Math.Round((attendanceExcel.SignInTime - employee.Job.StartTime).TotalHours);
                                if (deltaTimes > 0)
                                {
                                    employee.Budget.Amount -= discount.Amount * (decimal)deltaTimes;
                                    employee.MoneyTransactions.Add(new MoneyTransaction($"Late for {deltaTimes} hours", MoneyTransaction.TransactionType.Out, (int)(deltaTimes * discount.Amount)));

                                }
                            }

                            dbContext.Employees.Update(employee);
                        }
                    }

                    
                    excelFile.IsParsed = true;

                    dbContext.ExcelFiles.Update(excelFile);
                   await dbContext.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
            return RedirectToAction(nameof(HomeController.Index));

        }

    }

}

