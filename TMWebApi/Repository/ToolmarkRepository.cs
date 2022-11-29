using TMWebApi.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;


namespace TMWebApi.Repository
{
 
    public interface IToolmarkRepository
    {
        Task<IEnumerable<Toolmark>> GetToolmarks();
        Task<Toolmark> GetToolmarkByID(int ID);
        Task<Toolmark> InsertToolmark(Toolmark objToolmark);
        Task<Toolmark> UpdateToolmark(Toolmark objToolmark);
        bool DeleteToolmark(int ID);
    }
    public class ToolmarkRepository : IToolmarkRepository
    {

        private readonly APIDbContext _appDBContext;

        public ToolmarkRepository(APIDbContext context)
        {
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Toolmark>> GetToolmarks()
        {
            return await _appDBContext.Toolmarks.ToListAsync();
        }

        public async Task<Toolmark> GetToolmarkByID(int ID)
        {
            return await _appDBContext.Toolmarks.FindAsync(ID);
        }

        public async Task<Toolmark> InsertToolmark(Toolmark objToolmark)
        {
            _appDBContext.Toolmarks.Add(objToolmark);
            await _appDBContext.SaveChangesAsync();
            return objToolmark;
        }

        public async Task<Toolmark> UpdateToolmark(Toolmark objToolmark)
        {
            _appDBContext.Entry(objToolmark).State = EntityState.Modified;
            await _appDBContext.SaveChangesAsync();
            return objToolmark;
        }

        public bool DeleteToolmark(int ID)
        {
            bool result = false;
            var category = _appDBContext.Toolmarks.Find(ID);
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
