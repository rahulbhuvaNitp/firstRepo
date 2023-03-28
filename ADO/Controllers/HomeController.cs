using ADO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace ADO.Controllers
{
    public class HomeController : Controller
    {

		SqlConnection con = new SqlConnection("Data Source=PUIN-LT149;Initial Catalog=Company;Integrated Security=True");
		SqlCommand com = new SqlCommand();
		SqlDataReader dr;
		public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {


            List<Inventory> inventoryList = new List<Inventory>();

            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "Select * From Inventory";
            

                SqlCommand command = new SqlCommand(sql, connection);
                

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Inventory inventory = new Inventory();
                        inventory.Id = Convert.ToInt32(dataReader["Id"]);
                        inventory.Name = Convert.ToString(dataReader["Name"]);
                        inventory.Price = Convert.ToDecimal(dataReader["Price"]);
                        inventory.Quantity = Convert.ToInt32(dataReader["Quantity"]);
                        inventory.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);

                        inventoryList.Add(inventory);
                    }
                }

                connection.Close();
            }
            return View(inventoryList);
        }


        public IActionResult Create()
        {
            return View();
        }

		[HttpPost]
        public IActionResult Create(Inventory invent)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Insert Into Inventory (Name, Price, Quantity) Values ('{invent.Name}', '{invent.Price}','{invent.Quantity}')";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            ViewBag.Result = "Success";
            return RedirectToAction("Index","Home");
        }


		[HttpGet]
		public JsonResult FindRecord(int id)
		{
			con.Open();
			com.Connection = con;
			com.CommandText = "Select Id,Name,Price,Quantity from Inventory where Id=" + id;
			dr = com.ExecuteReader();

			var inv = new Inventory();

			while (dr.Read())
			{
				inv.Id = dr.GetInt32(0);
				inv.Name = dr.GetString(1);
				inv.Price = dr.GetDecimal(2);
				inv.Quantity = dr.GetInt32(3);
			}

			return Json(inv);
		}

		public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            Inventory inventory = new Inventory();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From Inventory Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        inventory.Id = Convert.ToInt32(dataReader["Id"]);
                        inventory.Name = Convert.ToString(dataReader["Name"]);
                        inventory.Price = Convert.ToDecimal(dataReader["Price"]);
                        inventory.Quantity = Convert.ToInt32(dataReader["Quantity"]);
                        inventory.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);
                    }
                }

                connection.Close();
            }
            return View(inventory);
        }

        [HttpPost]
        public IActionResult Update(Inventory inventory, int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Update Inventory SET Name='{inventory.Name}', Price='{inventory.Price}', Quantity='{inventory.Quantity}' Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From Inventory Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }


        
     
        
    }
}