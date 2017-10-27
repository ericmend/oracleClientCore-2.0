using System;
using System.Data;
using System.Data.OracleClient;
using Xunit;

namespace dotNetCore.Data.OracleClient.test
{
    /// <summary>
    /// https://docs.microsoft.com/pt-br/dotnet/core/testing/unit-testing-with-dotnet-test
    /// </summary>
    public class OracleClientCore
    {
        [Fact]
        public void InsertTable()
        {
            using (var connection = new OracleConnection("Data Source=XE;User ID=system;Password=oracle"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {

                    command.Transaction = connection.BeginTransaction();
                    

                    command.CommandText = "SELECT TABLE_NAME FROM ALL_TABLES WHERE TABLE_NAME = :p1 ORDER BY 1";
                    
                    OracleParameter myParameter1 = new OracleParameter(":p1", OracleType.VarChar);
		            myParameter1.Direction = ParameterDirection.Input;
                    myParameter1.Value = "WWV_FLOW_LIST_ITEMS";
		            command.Parameters.Add(myParameter1);
			
            
            // trans = con.BeginTransaction();
			// cmd2.Transaction = trans;
			// cmd2.CommandText = sql;
			
			// OracleParameter myParameter1 = new OracleParameter("p1", OracleType.VarChar, 32);
			// myParameter1.Direction = ParameterDirection.Input;
		
			// OracleParameter myParameter2 = new OracleParameter("p2", OracleType.Number);
			// myParameter2.Direction = ParameterDirection.Input;

			// myParameter2.Value = 182;
			// myParameter1.Value = "Mono";

			// cmd2.Parameters.Add (myParameter1);
            // cmd2.Parameters.Add (myParameter2);



                    

                    Console.WriteLine("Execute reader...");
                    using (var reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Tables:");

                        while (reader.Read())
                        {
                            string tableName = reader.GetString(reader.GetOrdinal("TABLE_NAME"));
                            Console.WriteLine(tableName);
                        };
                    }
                    command.Transaction.Rollback();
                }
            }
            Console.WriteLine("Done.");
            Assert.False(false, "1 should not be prime");
        }
    }
}
