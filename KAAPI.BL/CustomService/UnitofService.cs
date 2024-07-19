using KAAPI.BL.ICustomService;

namespace KAAPI.BL.CustomService
{
    public class UnitofService : IunitofService
    {
        public UnitofService( IAuthenticationBL authenticationBL) 
        { 
            AuthenticationBL = authenticationBL;
        }
        public IAuthenticationBL AuthenticationBL { get;}
    }
}
