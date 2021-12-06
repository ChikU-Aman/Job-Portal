using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Models;

namespace Job_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobesAPIController : ControllerBase
    {
        private readonly JobPortalContext _context;

        public JobesAPIController(JobPortalContext context)
        {
            _context = context;
        }

        // GET: api/JobesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jobe>>> GetJobes()
        {
            return await _context.Jobes.ToListAsync();
        }
    }
}
