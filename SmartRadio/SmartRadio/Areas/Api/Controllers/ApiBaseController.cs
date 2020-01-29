using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRadio.Areas.Api.Controllers
{
    [Area("Api")]
    [Authorize(Roles = "Radio")]
    public abstract class ApiBaseController : Controller
    {
        
    }
}