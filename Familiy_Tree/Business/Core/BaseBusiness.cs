using Data;

namespace Business.Core
{
    public class BaseBusiness
    {
        protected readonly UnitOfWork uow;

        public BaseBusiness(UnitOfWork _uow)
        {
            uow = _uow;
        }
    }
}
