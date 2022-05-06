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







            #region Retrieve Cats with DomainModels


            //cs = cs.Replace("NorthWind", "GadgetStore");
            //conn.ConnectionString = cs;
            //conn.Open();

            //cmd = new SqlCommand("SELECT CategoryId, CategoryName, CategoryDescription From Categories", conn);

            ////cmd.CommandText = "Query"

            //rdr = cmd.ExecuteReader();'
            //DAL dal = new DAL();
            //  List<CategoryDomainModel> categories = dal.GetCats();

            //Loop through the reader
            //while (rdr.Read())
            //{

            //    //we will create a CategoryDomainModel for every object in the categories table
            // CategoryDomainModel cat2 = new CategoryDomainModel()
            //    {
            // CategoryId = (int)rdr["CategoryId"],
            //CategoryId. CategoryName = (string)rdr["Name"],
            //CategoryDescription = (string)rdr["Description"] is DBNull ? "N/A" : (string)rdr["Description"]


            //   };
            //    categories.Add(cat2);
            //}
            //rdr.Close();
            //conn.Close();

            // foreach (var category in categories)
            //{
            //    Console.WriteLine($"{category.CategoryName}\n- {category.CategoryDescription}");
            //}
            // #endregion


            // Create a new class called ProductDomainModel

            //Make a property for each column in the table from SSMS

            //show all productname and price

            //retrieve and display ONE product/price by ID

            #endregion

            DAL dal2 = new DAL();
            List<ProductClassDomain> products = dal2.GetProducts();

            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductName}");
            }



            Console.WriteLine("\n\nPress any key to continue....\n");
            Console.ReadKey(true);
            Console.WriteLine("************************");






            Console.WriteLine(dal2.GetJsonProducts());












        }

    }







}

