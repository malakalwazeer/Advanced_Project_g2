using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;



namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicCertificationsController : ControllerBase
    {
        private readonly CertificationLookupService _lookupService;

        public PublicCertificationsController(CertificationLookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [AllowAnonymous]
        [HttpPost("verify")]
        //to add API documentation
        [ProducesResponseType(typeof(CertificationLookupResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CertificationLookupResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyCertificate(CertificationLookupDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _lookupService.VerifyAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
