using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TMWebApi.Models;
using TMWebApi.Repository;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace TMWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        //Note: production: switch sql stmts as procedure 

        private readonly IConfiguration _configuration;
        private readonly ICategoryRepository _category;


        public CategoryController(ICategoryRepository category, IConfiguration configuration)
        {
            _category = category ?? throw new ArgumentNullException(nameof(category));
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetCategory")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _category.GetCategory());
        }

        [HttpGet]
        [Route("GetCategoryByID/{Id}")]
        public async Task<IActionResult> GetDeptById(int Id)
        {
            return Ok(await _category.GetCategoryByID(Id));
        }


        [HttpPost]
        [Route("AddCategory")]
        public JsonResult Post(Category dep)
        {
            string query = @"
                    insert into dbo.Category values 
                    ('" + dep.Type + @"')
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("CompanyCOnnStr");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }



        [HttpPut]
        [Route("UpdateCategory")]
        public async Task<IActionResult> Put(Category dep)
        {
            await _category.UpdateCategory(dep);
            return Ok("Updated Successfully");
        }


        [HttpDelete]
        [Route("DeleteCategory")]
        public JsonResult Delete(int id)
        {
            _category.DeleteCategory(id);
            return new JsonResult("Deleted Successfully");
        }
    }
}
