using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MCIApi.API.Controllers
{
    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        protected int? GetCurrentEmployeeId()
        {
            var claim = User.FindFirst("EmployeeId");
            if (claim == null || !int.TryParse(claim.Value, out var employeeId))
                return null;

            return employeeId;
        }

        protected string? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value;
        }
    }
}

