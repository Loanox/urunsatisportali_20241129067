using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using urunsatisportali.Models;

namespace urunsatisportali.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        ResultModel<List<T>> GetAll();
        ResultModel<T> GetById(int id);
        ResultModel<T> Add(T entity);
        ResultModel<T> Update(T entity);
        ResultModel<bool> Delete(int id);
    }
}
