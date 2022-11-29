using TMWebApi.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace TMWebApi.Repository
{
    public interface ICategoryRepository
    {

        Task<IEnumerable<Category>> GetCategory();
        Task<Category> GetCategoryByID(int ID);
        Task<Category> InsertCategory(Category objCategory);
        Task<Category> UpdateCategory(Category objCategory);
        bool DeleteCategory(int ID);

    }


    public class CategoryRepository : ICategoryRepository
    {

        private readonly APIDbContext _appDBContext;

        public CategoryRepository(APIDbContext context)
        {
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Category>> GetCategory()
        {
            return await _appDBContext.Categorys.ToListAsync();
        }

        public async Task<Category> GetCategoryByID(int ID)
        {
            return await _appDBContext.Categorys.FindAsync(ID);
        }

        public async Task<Category> InsertCategory(Category objCategory)
        {
            _appDBContext.Categorys.Add(objCategory);
            await _appDBContext.SaveChangesAsync();
            return objCategory;

        }

        public async Task<Category> UpdateCategory(Category objCategory)
        {
            _appDBContext.Entry(objCategory).State = EntityState.Modified;
            await _appDBContext.SaveChangesAsync();
            return objCategory;

        }

        public bool DeleteCategory(int ID)
        {
            bool result = false;
            var category = _appDBContext.Categorys.Find(ID);
            if (category != null)
            {
                _appDBContext.Entry(category).State = EntityState.Deleted;
                _appDBContext.SaveChanges();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
  
    
    
    }
  }
