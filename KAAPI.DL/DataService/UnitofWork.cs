using KAAPI.DL.IDataService;

namespace KAAPI.DL.DataService
{
    public class UnitofWork : IUnitofWork
    {
        public UnitofWork(IAuthenicationDL authenicationDL)
        {
            AuthenicationDL = authenicationDL;
        }

        public IAuthenicationDL AuthenicationDL { get; }
    }
}