using Microsoft.AspNetCore.Mvc;

namespace FunDo.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
