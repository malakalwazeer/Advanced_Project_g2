using CourseManagement.ViewModels;
using CourseManagementAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CourseManagement.Controllers;

public class PublicCertificationLookupController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PublicCertificationLookupController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new PublicCertificationLookupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(PublicCertificationLookupViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _httpClientFactory.CreateClient("ApiClient");

        var requestDto = new CertificationLookupDto
        {
            TraineeId = model.TraineeId,
            CertificateReferenceNumber = model.CertificateReferenceNumber
        };

        try
        {
            var response = await client.PostAsJsonAsync("api/PublicCertifications/verify", requestDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CertificationLookupResultDto>();

                if (result is not null)
                {
                    model.HasResult = true;
                    model.IsValid = result.IsValid;
                    model.Message = result.Message;
                    model.TraineeName = result.TraineeName;
                    model.CertificationName = result.CertificationName;
                    model.RequiredCoursesCount = result.RequiredCoursesCount;
                    model.CompletedCoursesCount = result.CompletedCoursesCount;
                    model.ProgressPercentage = result.ProgressPercentage;
                    model.CompletedCourses = result.CompletedCourses ?? new List<string>();
                    model.MissingCourses = result.MissingCourses ?? new List<string>();
                }
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<CertificationLookupResultDto>();

                model.HasResult = true;
                model.IsValid = false;

                if (result is not null)
                {
                    model.Message = result.Message;
                    model.TraineeName = result.TraineeName;
                    model.CertificationName = result.CertificationName;
                    model.RequiredCoursesCount = result.RequiredCoursesCount;
                    model.CompletedCoursesCount = result.CompletedCoursesCount;
                    model.ProgressPercentage = result.ProgressPercentage;
                    model.CompletedCourses = result.CompletedCourses ?? new List<string>();
                    model.MissingCourses = result.MissingCourses ?? new List<string>();
                }
                else
                {
                    model.ErrorMessage = "Certificate verification failed. Please check the trainee ID and reference number.";
                }
            }
        }
        catch (HttpRequestException)
        {
            model.HasResult = true;
            model.IsValid = false;
            model.ErrorMessage = "The certification service is currently unavailable. Please try again later.";
        }

        return View(model);
    }
}
