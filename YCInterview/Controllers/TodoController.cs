using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using YCInterview.Models;

namespace YCInterview.Controllers
{
    public class TodoController : Controller
    {
        private readonly TodoDbContext _context;

        public TodoController(TodoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.TodoItems.ToList();
            return View(items);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoItem item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.TodoItems.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TodoItem item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TodoItems.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.TodoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportExcel()
        {
            var todos = await _context.TodoItems.ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Todo List");

            // 標題列
            var names = typeof(TodoItem)
                    .GetProperties()
                    .ToDictionary(
                        prop => prop.Name,
                        prop => prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                    );
            worksheet.Cell(1, 1).Value = names["Title"];
            worksheet.Cell(1, 2).Value = names["IsDone"];

            if (todos == null || todos.Count == 0) // 資料庫內沒資料
            {
                using var emptyStream = new MemoryStream();
                workbook.SaveAs(emptyStream);
                emptyStream.Seek(0, SeekOrigin.Begin);
                return File(emptyStream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "待辦事項.xlsx");
            }

            // 資料列
            for (int i = 0; i < todos.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = todos[i].Title;
                worksheet.Cell(i + 2, 2).Value = todos[i].IsDone ? "完成" : "未完成";
            }

            // 產生記憶體中的 Excel 檔案
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // 回傳 Excel 檔案給使用者下載
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "待辦事項.xlsx");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
