using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CSFileIOWithDBASPNETCore.Models;
using System.IO;
using System.Drawing;
using System.Data.SqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSFileIOWithDBASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            using (ImageDBContext dbContext = new ImageDBContext())
            {
                List<Guid> iamgeIds = dbContext.Images.Select(m => m.Id).ToList();
                return View(iamgeIds);
            }
        }

        [HttpGet]
        public IActionResult Portfolio()
        {
            using (ImageDBContext dbContext = new ImageDBContext())
            {
                List<Guid> iamgeIds = dbContext.Images.Select(m => m.Id).ToList();
                return View(iamgeIds);
            }
        }

        [HttpPost]
        public IActionResult UploadImage(IList<IFormFile> files)
        {
            IFormFile uploadedImage = files.FirstOrDefault();
           // if (uploadedImage == null || uploadedImage.ContentType.ToLower().StartsWith("image/"))
           // {
                using (ImageDBContext dbContext = new ImageDBContext())
                {
                    MemoryStream ms = new MemoryStream();
                    uploadedImage.OpenReadStream().CopyTo(ms);

                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                    Models.Image imageEntity = new Models.Image()
                    {
                        Id = Guid.NewGuid(),
                      //  Name = uploadedImage.Name,
                        Data = ms.ToArray(),
                       // Width = image.Width,
                       // Height = image.Height,
                        ContentType = uploadedImage.ContentType
                    };

                    dbContext.Images.Add(imageEntity);

                String _ConnectionString = @"Data Source=.;Initial Catalog=dbdb;Integrated Security=True;";
                //String _ConnectionString = @"Server=tcp:dina2decor.database.windows.net,1433;Initial Catalog=dina2decor;Persist Security Info=False;User ID=windersan;Password=Dina2decor!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                SqlConnection conn = new SqlConnection(_ConnectionString);
                conn.Open();

                string SqlInsert = "Insert into images (id, Data) " +
                    "values (@id, @imgdata);";

                SqlCommand insertcmd = new SqlCommand(SqlInsert, conn);
                Guid r = Guid.NewGuid();
                insertcmd.Parameters.AddWithValue("@id", r);
                insertcmd.Parameters.AddWithValue("@imgdata", ms.ToArray());
                //insertcmd.Parameters.AddWithValue("@h", image.Height);
                //insertcmd.Parameters.AddWithValue("@w", image.Width);
                //insertcmd.Parameters.AddWithValue("@c", uploadedImage.ContentType);
                //insertcmd.Parameters.AddWithValue("@l", uploadedImage.Length);
                //insertcmd.Parameters.AddWithValue("@n", "lol");




                //SqlCommand cmd = new SqlCommand("spGetPersonsByOrderDate", conn);
                // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@ImgData", ms);

                int numberOfRows = insertcmd.ExecuteNonQuery();
                // SqlDataReader reader = cmd.ExecuteReader();



                conn.Close();


//                string SqlInsert =
//"insert into customer (id, name) values ('" +
//customer.Id + "', '" + customer.Name + "')";
//                SqlCommand cmd = new SqlCommand(SqlInsert, conn);
//                int numberOfRows = cmd.ExecuteNonQuery();

                // dbContext.SaveChanges();
            }
           // }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public FileStreamResult ViewImage(Guid id)
        {
            using (ImageDBContext dbContext = new ImageDBContext())
            {
                 Models.Image image = dbContext.Images.FirstOrDefault(m => m.Id == id);
                String _ConnectionString = @"Data Source=.;Initial Catalog=dbdb;Integrated Security=True;";
                //String _ConnectionString = @"Server=tcp:dina2decor.database.windows.net,1433;Initial Catalog=dina2decor;Persist Security Info=False;User ID=windersan;Password=Dina2decor!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                SqlConnection conn = new SqlConnection(_ConnectionString);
                conn.Open();

                SqlDataReader reader = null;
                string SqlQuery = "select imgdata from image ";
                SqlCommand cmd = new SqlCommand(SqlQuery, conn);
               //////////// reader = cmd.ExecuteReader();
               /////// while (reader.Read())
                //{
                //    customer = new Customer();
                //    customer.Id =
                //    reader.GetInt32(reader.GetOrdinal("id"));
                //    customer.Name =
                //    reader.GetString(reader.GetOrdinal("name"));
                //}
               /////////// conn.Close();

                MemoryStream ms = new MemoryStream(image.Data);

                Models.Image im = new Models.Image();
                ///////MemoryStream ms = new MemoryStream();
                //return new FileStreamResult(ms, im.ContentType);
                return new FileStreamResult(ms, "image/jpeg");
               
            }
        }
    }
}
