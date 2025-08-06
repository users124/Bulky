using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    internal interface ICategoryRepositrory: IRepository<Category>
    {
        void Update(Category category);
        void Save();
    }
}
