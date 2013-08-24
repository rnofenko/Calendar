using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    public class FakePersonalEventRepository : FakeBaseRepository<PersonalEventLink>, IPersonalEventRepository
    {
    }
}
