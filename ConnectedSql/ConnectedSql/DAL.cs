using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConnectedSql
{
    internal class DAL
    {

        string cs = @"Data Source=.\sqlexpress;Initial Catalog=GadgetStore;Integrated Security=true;Encrypt=false";

        public List<CategoryDomainModel> GetCats()
        {

            SqlConnection conn = new SqlConnection(cs);
            List<CategoryDomainModel> categories = new();
            conn.Open();
            SqlCommand cmd = new("SELECT CategoryId, CategoryName, CategoryDescription From Categories", conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                CategoryDomainModel cat2 = new CategoryDomainModel()
                {
                    CategoryId = (int)rdr["CategoryId"],
                    CategoryName = (string)rdr["Name"],
                    CategoryDescription = (string)rdr["Description"] is DBNull ? "N/A" : (string)rdr["Description"]


                };
                categories.Add(cat2);
            }
            rdr.Close();
            conn.Close();
            return categories;

        }
        public CategoryDomainModel GetCatById(int catId)
        {
            SqlConnection conn = new SqlConnection(cs)

            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Categories WHERE CategoryId = @CategoryId", conn);
            cmd.Parameters.AddWithValue("@CategoryId", catId);

            SqlDataReader rdr = cmd.ExecuteReader();

            rdr.Read();
            CategoryDomainModel cat = new()
            {
                CategoryId = (int)rdr["CategoryId"],
                CategoryName = (string)rdr["CategoryName"],
                CategoryDescription = rdr["CategoryDescription"] is DBNull ? "N/A" : (string)rdr["CategoryDescription"]
            };
            rdr.Close();
            conn.Close();
            return cat;
        }


        public List<ProductClassDomain> GetProducts()
        {

            SqlConnection conn = new SqlConnection(cs);
            List<ProductClassDomain> products = new();
            conn.Open();
            SqlCommand cmd = new("SELECT ProductId, ProductName, ProductPrice, ProductDescription, UnitsInStock, UnitsOnOrder, IsDiscontinued, CategoryName, SupplierName, ProductImage FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.CategoryId LEFT JOIN Suppliers s ON p.SupplierId = s.SupplierId", conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ProductClassDomain prod = new ProductClassDomain()
                {
                    ProductId = (int)rdr["ProductId"],
                    ProductName = (string)rdr["ProductName"],
                    ProductPrice = (decimal)rdr["ProductPrice"],
                    ProductDescription = (string)rdr["ProductDescription"] is DBNull ? null : (string)rdr["ProductDescription"],
                    UnitsInStock = (short)rdr["UnitsInStock"],
                    UnitsOnOrder = (short)rdr["UnitsOnOrder"],
                    IsDiscontinued = (bool)rdr["IsDiscontinued"]




                };
                products.Add(prod);
            }
            rdr.Close ();
            conn.Close();
            return products;

           

        }


        public string GetJsonProducts()
        {
            string json = string.Empty;

            //retrieve a list of products from the db
            var products = GetProducts();

            //convert each Product object to JSON
            foreach (var product in products)
            {
                json += JsonSerializer.Serialize(product) + "\n\n";
            }
            //Print JSON results to a file
            File.WriteAllText("../../../test.json", json);


            return json;
        }


        

    }

}


