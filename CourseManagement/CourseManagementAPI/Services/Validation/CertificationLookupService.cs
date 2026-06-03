using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Services.Validation
{
    public class CertificationLookupService
    {
        //this validation checks:
//        Reference format is correct
//        Reference belongs to the trainee
//        Trainee exists
//        Certification exists
//        Certification has required courses
//        Trainee passed all required courses


        private readonly CourseManagementDbContext _context;

        public CertificationLookupService(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<CertificationLookupResultDto> VerifyAsync(CertificationLookupDto dto)
        {
            var parsed = TryParseCertificateReference(
                dto.CertificateReferenceNumber,
                out int referenceTraineeId,
                out int certificationId
            );

            if (!parsed)
            {
                return new CertificationLookupResultDto
                {
                    IsValid = false,
                    Message = "Invalid certificate reference format."
                };
            }

            if (referenceTraineeId != dto.TraineeId)
            {
                return new CertificationLookupResultDto
                {
                    IsValid = false,
                    Message = "Certificate reference does not match the trainee ID."
                };
            }

            var trainee = await _context.Trainees
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TraineeId == dto.TraineeId);

            if (trainee == null)
            {
                return new CertificationLookupResultDto
                {
                    IsValid = false,
                    Message = "Trainee was not found."
                };
            }

            var certification = await _context.Certifications
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CertificationId == certificationId);

            if (certification == null)
            {
                return new CertificationLookupResultDto
                {
                    IsValid = false,
                    Message = "Certification was not found."
                };
            }

            var requiredCourses = await _context.CertificationCourses
                .AsNoTracking()
                .Include(cc => cc.Course)
                .Where(cc => cc.CertificationId == certificationId)
                .ToListAsync();

            if (!requiredCourses.Any())
            {
                return new CertificationLookupResultDto
                {
                    IsValid = false,
                    Message = "This certification has no linked courses configured.",
                    TraineeId = trainee.TraineeId,
                    TraineeName = trainee.FullName,
                    CertificationId = certification.CertificationId,
                    CertificationName = certification.Name
                };
            }

            var passedCourseIds = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.TraineeId == dto.TraineeId)
                .Where(e => e.Assessments.Any(a => a.Result == 1))
                .Select(e => e.Session.CourseId)
                .Distinct()
                .ToListAsync();

            var completedCourses = requiredCourses
                .Where(rc => passedCourseIds.Contains(rc.CourseId))
                .Select(rc => rc.Course.CourseName)
                .ToList();

            var missingCourses = requiredCourses
                .Where(rc => !passedCourseIds.Contains(rc.CourseId))
                .Select(rc => rc.Course.CourseName)
                .ToList();

            var completedCount = completedCourses.Count;
            var requiredCount = requiredCourses.Count;

            var progress = Math.Round((decimal)completedCount / requiredCount * 100, 2);

            var isValid = missingCourses.Count == 0;

            return new CertificationLookupResultDto
            {
                IsValid = isValid,
                Message = isValid
                    ? "Certificate is valid. The trainee completed all required courses."
                    : "Certificate is not valid yet. The trainee has not completed all required courses.",

                TraineeId = trainee.TraineeId,
                TraineeName = trainee.FullName,
                CertificationId = certification.CertificationId,
                CertificationName = certification.Name,
                RequiredCoursesCount = requiredCount,
                CompletedCoursesCount = completedCount,
                ProgressPercentage = progress,
                CompletedCourses = completedCourses,
                MissingCourses = missingCourses
            };
        }

        private bool TryParseCertificateReference(
            string reference,
            out int traineeId,
            out int certificationId)
        {
            traineeId = 0;
            certificationId = 0;

            var parts = reference.Split("-");

            if (parts.Length != 3)
            {
                return false;
            }

            if (parts[0] != "CERT")
            {
                return false;
            }

            var traineeParsed = int.TryParse(parts[1], out traineeId);
            var certificationParsed = int.TryParse(parts[2], out certificationId);

            return traineeParsed && certificationParsed;
        }
    }
}
