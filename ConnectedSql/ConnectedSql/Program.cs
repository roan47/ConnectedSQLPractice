using ConnectedSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedSq1
{
    class Program
    {
        public static object SqlConnection { get; private set; }

        static void Main(string[] args)
        {

            #region Connected SQL Notes
            //Connected SQL is a part of ADO.NET (Active DataX Objects -> Microsoft's solution to incorporating data
            //into applications).  
            //With this implementation, we typically retrieve items from the database and convert those to 
            //C# objects.  Once that is done, we can display them.

            /*
             *  In order to retrieve data from the database we must define the connection string
             *  (roadmap the database)
             *  ConnectionString 3 basic parts: Data Source (SERVER), Initial Catalog (Db Name)
             *  Integrated Security (true/false/sspi). True indicates Windows (integrated) authentication.
             *  False indicates Sql Server authentication which requires a user name and password combo.
             *  SSPI basically works the same as false, but it rarely used.
             *  
             *  SqlConnection Object.  The connection object requires a connection string as a parameter
             *  to build it.  To acces the data, call SqlConnection.Open() then articulate our query.
             *  
             *  SqlCommand Object.  The command requires a minimum of CommandText (sql query as a string)
             *  and a SqlConnection object, to initialize the request.
             *  If a parameterized query is written, you can use the Paramters property and the 
             *  AddWithValue() to specify both the parameter AND its value.
             *  Available Methods (SqlCommand)
             *  ExecuteReader() - SELECT statements - ReturnType is SqlDataReader
             *  ExecuteNonQuery() - INSERT, UPDATE, DELETE statments - ReturnType is an int.
             *                    - Usually used to show RowsAffected by the statement
             *  ExecuteScalar()   - Aggragate Functions: SUM, AVG, COUNT, etc. - ReturnType Object
             *  
             *  SqlDataReader - This object holds the results of the command object's ExecuteReader().
             *  The reader will need to be looped through (if more than result is desired) or branched
             *  (if or switch) if a single result is required.  To call ExecuteReader() the connection
             *  object's Open() must have been called.  When the reader is done, you should call
             *  SqlDataReader.Close().  You should also close the connection as soon as possible.
             *  (SqlConnection.Close()).
             */
            #endregion


            #region Set up DB Connection
            //Connection string needed by the console app to connect to the db
            string cs = @"Data Source=.\sqlexpress;Initial Catalog=GadgetStore;Integrated Security=true";

            SqlConnection conn = new SqlConnection(cs);


            #endregion

            #region Example -> Retrieve a signle category
            Console.WriteLine("*** Example of retrieving a single Category from the DB to display. **\n--------------------------------\n");

            //Open the gates, let the data flow!
            conn.Open();

            //The command object is the text for your SQL query
            //NOTE: You MUST have two args with the command object -> QueryText, Connection object (conn)
            SqlCommand cmd = new SqlCommand("SELECT TOP 1 c.CategoryID, c.CategoryName, c.CategoryDescription FROM Categories c", conn);

            //The Reader object allows us to peruse the data that we retrieve from the db
            SqlDataReader rdr = cmd.ExecuteReader();

            //output string 
            string categoriesStr = "";
            //Make the rdr read the object to us. This essentially puts the record in memory.


            rdr.Read();

            //After reading, grab the specific column values we want to display and concat those to our output

            categoriesStr += $"{rdr["CategoryName"]}\n- {rdr["CategoryDescription"]}\n\n";

            //When finished querying, you MUST close the reader AND the connection.
            //Failure to do so WILL result in catasrophe on your machine.
            conn.Close();
            rdr.Close();

            Console.WriteLine(categoriesStr);
            #endregion



            Console.WriteLine("\n\nPress any key to continue....\n");
            Console.ReadKey(true);
            Console.WriteLine("************************");

            #region Retrieve all categories
            Console.WriteLine("\n\n** Example of retrieving all Categories from the DB to display.**" +
                "\n\n***************************\n");




            conn.Open();

            cmd.CommandText = "SELECT CategoryName, CategoryDescription From Categories";

            rdr = cmd.ExecuteReader();

            //empty old string
            categoriesStr = "";


            while (rdr.Read())
            {
                categoriesStr += $"{rdr["CategoryName"]}\n- {rdr["CategoryDescription"]}\n\n";
            }
            rdr.Close();
            conn.Close();

            Console.WriteLine(categoriesStr);

            #endregion





            Console.WriteLine("\n\nPress any key to continue....\n");
            Console.ReadKey(true);
            Console.WriteLine("************************");

            #region Retrieve all results from vwGadgetOverview

            conn.Open();

            cmd.CommandText = "Select * FROM newview;";

            rdr = cmd.ExecuteReader();

            //empty old string 

            categoriesStr = "";


            while (rdr.Read())
            {
                categoriesStr += $"{rdr["ProductName"]}\n- {rdr["ProductPrice"]}\n\n";
            }
            rdr.Close();
            conn.Close();

            Console.WriteLine(categoriesStr);

            #endregion

            Console.ReadKey(true);
            Console.Clear();



            string ex = @"Data Source=.\sqlexpress;Initial Catalog=Northwind;Integrated Security=true";

            SqlConnection north = new SqlConnection(ex);

            north.Open();

            SqlCommand nwd = new SqlCommand("SELECT TOP 1 c.CategoryID, c.CategoryName, c.Description FROM Categories c", north);

            SqlDataReader cat = nwd.ExecuteReader();

            string newCats = "";

            cat.Read();


            newCats += $"{cat["CategoryName"]}\n- {cat["Description"]}\n\n";


            north.Close();
            cat.Close();


            Console.WriteLine("\n\n** Example of retrieving all Categories from the DB to display.**" +
            "\n\n***************************\n");




            north.Open();

            nwd.CommandText = "SELECT CategoryName,Description From Categories";

            cat = nwd.ExecuteReader();

            //empty old string
            newCats = "";



            while (cat.Read())
            {
                newCats += $"{cat["CategoryName"]}\n- {cat["Description"]}\n\n";
            }
            cat.Close();
            north.Close();

            Console.WriteLine(newCats);

            Console.ReadKey(true);



            north.Open();

            nwd.CommandText = "SELECT ProductName from Products";

            cat = nwd.ExecuteReader();

            //empty old string
            newCats = "";



            while (cat.Read())
            {
                newCats += $"{cat["ProductName"]}\n-";
            }

            cat.Close();
            north.Close();



            Console.WriteLine(newCats);




            Console.ReadKey(true);



            north.Open();

            nwd.CommandText = "SELECT p.ProductName, C.CategoryName from Categories c Join Products p on c.CategoryID = p.CategoryID";

            cat = nwd.ExecuteReader();

            //empty old string
            newCats = "";



            while (cat.Read())
            {
                newCats += $"{cat["ProductName"]}\n- {cat["CategoryName"]}\n";
            }

            cat.Close();
            north.Close();



            Console.WriteLine(newCats);



            Console.ReadKey(true);





            #region Block 4

            #region Retrieve Cats with DomainModels
            Console.WriteLine("\n\n*** Example of retrieving all Categories from the DB with Domain Models.***" +
                "\n\n**************************\n");

            DAL dal = new();
            List<CategoryDomainModel> categories = dal.GetCats();

            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryId}) {category.CategoryName}\n- {category.CategoryDescription}\n");
            }

            Console.WriteLine("*** Retrive Category by ID (1)***");
            CategoryDomainModel YUP = dal.GetCatById(1);
            Console.WriteLine($"{YUP.CategoryId}) {YUP.CategoryName}\n- {YUP.CategoryDescription}");
            #endregion
            Console.Write("\n\nPress any key to continue...\n");
            Console.ReadKey(true);
            Console.Clear();


            #region Retrieve Products with DomainModels - LAB

            //Create a new class called ProductDomainModel
            //Make a property for each column in the table from SSMS

            //show all productname and price
            Console.WriteLine("*** Products from Gadget Store ***\n");

            List<ProductDomainModel> products = dal.GetProducts();

            products.ForEach(x => Console.WriteLine($"{x.ProductId}) {x.ProductName} - {x.ProductPrice:c}"));
            //retrieve and display ONE product/price by ID
            Console.Write("\n\nPress any key to continue...\n");
            Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("GetProdById - ProductId = 1");

            ProductDomainModel prod = dal.GetProdById(1);
            Console.WriteLine($"Product ID {prod.ProductId}");
            Console.WriteLine($"{prod.ProductName} - {prod.ProductPrice:c}");
            #endregion
            Console.Write("\n\nPress any key to continue...\n");
            Console.ReadKey(true);
            Console.Clear();

            #region Show a list of products broken out by category
            Console.WriteLine("List of all Products grouped by Category\n-----------------------------------");

            foreach (var c in categories)
            {
                Console.WriteLine($"{c.CategoryId}) {c.CategoryName} - {c.CategoryDescription}" +
                    $"\n-----------------------------------");
                foreach (var p in products.Where(x => x.CategoryId == c.CategoryId))
                {
                    Console.WriteLine($"\t{p.ProductName} - {p.ProductPrice:c}");
                }
                Console.WriteLine($"\n-----------------------------------");
            }//end outer foreach
            #endregion
            Console.Write("\n\nPress any key to continue...\n");
            Console.ReadKey(true);
            Console.Clear();

            #region JSON

            Console.WriteLine("JSON Product results\n-----------------------------------\n");

            Console.WriteLine(dal.GetJsonProducts());
            #endregion

            #endregion


            #region Block 5


            // Console.ReadKey(true);
            // Console.Clear();
            //dal.CreateCategory();


            // Console.ReadKey(true);
            // Console.Clear();
            // dal.EditCategory();



            // Console.ReadKey(true);
            // Console.Clear();
            // dal.DeleteCategory();


            // Console.ReadKey(true);
            // Console.Clear();
            // dal.CreateProduct();


            Console.ReadKey(true);
            Console.Clear();
            dal.DeleteProduct();













            foreach (ProductDomainModel c in dal.GetProducts())
            {
                Console.WriteLine($"Id:{c.ProductId}\n" +
                    $"Name: {c.ProductName} - {c.ProductDescription} - {c.ProductPrice} - {c.UnitsInStock} - {c.UnitsOnOrder} - {c.IsDiscontinued}\n\n");
            }










            #endregion
                foreach (CategoryDomainModel c in dal.GetCats())
            {
                Console.WriteLine($"Id:{c.CategoryId}\n" +
                    $"Name: {c.CategoryName} - {c.CategoryDescription}\n\n");
            }





        }

    }







}

