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
        
  
        

        static void Main(string[] args)

        {
            string cs = @"Data Source=.\sqlexpress;Initial Catalog=StoreFront;Integrated Security=true";

            SqlConnection conn = new SqlConnection(cs);

            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT p.Pname, C.CategoryName from Categories c Join Products p on c.CategoryID = p.ProductID Group by p.Pname, c.CategoryName", conn);

            SqlDataReader rdr = cmd.ExecuteReader();


            string categoriesStr = "";

            while (rdr.Read())
            {
                categoriesStr += $"{rdr["Pname"]}\n-{rdr["CategoryName"]}\n";
            }

            conn.Close();
            rdr.Close();

            Console.WriteLine(categoriesStr);












        }



    
    }
}