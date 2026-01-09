using JobTracker.api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.api.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class JobApplicationsController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await db.JobApplications
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
            return Ok(list);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await db.JobApplications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(item);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateJobApplicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CompanyName) || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("CompanyName et Title sont obligatoires.");

            var app = new JobApplication
            {
                CompanyName = dto.CompanyName.Trim(),
                Title = dto.Title.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            db.JobApplications.Add(app);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = app.Id }, app);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateJobApplicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CompanyName) || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("CompanyName et Title sont obligatoires.");

            var app = await db.JobApplications.FirstOrDefaultAsync(x => x.Id == id);
            if (app is null) return NotFound();

            app.CompanyName = dto.CompanyName.Trim();
            app.Title = dto.Title.Trim();

            await db.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var app = await db.JobApplications.FirstOrDefaultAsync(x => x.Id == id);
            if (app is null) return NotFound();

            app.IsDeleted = true;
            app.DeletedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("{id:guid}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            var app = await db.JobApplications
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (app is null) return NotFound();

            app.IsDeleted = false;
            app.DeletedAtUtc = null;

            await db.SaveChangesAsync();
            return NoContent();
        }

        public record CreateJobApplicationDto(string CompanyName, string Title);
        public record UpdateJobApplicationDto(string CompanyName, string Title);

    }
}
