using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ProgLibrary.API.Controllers
{
    [Route("[controller]")]
    public class ApiControllerBase : Controller
    {
        protected Guid UserId => User?.Identity?.IsAuthenticated == true ?
         Guid.Parse(User.Identity.Name) : // parsujemy na Guid nazwe Usera
         Guid.Empty; // jeśli token jest niepoprawny albo nie istenieje return Empty Guid
        


    }
}
