using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
  [ServiceFilter(typeof(LogUserActivity))]
  [ApiController]
  [Route("api/[controller]")]
  public class BaseApiController : ControllerBase
  {

  }
}