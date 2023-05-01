using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using demo_test_api.Data;
using demo_test_api.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using NuGet.Versioning;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace demo_test_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    { 

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly AuthenticationContext _context;

        public AuthenticationsController(IConfiguration configuration, IMapper mapper, AuthenticationContext context)
        {
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Authentications
        [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<Authentication>>> GetAuthentication()
        {
          if (_context.Authentication == null)
          {
              return NotFound();
          }

            var authentication = await _context.Authentication.ToListAsync();
            var authenticationDTO = _mapper.Map<List<Authentication>>(authentication);

            return authenticationDTO;
        }

        // GET: api/Authentications/5
        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<Authentication>> GetAuthentication(Guid id)
        {
          if (_context.Authentication == null)
          {
              return NotFound();
          }
            var authentication = await _context.Authentication.FindAsync(id);

            if (authentication == null)
            {
                return NotFound();
            }

            return authentication;
        }

        // PUT: api/Authentications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutAuthentication(Guid id, Authentication authentication)
        {
            if (id != authentication.uuid)
            {
                return BadRequest();
            }

            _context.Entry(authentication).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthenticationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Authentications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TokenDTO>> PostAuthentication(AuthenticationDTO authenticationDTO)
        {
          if (_context.Authentication == null)
          {
              return NotFound();
          }

            var authentication = await _context.Authentication.FirstOrDefaultAsync(src => src.username == authenticationDTO.username && src.password == authenticationDTO.password);
            var authenticationDTOMapping = _mapper.Map<AuthenticationDTO>(authentication);
            var tokenDTO = new TokenDTO();
            if (authentication != null)
            {
                tokenDTO.uuid = authentication.uuid;
                tokenDTO.username = authentication.username;
                tokenDTO.token = GenerateToken(authenticationDTOMapping);
            }

            return CreatedAtAction("GetAuthentication", new { tokenDTO.token }, tokenDTO);
        }

        // DELETE: api/Authentications/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAuthentication(Guid id)
        {
            if (_context.Authentication == null)
            {
                return NotFound();
            }
            var authentication = await _context.Authentication.FindAsync(id);
            if (authentication == null)
            {
                return NotFound();
            }

            _context.Authentication.Remove(authentication);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthenticationExists(Guid id)
        {
            return (_context.Authentication?.Any(e => e.uuid == id)).GetValueOrDefault();
        }

        private string GenerateToken(AuthenticationDTO authentication)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, authentication.username)
            };
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
