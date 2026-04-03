using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace MyAcademyJWT_Identity.Attributes
{
    public class PackageAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int _requiredLevel;

        public PackageAuthorizeAttribute(int requiredLevel)
        {
            _requiredLevel = requiredLevel;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var packageClaim = user.Claims.FirstOrDefault(c => c.Type == "PackageId");
            if (packageClaim == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            int userPackageLevel = Convert.ToInt32(packageClaim.Value);

            // Elite = 1, Free = 6. 
            // Kullanıcının paketi, istenen seviyeden büyükse (sayısal olarak daha değersizse) reddet
            if (userPackageLevel > _requiredLevel)
            {
                context.Result = new ObjectResult(new { message = "Bu şarkıyı dinlemek için paket seviyenizi yükseltmelisiniz!" })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
