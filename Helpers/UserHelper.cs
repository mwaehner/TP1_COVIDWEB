using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace TP1_ARQWEB.Helpers
{
    public class UserHelper {

        public static string getUserId(Controller controller)
        {

            var claimsIdentity = controller.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return null;

            // the principal identity is a claims identity.
            // now we need to find the NameIdentifier claim
            var userIdClaim = claimsIdentity.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);


            return userIdClaim.Value;



        }    
    }
}
