using System;
using Microsoft.AspNetCore.Mvc;
using TMWebApi.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using TMWebApi.Repository;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;


namespace TMWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolmarkController : ControllerBase
    {
        //Note: production: switch sql stmts as procedure 

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ToolmarkController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        [Route("GetToolmark")]
        public JsonResult Get()
        {
            string query = @"
                    select ToolmarkID,CaseNumber,Category,Email,Note,
                    convert(varchar(10),DateOfCollected,120) as DateOfCollected
                    ,ImageFileName
                    from dbo.Toolmark
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

            return new JsonResult(table);
        }


        [HttpPost]
        [Route("AddToolmark")]
        public JsonResult Post(Toolmark tm)
        {
            string sReturnMsg = string.Empty;

            if (tm.ImageFileName == null)
                tm.ImageFileName = "anonymous.png";

            string query = @"
                    insert into dbo.Toolmark 
                    (CaseNumber,Category,Email,Note,DateOfCollected, ImageFileName)
                    values 
                    (
                    '" + tm.CaseNumber + @"'
                    ,'" + tm.Category + @"'
                    ,'" + tm.Email + @"'
                    ,'" + tm.Note + @"'
                    ,'" + tm.DateOfCollected + @"'
                    ,'" + tm.ImageFileName + @"'
                    )
                    ;SELECT SCOPE_IDENTITY()";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("CompanyCOnnStr");
            int newID;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCon.Open();
                    newID = System.Convert.ToInt32(myCommand.ExecuteScalar());

                    if (myCon.State == System.Data.ConnectionState.Open)  myCon.Close();
                    sReturnMsg = "Added Successfully";

                }
            }


            if (sReturnMsg.Contains("Successfully") == true)
            {
                tm.ToolmarkID = newID;

                return new JsonResult(ObjectToData(tm));
            }
            else
                return new JsonResult(null);

        }


        [HttpPut]
        [Route("UpdateToolmark/{ID}")]
        public JsonResult Put(int ID, [FromBody] Toolmark tm)
        {
            string sReturnMsg = string.Empty;
            if (tm.ImageFileName == null)
                tm.ImageFileName = "anonymous.png";
            // CaseNumber,Category,Email,Note,ImageFileName
            string query = @"
                    update dbo.Toolmark set 
                    CaseNumber = '" + tm.CaseNumber + @"'
                    ,Category = '" + tm.Category + @"'
                    ,Email = '" + tm.Email + @"'
                    ,Note = '" + tm.Note + @"'
                    ,DateOfCollected = '" + tm.DateOfCollected + @"'
                    ,ImageFileName = '" + tm.ImageFileName + @"'
                    where ToolmarkID = " + ID + @" 
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("CompanyCOnnStr");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    try
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader); ;

                        myReader.Close();
                        myCon.Close();

                        sReturnMsg = "Updated Successfully";
                    }
                    catch (Exception Ex)
                    {
                        new JsonResult(Ex.Message.ToString() + "Query:" + query);
                        sReturnMsg = Ex.Message.ToString() + "Query:" + query;
                    }
                }
            }

            if (sReturnMsg.Contains("Successfully") == true)
            {
                tm.ToolmarkID  = ID;
                return new JsonResult(ObjectToData(tm));
            }
            else
                return new JsonResult(null);

        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                    delete from dbo.Toolmark
                    where ToolmarkID = " + id + @" 
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

            return new JsonResult("Deleted Successfully");
        }


        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;

                var imagePath = _configuration.GetSection("ImagePath")["PathName"];

                var physicalPath = _env.ContentRootPath + "/" + imagePath + "/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    stream.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }


        // get single record by ID like users?id=1 traditional style 
        [HttpGet]
        [Route("GetToolmark/ToolmarkId={ToolmarkId}")]
        public JsonResult GetEmpByID(int ToolmarkId)
        {
            string query = @"
                    select ToolmarkID,CaseNumber,Category,Email,Note,
                    convert(varchar(10),DateOfCollected,120) as DateOfCollected
                    ,ImageFileName
                    from dbo.Toolmark
                    ";
            string wherequery = string.Empty;

            if (ToolmarkId > 0)
            {
                wherequery = @" WHERE ToolmarkID = '" + ToolmarkId.ToString() + "'";
                query = query + wherequery;
            }

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

            return new JsonResult(table);
        }


        // search function
        // like operate function  [HttpPost("{Name}")]
        [HttpPost("Searchby/CaseNumber={CaseNumber}")]
        public JsonResult SearchPeople(string CaseNumber)
        {
            string query = @"
                    select ToolmarkID,CaseNumber,Category,Email,Note,
                    convert(varchar(10),DateOfCollected,120) as DateOfCollected
                    ,ImageFileName
                    from dbo.Toolmark ";

            string WhereQuery = string.Empty;

            if (CaseNumber.Trim().Length > 0)
            {
                WhereQuery = @"
                    WHERE CaseNumber LIKE "
                    + "'" + CaseNumber.Trim() + "%'";
                query = query + WhereQuery;
            }

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

            return new JsonResult(table);

        }

        [Route("GetAllCategoryNames")]
        [HttpGet]
        public JsonResult GetAllCategoryNames()
        {
            string query = @"
                    select type from dbo.Category
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

            return new JsonResult(table);
        }


        // for adding or updating function, return value as table from the original - successfully 
        [ApiExplorerSettings(IgnoreApi = true)]
        public  DataTable ObjectToData(object o)
        {
            DataTable dt = new DataTable("OutputData");

            DataRow dr = dt.NewRow();


            o.GetType().GetProperties().ToList().ForEach(f =>
            {
                try
                {
                    f.GetValue(o, null);
                    dt.Columns.Add(f.Name, typeof(string));
                    dr[f.Name] = f.GetValue(o, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            dt.Rows.Add(dr);

            return dt;
        }

    }
}
