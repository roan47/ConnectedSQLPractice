using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConnectedSql
{
    /// <summary>
    /// DAL -> Data Access Layer
    /// </summary>
    internal class DAL
    {
        //props -- Extracted SQL and connections to be properties
        static readonly string cs = @"Data Source=.\sqlexpress;Initial Catalog=GadgetStore;Integrated Security=true;Encrypt=false";
        
        readonly SqlConnection conn = new(cs);
        readonly string catSql = "SELECT * FROM Categories";
        readonly string prodSql = "SELECT * FROM Products;";

        //methods
        public List<CategoryDomainModel> GetCats()
        {

            List<CategoryDomainModel> categories = new();

            conn.Open();

            SqlCommand cmd = new(catSql, conn);

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                CategoryDomainModel cat = new CategoryDomainModel()
                {
                    CategoryId = (int)rdr["CategoryId"],
                    CategoryName = (string)rdr["CategoryName"],
                    CategoryDescription = rdr["CategoryDescription"] is DBNull ? "N/A" : (string)rdr["CategoryDescription"]
                };
                categories.Add(cat);
            }//end while
            rdr.Close();
            conn.Close();
            return categories;
        }

        public CategoryDomainModel GetCatById(int catId)
        {            
            conn.Open();
            SqlCommand cmd = new SqlCommand(catSql + " WHERE CategoryId = @CategoryId", conn);
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

        //Get prods In-Class Lab
        public List<ProductDomainModel> GetProducts()
        {
            List<ProductDomainModel> products = new();
            conn.Open();
            var rdr = new SqlCommand(prodSql, conn).ExecuteReader();
            while (rdr.Read())
            {
                ProductDomainModel product = new ProductDomainModel()
                {
                    ProductId = (int)rdr["ProductId"],
                    ProductName = (string)rdr["ProductName"],
                    ProductPrice = (decimal)rdr["ProductPrice"],
                    ProductDescription = rdr["ProductDescription"] is DBNull ? "N/A" : (string)rdr["ProductDescription"],
                    UnitsInStock = (short)rdr["UnitsInStock"],
                    UnitsOnOrder = (short)rdr["UnitsOnOrder"],
                    IsDiscontinued = (bool)rdr["IsDiscontinued"],
                    CategoryId= (int)rdr["CategoryId"],
                    SupplierId = rdr["SupplierId"] is DBNull ? 0: (int)rdr["SupplierId"],
                    ProductImage = rdr["ProductImage"] is DBNull ? "N/A" : (string)rdr["ProductImage"]
                };
                products.Add(product);
            }
            rdr.Close();
            conn.Close();
            return products;
        }

        //Get prods by ID In Class Lab
        public ProductDomainModel GetProdById(int prodId)
        {
            var result = GetProducts().First(x => x.ProductId == prodId);
            return result;
        }
        
        public string GetJsonProducts()
        {
            string json = string.Empty;

            //retrieve a list of products from the db
            List<ProductDomainModel> products = GetProducts();

            //convert each Product object to JSON
            foreach (var product in products)
            {
                json += JsonSerializer.Serialize<ProductDomainModel>(product) + "\n\n";
            }
            //Print JSON results to a file
            File.WriteAllText("../../../test.json", json);

            return json;
        }



        #region Cat CRUD

        public void CreateCategory()
        {
            //1) Create variables needed for category properties
            string catName = "";
            string catDescription = "";

            //2) Allow user to enter name and description 
            Console.WriteLine("**** Create Category *******\n\n");
            Console.WriteLine("Enter the Category Name");

            catName = Console.ReadLine();
            Console.WriteLine($"Enter a Description for {catName}");
            catDescription = Console.ReadLine();    

            //3) Use cinnected SQL to insert
            using (SqlConnection conn = new SqlConnection(cs))
            {
                //Open the operation
                conn.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("INSERT INTO Categories (CategoryName, CategoryDescription) Values (@CategoryName, @CategoryDescription);", conn);

                //Update the command with user arguments
                cmd.Parameters.AddWithValue("@CategoryName", catName);
                cmd.Parameters.AddWithValue("@CategoryDescription", catDescription);

                //executes the command
                cmd.ExecuteNonQuery();//no rdr since we are only inserting and not retrieving 
            }


        }

        public void EditCategory()
        {
            Console.WriteLine("******* EDIT A CATEGORY *******\n\n");
            Console.WriteLine("Please choose a Category to modify.");

            var categories = GetCats();

            foreach (var item in categories)
            {
                Console.WriteLine($"{item.CategoryId}{item.CategoryName} - {item.CategoryDescription}\n");

            }

            Console.WriteLine("\nEnter the CategoryId you'd like to edit: ");
            int catId = int.Parse(Console.ReadLine());
            Console.WriteLine("------------------------------------------");
            Console.Write("New Category Name: ");
            string categoryName = Console.ReadLine();
            Console.Write("New Category Description:  ");
            string  categoryDescription = Console.ReadLine();   

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                SqlCommand cmd = new("UPDATE Categories SET CategoryName = @CategoryName, CategoryDescription = @CategoryDescription WHERE CategoryId = @CategoryId", conn);

                cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                cmd.Parameters.AddWithValue("@CategoryDescription", categoryDescription);
                cmd.Parameters.AddWithValue("@CategoryId", catId);



                cmd.ExecuteNonQuery();
            }
        }


        public void DeleteCategory()
        {
            Console.WriteLine("******* Delete A CATEGORY *******\n\n");
            Console.WriteLine("Please choose a Category to delete.");

         List<CategoryDomainModel> categories = GetCats();

           
        categories.ForEach(item =>Console.WriteLine($"{item.CategoryId}{item.CategoryName} - {item.CategoryDescription}\n"));

            Console.WriteLine("\nEnter the CategoryId you'd like to Delete: ");
            string catId = Console.ReadLine();

            using (SqlConnection conn = new(cs))
            {
                conn.Open();
                SqlCommand cmd = new("DELETE FROM Categories WHERE CategoryId = @CategoryId", conn);

                cmd.Parameters.AddWithValue("@CategoryId", catId);
                cmd.ExecuteNonQuery ();

            }


            
        }
        #endregion


        #region Prod Crud



        public void CreateProduct()
        {
            //1) Create variables needed for category properties
            string prodName = "";
            string prodDescription = "";
           

            //2) Allow user to enter name and description 
            Console.WriteLine("**** Create Product *******\n\n");
            Console.WriteLine("Enter the Product Name");

            prodName = Console.ReadLine();
            Console.WriteLine($"Enter a Description for {prodDescription}");
            prodDescription = Console.ReadLine();
            Console.WriteLine("enter Product price");
          decimal prodPrice = decimal.Parse(Console.ReadLine());
            Console.WriteLine("enter units in stock and units out stock");
            int unitsInStock = int.Parse(Console.ReadLine());
            int unitsOnOrder = int.Parse(Console.ReadLine());
            Console.WriteLine("is discontinued?");
            bool isDiscontinued = bool.Parse(Console.ReadLine());
            Console.WriteLine("Category Id");
            int catId = int.Parse(Console.ReadLine());
            



            //3) Use cinnected SQL to insert
            using (SqlConnection conn = new SqlConnection(cs))
            {
                //Open the operation
                conn.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("insert into Products (ProductName, ProductDescription, ProductPrice, UnitsInStock, UnitsOnOrder, IsDiscontinued, CategoryId) values (@ProductName, @ProductDescription, @ProductPrice, @UnitsInStock, @UnitsOnOrder, @IsDiscontinued, @CategoryId)", conn);


                //Update the command with user arguments
                cmd.Parameters.AddWithValue("@ProductName", prodName);
                cmd.Parameters.AddWithValue("@ProductDescription", prodDescription);
                cmd.Parameters.AddWithValue("@ProductPrice", prodPrice);
                cmd.Parameters.AddWithValue("@UnitsInStock", unitsInStock);
                cmd.Parameters.AddWithValue("@UnitsOnOrder", unitsOnOrder);
                cmd.Parameters.AddWithValue("@IsDiscontinued", isDiscontinued);
                cmd.Parameters.AddWithValue("@CategoryId", catId);




                //executes the command
                cmd.ExecuteNonQuery();//no rdr since we are only inserting and not retrieving 
            }





            #endregion


        }



        public void EditProduct()
        {
            Console.WriteLine("******* EDIT A Products *******\n\n");
            Console.WriteLine("Please choose a Product to modify.");

            var products = GetProducts();

            foreach (var item in products)
            {
                Console.WriteLine($"{item.ProductId}{item.ProductName} - {item.ProductDescription}" +
                    $"{item.ProductPrice} - {item.UnitsInStock} - {item.UnitsOnOrder}" +
                    $"{item.IsDiscontinued} - {item.CategoryId}");

            }


            Console.WriteLine("\nEnter the ProductIDyou'd like to edit: ");
                int prodID = int.Parse(Console.ReadLine());


            Console.WriteLine("Enter the Product Name");

  string    prodName = Console.ReadLine();
            Console.WriteLine($"Enter a Description");
   string   prodDescription = Console.ReadLine();
            Console.WriteLine("enter Product price");
          decimal prodPrice = decimal.Parse(Console.ReadLine());
            Console.WriteLine("enter units in stock and units out stock");
            int unitsInStock = int.Parse(Console.ReadLine());
            int unitsOnOrder = int.Parse(Console.ReadLine());
            Console.WriteLine("is discontinued?");
            bool isDiscontinued = bool.Parse(Console.ReadLine());
            Console.WriteLine("Category Id");
            int catId = int.Parse(Console.ReadLine());


            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                SqlCommand cmd = new("update Products set ProductName = @ProductName, ProductDescription = @ProductDescription,ProductPrice = @ProductPrice,UnitsInStock = @UnitsInStock,UnitsOnOrder = @UnitsOnOrder,IsDiscontinued = @IsDiscontinued,CategoryId = @CategoryId Where ProductId = @ProductId", conn);

                cmd.Parameters.AddWithValue("@ProductName", prodName);
                cmd.Parameters.AddWithValue("@ProductDescription", prodDescription);
                cmd.Parameters.AddWithValue("@ProductPrice", prodPrice);
                cmd.Parameters.AddWithValue("@UnitsInStock", unitsInStock);
                cmd.Parameters.AddWithValue("@UnitsOnOrder", unitsOnOrder);
                cmd.Parameters.AddWithValue("@IsDiscontinued", isDiscontinued);
                cmd.Parameters.AddWithValue("@CategoryId", catId);
                cmd.Parameters.AddWithValue("@ProductId", prodID);





                //executes the command
                cmd.ExecuteNonQuery();//no rdr since we are only inserting and not ret
            }


        }




        public void DeleteProduct()
        {
            Console.WriteLine("******* Delete A Product  *******\n\n");
            Console.WriteLine("Please choose a Products to delete.");

            List<ProductDomainModel> products = GetProducts();


            products.ForEach(item => Console.WriteLine($"{item.ProductId}{item.ProductName} - {item.ProductDescription}" +
                    $"{item.ProductPrice} - {item.UnitsInStock} - {item.UnitsOnOrder}" +
                    $"{item.IsDiscontinued} - {item.CategoryId}"));

            Console.WriteLine("\nEnter the ProductId you'd like to Delete: ");
            string catId = Console.ReadLine();

            using (SqlConnection conn = new(cs))
            {
                conn.Open();
                SqlCommand cmd = new("DELETE FROM Products WHERE ProductId = @ProductId", conn);

                cmd.Parameters.AddWithValue("@ProductId", catId);
                cmd.ExecuteNonQuery();


            }






        }



    }

}

