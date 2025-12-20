using System;
using System.Collections.Generic;
using System.Linq;
using urunsatisportali.Data;
using urunsatisportali.Models;

namespace urunsatisportali.Repositories
{
    public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context = context;

        public ResultModel<List<T>> GetAll()
        {
            try
            {
                var data = _context.Set<T>().Where(x => !x.IsDeleted).ToList();
                return new ResultModel<List<T>>
                {
                    IsSuccess = true,
                    Data = data,
                    StatusCode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<T>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public ResultModel<T> GetById(int id)
        {
            try
            {
                var data = _context.Set<T>().FirstOrDefault(x => x.Id == id && !x.IsDeleted);
                if (data == null)
                {
                    return new ResultModel<T>
                    {
                        IsSuccess = false,
                        Message = "Not Found",
                        StatusCode = 404
                    };
                }

                return new ResultModel<T>
                {
                    IsSuccess = true,
                    Data = data,
                    StatusCode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public ResultModel<T> Add(T entity)
        {
            try
            {
                entity.CreatedAt = DateTime.Now;
                entity.IsActive = true;
                entity.IsDeleted = false;

                _context.Set<T>().Add(entity);
                _context.SaveChanges();

                return new ResultModel<T>
                {
                    IsSuccess = true,
                    Data = entity,
                    StatusCode = 201,
                    Message = "Created"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public ResultModel<T> Update(T entity)
        {
            try
            {
                var existing = _context.Set<T>().Find(entity.Id);
                if (existing == null)
                {
                    return new ResultModel<T>
                    {
                        IsSuccess = false,
                        Message = "Not Found",
                        StatusCode = 404
                    };
                }

                entity.UpdatedAt = DateTime.Now;
                _context.Entry(existing).CurrentValues.SetValues(entity);
                _context.SaveChanges();

                return new ResultModel<T>
                {
                    IsSuccess = true,
                    Data = entity,
                    StatusCode = 200,
                    Message = "Updated"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<T>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public ResultModel<bool> Delete(int id)
        {
            try
            {
                var entity = _context.Set<T>().Find(id);
                if (entity == null)
                {
                    return new ResultModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Not Found",
                        StatusCode = 404,
                        Data = false
                    };
                }

                // Soft Delete
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.Now;
                _context.SaveChanges();

                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    StatusCode = 200,
                    Message = "Deleted"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500,
                    Data = false
                };
            }
        }
    }
}
