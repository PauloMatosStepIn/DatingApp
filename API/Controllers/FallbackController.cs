using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  //NG App is our API View 
  public class FallbackController : Controller
  {
    public ActionResult Index()
    { 
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
        "wwwroot", "index.html"),
        "text/HTML"
        );
    }
  }
}