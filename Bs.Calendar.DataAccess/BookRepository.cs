using System.Data.Entity;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public override Book Get(int id)
        {
            return Load().Where(x => x.Id == id).Include(x => x.BookHistories).FirstOrDefault();
        }
    }
}