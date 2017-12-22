using System;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace dotNetCore.Data.OracleClient.test
{
    /// <summary>
    /// https://docs.microsoft.com/pt-br/dotnet/core/testing/unit-testing-with-dotnet-test
	/// usando os testes existente no projeto mono.
    /// </summary>
    public class OracleClientCore
    {
		private const string dataSource = "XE";
		private const string userID = "SYSTEM";
		private const string password = "oracle";

		private string connectionString;

		public OracleClientCore()
		{
			connectionString = string.Format("Data Source={0};User ID={1};Password={2}", dataSource, userID, password);
		}

		private void OnStateChange(object sender, StateChangeEventArgs e) 
		{
			Console.WriteLine("StateChange CurrentSate:" + e.CurrentState.ToString());
			Console.WriteLine("StateChange OriginalState:" + e.OriginalState.ToString());
		}
		
		private void OnInfoMessage(object sender, OracleInfoMessageEventArgs e) 
		{
			Console.WriteLine("InfoMessage Message: " + e.Message.ToString());
			Console.WriteLine("InfoMessage Code: " + e.Code.ToString());
			Console.WriteLine("InfoMessage Source: " + e.Source.ToString());
		}

		// use this function to read a byte array into a string
		// for easy display of binary data, such as, a BLOB value
		private string GetHexString(byte[] bytes)
		{ 			
			string bvalue = "";
			
			StringBuilder sb2 = new StringBuilder();
			for(int z = 0; z < bytes.Length; z++){
				byte byt = bytes[z];
				if(byt < 0x10)
					sb2.Append("0");
				sb2.Append(byt.ToString("x"));
			}
			if(sb2.Length > 0)
				bvalue = "0x" + sb2.ToString();
	
			return bvalue;
		}

		private byte[] ByteArrayCombine(byte[] b1, byte[] b2) 
		{
			if(b1 == null)
				b1 = new byte[0];
			if(b2 == null)
				b2 = new byte[0];
		
			byte[] bytes = new byte[b1.Length + b2.Length];
			int i = 0;
			for(int j = 0; j < b1.Length; j++){
				bytes[i] = b1[j];
				i++;
			}
			for(int k = 0; k < b2.Length; k++){
				bytes[i] = b2[k];
				i++;
			}
			return bytes;
		}

		private bool ByteArrayCompare(byte[] ba1, byte[] ba2)
		{
		    if(ba1 == null && ba2 == null)
		        return true;

		    if(ba1 == null)
		        return false;

		    if(ba2 == null)
		        return false;

		    if(ba1.Length != ba2.Length)
		        return false;

		    for(int i = 0; i < ba1.Length; i++)
		    {
		        if(ba1[i] != ba2[i])
		            return false;
		    }

		    return true;
		}

		private OracleLob GetOracleLob(OracleTransaction transaction, byte[] blob)
		{
		    string BLOB_CREATE = "DECLARE dpBlob BLOB; "	
		    + "BEGIN "
		    + "   DBMS_LOB.CREATETEMPORARY(dpBlob , False, 0); " 
		    + "  :tempBlob := dpBlob; "
		    + "END;";

		    OracleLob tempLob = OracleLob.Null;
		    if(blob != null)
		    {
		        // Create a new command using the same connection
		        OracleCommand command = transaction.Connection.CreateCommand();

		        // Assign the transaction to the command
		        command.Transaction = transaction;

		        // Create blob storage on the Oracle server
		        command.CommandText = BLOB_CREATE;

		        // Add a new output paramter to accept the blob storage	reference
                OracleParameter parm = new OracleParameter(":tempBlob", OracleType.Blob, blob.Length);
                parm.Direction = ParameterDirection.Output;
                command.Parameters.Add(parm);

                // Fire as your guns bear...
                command.ExecuteNonQuery();
				
                // Retrieve the blob stream from the OracleLob parameter 
                tempLob = (OracleLob)command.Parameters[0].Value; 

		        // Prevent server side events from firing while we write to the	stream
		        tempLob.BeginBatch(OracleLobOpenMode.ReadWrite);

		        // Write bytes to the stream
		        tempLob.Write(blob, 0, blob.Length);
			
		        // Resume firing server events
		        tempLob.EndBatch();
		    }

		    return tempLob;
		}
		private void SetupMyPackage(OracleConnection con) 
		{
			try {
				OracleCommand cmd2 = con.CreateCommand();
				cmd2.CommandText = "DROP TABLE BLOBTEST2";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}

			OracleCommand create = con.CreateCommand();
			create.CommandText = "CREATE TABLE BLOBTEST2(BLOB_COLUMN BLOB)";
			create.ExecuteNonQuery();

			create.CommandText = "commit";
			create.ExecuteNonQuery();

			OracleCommand cmd = con.CreateCommand();
			cmd.CommandText = 
				"CREATE OR REPLACE PACKAGE MyPackage AS\n" +
				" Procedure InsertBlob(i_Sig_File blob);\n" +
				"END MyPackage;";
			cmd.ExecuteNonQuery();

			cmd.CommandText = 
				"CREATE OR REPLACE PACKAGE BODY MyPackage AS\n" +
				"   Procedure InsertBlob(i_Sig_File blob)\n" +
				"   IS\n" +
				"   BEGIN\n" +
				"	INSERT INTO BLOBTEST2(BLOB_COLUMN) VALUES(i_Sig_File); " +
				"   END InsertBlob; " +
				"END MyPackage;";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "commit";
			cmd.ExecuteNonQuery();
		}

		private void SetupEMP(OracleConnection con) 
		{
			try {
				OracleCommand cmd2 = con.CreateCommand();
				cmd2.CommandText = "DROP TABLE emp";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}
			try {
				OracleCommand cmd3 = con.CreateCommand();
				cmd3.CommandText = "DROP TABLE dept";
				cmd3.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}

			OracleCommand create = con.CreateCommand();
			create.CommandText = "create table dept(deptno number(2,0), dname varchar2(14), loc varchar2(13), constraint pk_dept primary key(deptno))";
			create.ExecuteNonQuery();

			create.CommandText = "create table emp(empno number(4,0), ename varchar2(10), job varchar2(9), mgr number(4,0), hiredate date, sal number(7,2), " +
  								 "comm number(7,2), deptno number(2,0), constraint pk_emp primary key(empno), constraint fk_deptno foreign key(deptno) references dept(deptno))";
			create.ExecuteNonQuery();

			create.CommandText = "commit";
			create.ExecuteNonQuery();

			OracleCommand cmdInsert = con.CreateCommand();
			cmdInsert.CommandText = "insert into DEPT(DEPTNO, DNAME, LOC) values(10, 'ACCOUNTING', 'NEW YORK')";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into dept values(20, 'RESEARCH', 'DALLAS')";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into dept values(30, 'SALES', 'CHICAGO')";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into dept values(40, 'OPERATIONS', 'BOSTON')";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7839, 'KING', 'PRESIDENT', null, to_date('17-11-1981','dd-mm-yyyy'), 5000, null, 10)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7698, 'BLAKE', 'MANAGER', 7839, to_date('1-5-1981','dd-mm-yyyy'), 2850, null, 30)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7782, 'CLARK', 'MANAGER', 7839, to_date('9-6-1981','dd-mm-yyyy'), 2450, null, 10)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7566, 'JONES', 'MANAGER', 7839, to_date('2-4-1981','dd-mm-yyyy'), 2975, null, 20)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7788, 'SCOTT', 'ANALYST', 7566, to_date('13-JUL-87','dd-mm-rr') - 85, 3000, null, 20)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7902, 'FORD', 'ANALYST', 7566, to_date('3-12-1981','dd-mm-yyyy'), 3000, null, 20)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7369, 'SMITH', 'CLERK', 7902, to_date('17-12-1980','dd-mm-yyyy'), 800, null, 20)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7499, 'ALLEN', 'SALESMAN', 7698, to_date('20-2-1981','dd-mm-yyyy'), 1600, 300, 30)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7521, 'WARD', 'SALESMAN', 7698, to_date('22-2-1981','dd-mm-yyyy'), 1250, 500, 30)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7654, 'MARTIN', 'SALESMAN', 7698, to_date('28-9-1981','dd-mm-yyyy'), 1250, 1400, 30)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7844, 'TURNER', 'SALESMAN', 7698, to_date('8-9-1981','dd-mm-yyyy'), 1500, 0, 30)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7876, 'ADAMS', 'CLERK', 7788, to_date('13-JUL-87', 'dd-mm-rr') - 51, 1100, null, 20)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7900, 'JAMES', 'CLERK', 7698, to_date('3-12-1981','dd-mm-yyyy'), 950, null, 30)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "insert into emp values(7934, 'MILLER', 'CLERK', 7782, to_date('23-1-1982','dd-mm-yyyy'), 1300, null, 10)";
			cmdInsert.ExecuteNonQuery();

			cmdInsert.CommandText = "commit";
			cmdInsert.ExecuteNonQuery();
		}

		private bool ReadSimpleTest(OracleConnection con, string selectSql) 
		{
			OracleCommand cmd = null;
			OracleDataReader reader = null;
		
			cmd = con.CreateCommand();
			cmd.CommandText = selectSql;
			reader = cmd.ExecuteReader();
		
			Console.WriteLine("  Results...");
			Console.WriteLine("    Schema");
			DataTable table;
			table = reader.GetSchemaTable();
			for(int c = 0; c < reader.FieldCount; c++){
				Console.WriteLine("  Column " + c.ToString());
				DataRow row = table.Rows[c];
			
				string strColumnName = row["ColumnName"].ToString();
				string strBaseColumnName = row["BaseColumnName"].ToString();
				string strColumnSize = row["ColumnSize"].ToString();
				string strNumericScale = row["NumericScale"].ToString();
				string strNumericPrecision = row["NumericPrecision"].ToString();
				string strDataType = row["DataType"].ToString();

				Console.WriteLine("      ColumnName: " + strColumnName);
				Console.WriteLine("      BaseColumnName: " + strBaseColumnName);
				Console.WriteLine("      ColumnSize: " + strColumnSize);
				Console.WriteLine("      NumericScale: " + strNumericScale);
				Console.WriteLine("      NumericPrecision: " + strNumericPrecision);
				Console.WriteLine("      DataType: " + strDataType);
			}

			int r = 0;
			Console.WriteLine("    Data");
			while(reader.Read()){
				r++;
				Console.WriteLine("       Row: " + r.ToString());
				for(int f = 0; f < reader.FieldCount; f++){
					string sname = "";
					object ovalue = "";
					string svalue = "";
					string sDataType = "";
					string sFieldType = "";
					string sDataTypeName = "";
					string sOraDataType = "";

					sname = reader.GetName(f);

					if(reader.IsDBNull(f)){
						ovalue = DBNull.Value;
						svalue = "";
						sDataType = "DBNull.Value";
						sOraDataType = "DBNull.Value";
					}
					else {
						//ovalue = reader.GetValue(f);
						ovalue = reader.GetOracleValue(f);
						object oravalue = null;
					
						sDataType = ovalue.GetType().ToString();
						switch(sDataType){
						case "System.Data.OracleClient.OracleString":
							oravalue = ((OracleString) ovalue).Value;
							break;
						case "System.Data.OracleClient.OracleNumber":
							oravalue = ((OracleNumber) ovalue).Value;
							break;
						case "System.Data.OracleClient.OracleLob":
							OracleLob lob = (OracleLob) ovalue;
							oravalue = lob.Value;
							lob.Close();
							break;
						case "System.Data.OracleClient.OracleDateTime":
							oravalue = ((OracleDateTime) ovalue).Value;
							break;
						case "System.Byte[]":
							oravalue = GetHexString((byte[])ovalue);
							break;
						case "System.Decimal":
							decimal dec = reader.GetDecimal(f);

							oravalue = (object) dec;
							break;
						default:
							oravalue = ovalue.ToString();

							break;
						}
					
						sOraDataType = oravalue.GetType().ToString();
						if(sOraDataType.Equals("System.Byte[]")) 
							svalue = GetHexString((byte[]) oravalue);
						else
							svalue = oravalue.ToString();
						
					}
					sFieldType = reader.GetFieldType(f).ToString();
					sDataTypeName = reader.GetDataTypeName(f);

					Console.WriteLine("           Field: " + f.ToString());
					Console.WriteLine("               Name: " + sname);
					Console.WriteLine("               Value: " + svalue);
					Console.WriteLine("               Oracle Data Type: " + sOraDataType);
					Console.WriteLine("               Data Type: " + sDataType);
					Console.WriteLine("               Field Type: " + sFieldType);
					Console.WriteLine("               Data Type Name: " + sDataTypeName);
				}
			}
			if(r == 0)
			{
				Console.WriteLine("  No data returned.");
				return false;
			}
			return true;
		}

		private object ReadScalar(OracleConnection con, string selectSql) 
		{
			OracleCommand cmd = null;
			cmd = con.CreateCommand();
			cmd.CommandText = selectSql;

			object o = cmd.ExecuteScalar();

			string dataType = o.GetType().ToString();
			Console.WriteLine("       DataType: " + dataType);
			return o;
		}

		private bool ReadOracleScalar(OracleConnection con, string selectSql) 
		{
			OracleCommand cmd = null;
			cmd = con.CreateCommand();
			cmd.CommandText = selectSql;

			object o = cmd.ExecuteOracleScalar();

			string dataType = o.GetType().ToString();
			Console.WriteLine("       DataType: " + dataType);
			if(dataType.Equals("System.Data.OracleClient.OracleLob"))
				o = ((OracleLob)o).Value;
			if(o.GetType().ToString().Equals("System.Byte[]"))
				o = GetHexString((byte[])o);
			
			Console.WriteLine("          Value: " + o.ToString());
			return !string.IsNullOrEmpty(o.ToString());
		}

		[Fact]
		public void InsertBlobTest()
        {
			using(var con = new OracleConnection(connectionString))
            {
				con.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				con.StateChange += new StateChangeEventHandler(OnStateChange);
				con.Open();

				SetupMyPackage(con);
				
				byte[] ByteArray = new byte[2000]; // test Blob data
				byte j = 0;
				for(int i = 0; i < ByteArray.Length; i++){
					ByteArray[i] = j;
					if(j > 255)
						j = 0;
					j++;
				}

				string sproc = "MyPackage" + ".InsertBlob";

				OracleCommand cmd = new OracleCommand();
				cmd.CommandText = sproc;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.Transaction = cmd.Connection.BeginTransaction();

				try {
					OracleParameter p1 = new OracleParameter("i_Sig_File", OracleType.Blob);
					p1.Direction = ParameterDirection.Input;

					OracleLob lob2 = GetOracleLob(cmd.Transaction, ByteArray);
					p1.Value = lob2.Value;

					cmd.Parameters.Add(p1);

					cmd.ExecuteNonQuery();

					cmd.Transaction.Commit();
				
					OracleCommand select = con.CreateCommand();
					select.CommandText = "SELECT BLOB_COLUMN FROM BLOBTEST2";

					OracleDataReader reader = select.ExecuteReader();
					Assert.True(reader.Read(), "ERROR: RECORD NOT FOUND");

					if(reader.IsDBNull(0))
						Assert.True(false, "Lob IsNull");
					else {
						OracleLob lob = reader.GetOracleLob(0);
						if(lob == OracleLob.Null)
							Assert.True(false, "Lob is OracleLob.Null");
						else {
							byte[] blob = (byte[]) lob.Value;
							string result = GetHexString(blob);
							Assert.True(ByteArrayCompare(ByteArray, blob), "ByteArray and blob are not the same: bad");
						}
					}
				}
				catch(Exception e){
					Assert.True(false, e.Message);
				}
			}
        }

       [Fact]
        public void ConnectionInfo()
        {
			OracleConnection con1 = new OracleConnection();
			try
			{
				Assert.True(string.IsNullOrEmpty(con1.DataSource));

				con1.ConnectionString = connectionString;

				con1.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				con1.StateChange += new StateChangeEventHandler(OnStateChange);

				con1.Open();

				Assert.True(!string.IsNullOrEmpty(con1.ServerVersion));

				Assert.True(con1.DataSource.Equals(dataSource));

			} catch(System.Exception e){
				Assert.True(false, e.Message);
			}
			finally
			{
				if(con1 != null)
				{
					con1.Close();
				}
				else
				{
					Assert.True(false, "Conexão nula.");
				}
            	con1 = null;
			}
        }

		[Fact]
		private void MonoTest()  
		{
			using(var con = new OracleConnection(connectionString))
            {
				con.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				con.StateChange += new StateChangeEventHandler(OnStateChange);
				con.Open();

				try {
					OracleCommand cmd2 = con.CreateCommand();
					cmd2.CommandText = "DROP TABLE MONO_ORACLE_TEST";
					cmd2.ExecuteNonQuery();
				}
				catch(OracleException){
					// ignore if table already exists
				}

				OracleCommand cmd = new OracleCommand();
				cmd.Connection = con;
				cmd.CommandText = "CREATE TABLE MONO_ORACLE_TEST( " +
					" varchar2_value VarChar2(32),  " +
					" long_value long, " +
					" number_whole_value Number(18), " +
					" number_scaled_value Number(18,2), " +
					" number_integer_value Integer, " +
					" float_value Float, " +
					" date_value Date, " +
					" char_value Char(32), " +
					" clob_value Clob, " +
					" blob_value Blob, " +
					" clob_empty_value Clob, " +
					" blob_empty_value Blob, " +
					" varchar2_null_value VarChar2(32),  " +
					" number_whole_null_value Number(18), " +
					" number_scaled_null_value Number(18,2), " +
					" number_integer_null_value Integer, " +
					" float_null_value Float, " +
					" date_null_value Date, " +
					" char_null_value Char(32), " +
					" clob_null_value Clob, " +
					" blob_null_value Blob " +
					")";

				cmd.ExecuteNonQuery();

				OracleTransaction trans = con.BeginTransaction();

				cmd = new OracleCommand();
				cmd.Connection = con;
				cmd.Transaction = trans;
				cmd.CommandText = "INSERT INTO mono_oracle_test " +
					"( varchar2_value,  " +
					"  long_value, " +
					"  number_whole_value, " +
					"  number_scaled_value, " +
					"  number_integer_value, " +
					"  float_value, " +
					"  date_value, " +
					"  char_value, " +
					"  clob_value, " +
					"  blob_value, " +
					"  clob_empty_value, " +
					"  blob_empty_value " +
					") " +
					" VALUES( " +
					"  'Mono', " +
					"  'This is a LONG column', " +
					"  123, " +
					"  456.78, " +
					"  8765, " +
					"  235.2, " +
					"  TO_DATE( '2004-12-31', 'YYYY-MM-DD' ), " +
					"  'US', " +
					"  EMPTY_CLOB(), " +
					"  EMPTY_BLOB()," +
					"  EMPTY_CLOB(), " +
					"  EMPTY_BLOB()" +
					")";

				cmd.ExecuteNonQuery();

				// update BLOB and CLOB columns
				OracleCommand select = con.CreateCommand();
				select.Transaction = trans;
				select.CommandText = "SELECT CLOB_VALUE, BLOB_VALUE FROM MONO_ORACLE_TEST FOR UPDATE";
				OracleDataReader reader = select.ExecuteReader();

				Assert.True(reader.Read(), "ERROR: RECORD NOT FOUND");

				// update clob_value
				OracleLob clob = reader.GetOracleLob(0);
				byte[] bytes = null;
				UnicodeEncoding encoding = new UnicodeEncoding();
				bytes = encoding.GetBytes("Mono is fun!");
				clob.Write(bytes, 0, bytes.Length);
				clob.Close();

				// update blob_value
				OracleLob blob = reader.GetOracleLob(1);
				bytes = new byte[6] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x036 };
				blob.Write(bytes, 0, bytes.Length);
				blob.Close();
				
				trans.Commit();

				// OracleCommand.ExecuteReader of MONO_ORACLE_TEST table
				Assert.True(ReadSimpleTest(con, "SELECT * FROM MONO_ORACLE_TEST"), "Erro na consulta do MONO_ORACLE_TEST");
				

				// OracleCommand.ExecuteScalar
				string varchar2_value = (string)ReadScalar(con,"SELECT MAX(varchar2_value) FROM MONO_ORACLE_TEST");
				Assert.True(!string.IsNullOrEmpty(varchar2_value), "Error Read Scalar: varchar2_value");

				decimal? number_whole_value = (decimal?)ReadScalar(con,"SELECT MAX(number_whole_value) FROM MONO_ORACLE_TEST");
				Assert.True(number_whole_value.GetValueOrDefault() > 0, "Error Read Scalar: number_whole_value");

				decimal? number_scaled_value = (decimal?)ReadScalar(con,"SELECT number_scaled_value FROM MONO_ORACLE_TEST");
				Assert.True(number_scaled_value.GetValueOrDefault() > 0, "Error Read Scalar: number_scaled_value");
			
				DateTime? date_value = (DateTime?)ReadScalar(con,"SELECT date_value FROM MONO_ORACLE_TEST");
				Assert.True(date_value.GetValueOrDefault().ToBinary() > 0, "Error Read Scalar: date_value");
				
				string clob_value = (string)ReadScalar(con,"SELECT clob_value FROM MONO_ORACLE_TEST");
				Assert.True(!string.IsNullOrEmpty(clob_value), "Error Read Scalar: clob_value");

				byte[] blob_value = (byte[])ReadScalar(con,"SELECT blob_value FROM MONO_ORACLE_TEST");
				Assert.True(blob_value.Length > 0, "Error Read Scalar: blob_value");
				
				// OracleCommand.ExecuteOracleScalar
				Assert.True(ReadOracleScalar(con,"SELECT MAX(varchar2_value) FROM MONO_ORACLE_TEST"), "Error Read Oracle Scalar: varchar2_value");

				Assert.True(ReadOracleScalar(con,"SELECT MAX(number_whole_value) FROM MONO_ORACLE_TEST"), "Error Read Oracle Scalar: number_whole_value");

				Assert.True(ReadOracleScalar(con,"SELECT number_scaled_value FROM MONO_ORACLE_TEST"), "Error Read Oracle Scalar: number_scaled_value");

				Assert.True(ReadOracleScalar(con,"SELECT date_value FROM MONO_ORACLE_TEST"), "Error Read Oracle Scalar: date_value");
			
				Assert.True(ReadOracleScalar(con,"SELECT clob_value FROM MONO_ORACLE_TEST"), "Error Read Oracle Scalar: clob_value");

				Assert.True(ReadOracleScalar(con,"SELECT blob_value FROM MONO_ORACLE_TEST"), "Error Read Oracle Scalar: blob_value");

			}
			
		}

		[Fact]
		private void CLOBTest()
		{
			using(var connection = new OracleConnection(connectionString))
            {
				connection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				connection.StateChange += new StateChangeEventHandler(OnStateChange);
				connection.Open();

				OracleTransaction transaction = connection.BeginTransaction();

				try {
					OracleCommand cmd2 = connection.CreateCommand();
					cmd2.Transaction = transaction;
					cmd2.CommandText = "DROP TABLE CLOBTEST";
					cmd2.ExecuteNonQuery();
				}
				catch(OracleException){
					// ignore if table already exists
				}

				OracleCommand create = connection.CreateCommand();
				create.Transaction = transaction;
				create.CommandText = "CREATE TABLE CLOBTEST(CLOB_COLUMN CLOB)";
				create.ExecuteNonQuery();

				OracleCommand insert = connection.CreateCommand();
				insert.Transaction = transaction;
				insert.CommandText = "INSERT INTO CLOBTEST VALUES(EMPTY_CLOB())";
				insert.ExecuteNonQuery();

				OracleCommand select = connection.CreateCommand();
				select.Transaction = transaction;
				select.CommandText = "SELECT CLOB_COLUMN FROM CLOBTEST FOR UPDATE";

				using(OracleDataReader reader = select.ExecuteReader())
				{
					Assert.True(reader.Read(), "ERROR: RECORD NOT FOUND");

					using(OracleLob lob = reader.GetOracleLob(0))
					{
						Assert.True(lob.Length == 0, string.Format("  LENGTH: {0}", lob.Length));
						Assert.True(lob.ChunkSize == 8132, string.Format("  CHUNK SIZE: {0}", lob.ChunkSize));

						UnicodeEncoding encoding = new UnicodeEncoding();

						byte[] value = new byte[lob.Length * 2];
						Assert.True(lob.Position == 0, string.Format("  CURRENT POSITION: {0}", lob.Position));
						value = encoding.GetBytes("TEST ME!");
						lob.Write(value, 0, value.Length);

						Assert.True(lob.Position == 8, string.Format("  CURRENT POSITION: {0}", lob.Position));

						lob.Seek(1, SeekOrigin.Begin);

						Assert.True(lob.Position == 1, string.Format("  CURRENT POSITION: {0}", lob.Position));

						value = new byte[lob.Length * 2];
						lob.Read(value, 0, value.Length);

						Assert.True(!string.IsNullOrEmpty(encoding.GetString(value)), "  Read Value NULL");

						Assert.True(lob.Position == 8, string.Format("  CURRENT POSITION: {0}", lob.Position));
					}
				}
				transaction.Commit();
			}
		}

		[Fact]
		public void BLOBTest() 
		{
			using(var connection = new OracleConnection(connectionString))
            {
				connection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				connection.StateChange += new StateChangeEventHandler(OnStateChange);
				connection.Open();
				
				OracleTransaction transaction = connection.BeginTransaction();

				try {
					OracleCommand cmd2 = connection.CreateCommand();
					cmd2.Transaction = transaction;
					cmd2.CommandText = "DROP TABLE BLOBTEST";
					cmd2.ExecuteNonQuery();
				}
				catch(OracleException){
					// ignore if table already exists
				}

				OracleCommand create = connection.CreateCommand();
				create.Transaction = transaction;
				create.CommandText = "CREATE TABLE BLOBTEST(BLOB_COLUMN BLOB)";
				create.ExecuteNonQuery();

				OracleCommand insert = connection.CreateCommand();
				insert.Transaction = transaction;
				insert.CommandText = "INSERT INTO BLOBTEST VALUES(EMPTY_BLOB())";
				insert.ExecuteNonQuery();

				OracleCommand select = connection.CreateCommand();
				select.Transaction = transaction;
				select.CommandText = "SELECT BLOB_COLUMN FROM BLOBTEST FOR UPDATE";

				using(OracleDataReader reader = select.ExecuteReader())
				{
					Assert.True(reader.Read(), "ERROR: RECORD NOT FOUND");

					using(OracleLob lob = reader.GetOracleLob(0))
					{
						byte[] value = null;
						byte[] bytes = new byte[6];
						bytes[0] = 0x31;
						bytes[1] = 0x32;
						bytes[2] = 0x33;
						bytes[3] = 0x34;
						bytes[4] = 0x35;
						bytes[5] = 0x36;

						lob.Write(bytes, 0, bytes.Length);
						Assert.True(lob.Length == 6, string.Format("  LENGTH: {0}", lob.Length));

						lob.Seek(1, SeekOrigin.Begin);
						Assert.True(lob.Position == 1, string.Format("  CURRENT POSITION: {0}", lob.Position));

						value = new byte[lob.Length];
						lob.Read(value, 0, value.Length);
						Assert.True(lob.Position == 6, string.Format("  CURRENT POSITION: {0}", lob.Position));
						
						Assert.True(ByteArrayCompare((byte[])lob.Value, bytes), "Error compare bytes");

						Assert.True(lob.Position == 6, string.Format("  CURRENT POSITION: {0}", lob.Position));
					}
				}
				transaction.Commit();
			}
		}

		[Fact]
		private void DataAdapterTest()
		{
			using(var connection = new OracleConnection(connectionString))
            {
				connection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				connection.StateChange += new StateChangeEventHandler(OnStateChange);
				connection.Open();
				
				SetupEMP(connection);

				try {
				
					// Create select command...;
					OracleCommand command = connection.CreateCommand();
					command.CommandText = "SELECT * FROM EMP";

					// Create data adapter...
					OracleDataAdapter adapter = new OracleDataAdapter(command);

					// Create DataSet...
					DataSet dataSet = new DataSet("EMP");

					// Fill DataSet via data adapter...
					adapter.Fill(dataSet);

					// Get DataTable...
					DataTable table = dataSet.Tables[0];

					// Display each row...
					int rowCount = 0;
					foreach(DataRow row in table.Rows){
						for(int i = 0; i < table.Columns.Count; i += 1){
							string s = string.Format("      {0}: {1}", table.Columns[i].ColumnName, row[i]);
						}
						rowCount += 1;
					}
					Assert.True(rowCount == table.Rows.Count, string.Format("  ROWS: {0}", table.Rows.Count));
				}
				catch(Exception ex){
					Assert.True(false, ex.Message);
				}
			}
			
		}

		[Fact]
		private void RollbackTest()
		{
			using(var connection = new OracleConnection(connectionString))
            {
				connection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				connection.StateChange += new StateChangeEventHandler(OnStateChange);
				connection.Open();
				
				SetupEMP(connection);
				OracleTransaction transaction = connection.BeginTransaction();

				OracleCommand insert = connection.CreateCommand();

				insert.Transaction = transaction;
				insert.CommandText = "INSERT INTO EMP(EMPNO, ENAME, JOB) VALUES(8787, 'T Coleman', 'Monoist')";

				Assert.True(insert.ExecuteNonQuery() == 1, "Error Insert EMP");

				OracleCommand select = connection.CreateCommand();
				select.CommandText = "SELECT COUNT(*) FROM EMP WHERE EMPNO = 8787";
				select.Transaction = transaction;
				OracleDataReader reader = select.ExecuteReader();

				Assert.True(reader.Read(), "ERROR: RECORD NOT FOUND");

				Assert.True(reader.GetValue(0).ToString() == "1", "Row count SHOULD BE 1, VALUE IS " + reader.GetValue(0).ToString());

				reader.Close();

				transaction.Rollback();

				select = connection.CreateCommand();
				select.CommandText = "SELECT COUNT(*) FROM EMP WHERE EMPNO = 8787";

				reader = select.ExecuteReader();
				Assert.True(reader.Read(), "ERROR: RECORD NOT FOUND");
				Assert.True(reader.GetValue(0).ToString() == "0", "Row count SHOULD BE 1, VALUE IS " + reader.GetValue(0).ToString());
				reader.Close();
			}	
		}

		[Fact]
		private void CommitTest()
		{
			using(var connection = new OracleConnection(connectionString))
            {
				connection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				connection.StateChange += new StateChangeEventHandler(OnStateChange);
				connection.Open();
				
				SetupEMP(connection);

				OracleTransaction transaction = connection.BeginTransaction();

				OracleCommand insert = connection.CreateCommand();
				insert.Transaction = transaction;
				insert.CommandText = "INSERT INTO EMP(EMPNO, ENAME, JOB) VALUES(8787, 'T Coleman', 'Monoist')";

				insert.ExecuteNonQuery();

				OracleCommand select = connection.CreateCommand();
				select.CommandText = "SELECT COUNT(*) FROM EMP WHERE EMPNO = 8787";
				select.Transaction = transaction;

				decimal countInsert = 0;

				countInsert = (decimal)select.ExecuteScalar();
				Assert.True(countInsert == 1, string.Format("Row count SHOULD BE 1, VALUE IS {0}", countInsert));

				transaction.Commit();

				select = connection.CreateCommand();
				select.CommandText = "SELECT COUNT(*) FROM EMP WHERE EMPNO = 8787";

				countInsert = (decimal)select.ExecuteScalar();
				Assert.True(countInsert == 1, string.Format("Row count SHOULD BE 1, VALUE IS {0}", countInsert));

				transaction = connection.BeginTransaction();
				OracleCommand delete = connection.CreateCommand();
				delete.Transaction = transaction;
				delete.CommandText = "DELETE FROM EMP WHERE EMPNO = 8787";
				delete.ExecuteNonQuery();
				transaction.Commit();
			}
		}

		[Fact]
		private void ParameterTest() 
		{
			using(var connection = new OracleConnection(connectionString))
            {
				connection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
				connection.StateChange += new StateChangeEventHandler(OnStateChange);
				connection.Open();

				OracleCommand cmd2 = connection.CreateCommand();
				cmd2.CommandText = "ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD HH24:MI:SS'";
				cmd2.ExecuteNonQuery();

				try {
					cmd2.CommandText = "DROP TABLE MONO_TEST_TABLE7";
					cmd2.ExecuteNonQuery();
				}
				catch(OracleException){
					// ignore if table already exists
				}

				cmd2.CommandText = "CREATE TABLE MONO_TEST_TABLE7(" +
					" COL1 VARCHAR2(8) NOT NULL, " +
					" COL2 VARCHAR2(32), " +
					" COL3 NUMBER(18,2) NOT NULL, " +
					" COL4 NUMBER(18,2), " +
					" COL5 DATE NOT NULL, " +
					" COL6 DATE, " +
					" COL7 BLOB NOT NULL, " +
					" COL8 BLOB, " +
					" COL9 CLOB NOT NULL, " +
					" COL10 CLOB " +
					")";
				cmd2.ExecuteNonQuery();

				cmd2.CommandText = "COMMIT";
				cmd2.ExecuteNonQuery();

				OracleTransaction trans = connection.BeginTransaction();
				OracleCommand cmd = connection.CreateCommand();
				cmd.Transaction = trans;

				cmd.CommandText = "INSERT INTO MONO_TEST_TABLE7 " + 
					"(COL1,COL2,COL3,COL4,COL5,COL6,COL7,COL8,COL9,COL10) " + 
					"VALUES(:P1,:P2,:P3,:P4,:P5,:P6,:P7,:P8,:P9,:P10)";

				OracleParameter parm1 = cmd.Parameters.Add(":P1", OracleType.VarChar, 8);
				OracleParameter parm2 = cmd.Parameters.Add(":P2", OracleType.VarChar, 32);
			
				OracleParameter parm3 = cmd.Parameters.Add(":P3", OracleType.Number);
				OracleParameter parm4 = cmd.Parameters.Add(":P4", OracleType.Number);
			
				OracleParameter parm5 = cmd.Parameters.Add(":P5", OracleType.DateTime);
				OracleParameter parm6 = cmd.Parameters.Add(":P6", OracleType.DateTime);

				OracleParameter parm7 = cmd.Parameters.Add(":P7", OracleType.Blob);
				OracleParameter parm8 = cmd.Parameters.Add(":P8", OracleType.Blob);

				OracleParameter parm9 = cmd.Parameters.Add(":P9", OracleType.Clob);
				OracleParameter parm10 = cmd.Parameters.Add(":P10", OracleType.Clob);

				// TODO: implement out, return, and ref parameters

				string s = "Mono";
				decimal d = 123456789012345.678M;
				DateTime dt = DateTime.Now;

				string clob = "Clob";
				byte[] blob = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 };
			
				parm1.Value = s;
				parm2.Value = DBNull.Value;
			
				parm3.Value = d;
				parm4.Value = DBNull.Value;
			
				parm5.Value = dt;
				parm6.Value = DBNull.Value;
			
				parm7.Value = blob;
				parm8.Value = DBNull.Value;

				parm9.Value = clob;
				parm10.Value = DBNull.Value;
			
				cmd.ExecuteNonQuery();
				trans.Commit();
				
				OracleCommand select = connection.CreateCommand();
				select.CommandText = "SELECT COL10,COL9,COL8,COL7,COL6,COL5,COL4,COL3,COL2,COL1 FROM MONO_TEST_TABLE7";

				using(OracleDataReader reader = select.ExecuteReader())
				{
					int ordinal1 = reader.GetOrdinal("COL1");
					int ordinal2 = reader.GetOrdinal("COL2");
					int ordinal3 = reader.GetOrdinal("COL3");
					int ordinal4 = reader.GetOrdinal("COL4");
					int ordinal5 = reader.GetOrdinal("COL5");
					int ordinal6 = reader.GetOrdinal("COL6");
					int ordinal7 = reader.GetOrdinal("COL7");
					int ordinal8 = reader.GetOrdinal("COL8");
					int ordinal9 = reader.GetOrdinal("COL9");
					int ordinal10 = reader.GetOrdinal("COL10");

					while(reader.Read())
					{
						Assert.True(reader.GetString(ordinal1) == s, string.Format("COL1 Value: {0}", reader.GetString(ordinal1)));
						Assert.True(reader[ordinal2] == DBNull.Value, string.Format("COL2 Value: {0}", reader[ordinal2]));
						
						Assert.True(reader.GetDecimal(ordinal3) == decimal.Round(d, 2), string.Format("COL3 Value: {0}", reader.GetDecimal(ordinal3)));
						Assert.True(reader[ordinal4] == DBNull.Value, string.Format("COL4 Value: {0}", reader[ordinal4]));
						
						Assert.True(reader.GetDateTime(ordinal5).ToString() == dt.ToString(), string.Format("COL5 Value: {0}", reader.GetDateTime(ordinal5)));
						Assert.True(reader[ordinal6] == DBNull.Value, string.Format("COL6 Value: {0}", reader[ordinal6]));
						
						Assert.True(ByteArrayCompare((byte[])reader[ordinal7], blob), string.Format("COL7 Value: {0}", reader[ordinal7]));
						Assert.True(reader[ordinal8] == DBNull.Value, string.Format("COL8 Value: {0}", reader[ordinal8]));
						
						Assert.True(reader.GetString(ordinal9) == clob, string.Format("COL9 Value: {0}", reader.GetString(ordinal9)));
						Assert.True(reader[ordinal10] == DBNull.Value, string.Format("COL10 Value: {0}", reader[ordinal10]));
					}
				}
			}
			
		}


/*

		

		public static void DataAdapterTest2(OracleConnection con) 
		{
			DataAdapterTest2_Setup(con);
			ReadSimpleTest(con, "SELECT * FROM mono_adapter_test");
		
			GetMetaData(con, "SELECT * FROM mono_adapter_test");

			DataAdapterTest2_Insert(con);
			ReadSimpleTest(con, "SELECT * FROM mono_adapter_test");
		
			DataAdapterTest2_Update(con);
			ReadSimpleTest(con, "SELECT * FROM mono_adapter_test");

			DataAdapterTest2_Delete(con);
			ReadSimpleTest(con, "SELECT * FROM mono_adapter_test");
		}

		public static void GetMetaData(OracleConnection con, string sql) 
		{
			OracleCommand cmd = null;
			OracleDataReader rdr = null;
		
			cmd = con.CreateCommand();
			cmd.CommandText = sql;

			Console.WriteLine("Read Schema With KeyInfo");
			rdr = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly);
		
			DataTable dt;
			dt = rdr.GetSchemaTable();
			foreach(DataRow schemaRow in dt.Rows){
				foreach(DataColumn schemaCol in dt.Columns){
					Console.WriteLine(schemaCol.ColumnName + 
						" = " + 
						schemaRow[schemaCol]);
					Console.WriteLine("---Type: " + schemaRow[schemaCol].GetType().ToString());
				}
				Console.WriteLine("");
			}

			Console.WriteLine("Read Schema with No KeyInfo");

			rdr = cmd.ExecuteReader();

			dt = rdr.GetSchemaTable();
			foreach(DataRow schemaRow in dt.Rows){
				foreach(DataColumn schemaCol in dt.Columns){
					Console.WriteLine(schemaCol.ColumnName + 
						" = " + 
						schemaRow[schemaCol]);
					Console.WriteLine("---Type: " + schemaRow[schemaCol].GetType().ToString());
					Console.WriteLine();
				}
			}

		}

		public static void DataAdapterTest2_Setup(OracleConnection con) 
		{
			Console.WriteLine("  Drop table mono_adapter_test ...");
			try {
				OracleCommand cmd2 = con.CreateCommand();
				cmd2.CommandText = "DROP TABLE mono_adapter_test";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}

			OracleCommand cmd = null;

			Console.WriteLine("  Creating table mono_adapter_test...");
			cmd = new OracleCommand();
			cmd.Connection = con;
			cmd.CommandText = "CREATE TABLE mono_adapter_test( " +
				" varchar2_value VarChar2(32),  " +
				" number_whole_value Number(18) PRIMARY KEY, " +
				" number_scaled_value Number(18,2), " +
				" number_integer_value Integer, " +
				" float_value Float, " +
				" date_value Date, " +
                " char_value Char(32), " +
				" clob_value Clob, " +
				" blob_value Blob ) ";

			cmd.ExecuteNonQuery();

			Console.WriteLine("  Begin Trans for table mono_adapter_test...");
			OracleTransaction trans = con.BeginTransaction();

			Console.WriteLine("  Inserting value into mono_adapter_test...");
			cmd = new OracleCommand();
			cmd.Connection = con;
			cmd.Transaction = trans;
            cmd.CommandText = "INSERT INTO mono_adapter_test " +
                "( varchar2_value,  " +
                "  number_whole_value, " +
                "  number_scaled_value, " +
                "  number_integer_value, " +
                "  float_value, " +
                "  date_value, " +
                "  char_value, " +
                "  clob_value, " +
                "  blob_value " +
                ") " +
                " VALUES( " +
                "  'Mono', " +
                "  11, " +
                "  456.78, " +
                "  8765, " +
                "  235.2, " +
                "  TO_DATE( '2004-12-31', 'YYYY-MM-DD' ), " +
                "  'US', " +
                "  EMPTY_CLOB(), " +
                "  EMPTY_BLOB() " +
                ")";
			cmd.ExecuteNonQuery();

			Console.WriteLine("  Select/Update CLOB columns on table mono_adapter_test...");
		
			// update BLOB and CLOB columns
			OracleCommand select = con.CreateCommand();
			select.Transaction = trans;
			select.CommandText = "SELECT CLOB_VALUE, BLOB_VALUE FROM mono_adapter_test FOR UPDATE";
			OracleDataReader reader = select.ExecuteReader();
			if(!reader.Read())
				Console.WriteLine("ERROR: RECORD NOT FOUND");
		
			// update clob_value
			Console.WriteLine("     Update CLOB column on table mono_adapter_test...");
			OracleLob clob = reader.GetOracleLob(0);
			byte[] bytes = null;
			UnicodeEncoding encoding = new UnicodeEncoding();
			bytes = encoding.GetBytes("Mono is fun!");
			clob.Write(bytes, 0, bytes.Length);
			clob.Close();
		
			// update blob_value
			Console.WriteLine("     Update BLOB column on table mono_adapter_test...");
			OracleLob blob = reader.GetOracleLob(1);
			bytes = new byte[6] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x036 };
			blob.Write(bytes, 0, bytes.Length);
			blob.Close();
			
			Console.WriteLine("  Commit trans for table mono_adapter_test...");
			trans.Commit();

			CommitCursor(con);
		}

		public static void DataAdapterTest2_Insert(OracleConnection con) 
		{
			Console.WriteLine("================================");
			Console.WriteLine("=== Adapter Insert =============");
			Console.WriteLine("================================");
			OracleTransaction transaction = con.BeginTransaction();
		
			Console.WriteLine("   Create adapter...");
			OracleDataAdapter da = new OracleDataAdapter("select * from mono_adapter_test", con);
			da.SelectCommand.Transaction = transaction;
		
			Console.WriteLine("   Create command builder...");
			OracleCommandBuilder mycb = new OracleCommandBuilder(da);

			Console.WriteLine("   Create data set ...");
			DataSet ds = new DataSet();
		
			Console.WriteLine("   Fill data set via adapter...");
			da.Fill(ds, "mono_adapter_test");

			Console.WriteLine("   New Row...");
			DataRow myRow;
			myRow = ds.Tables["mono_adapter_test"].NewRow();

			byte[] bytes = new byte[] { 0x45,0x46,0x47,0x48,0x49,0x50 };

			Console.WriteLine("   Set values in the new DataRow...");
			myRow["varchar2_value"] = "OracleClient";
			myRow["number_whole_value"] = 22;
			myRow["number_scaled_value"] = 12.34;
			myRow["number_integer_value"] = 456;
			myRow["float_value"] = 98.76;
			myRow["date_value"] = new DateTime(2001,07,09);
			Console.WriteLine("   *** FIXME; char value not working");
			myRow["char_value"] = "Romeo";
			myRow["clob_value"] = "clobtest";
			myRow["blob_value"] = bytes;
		
			Console.WriteLine("    Add DataRow to DataTable...");		
			ds.Tables["mono_adapter_test"].Rows.Add(myRow);

			Console.WriteLine("da.Update(ds...");
			da.Update(ds, "mono_adapter_test");

			transaction.Commit();

			mycb.Dispose();
			mycb = null;
		}

		public static void DataAdapterTest2_Update(OracleConnection con) 
		{
			Console.WriteLine("================================");
			Console.WriteLine("=== Adapter Update =============");
			Console.WriteLine("================================");

			OracleTransaction transaction = con.BeginTransaction();

			Console.WriteLine("   Create adapter...");
			OracleCommand selectCmd = con.CreateCommand();
			selectCmd.Transaction = transaction;
			selectCmd.CommandText = "SELECT * FROM mono_adapter_test";
			OracleDataAdapter da = new OracleDataAdapter(selectCmd);
			Console.WriteLine("   Create command builder...");
			OracleCommandBuilder mycb = new OracleCommandBuilder(da);
			Console.WriteLine("   Create data set ...");
			DataSet ds = new DataSet();

			Console.WriteLine("   Set missing schema action...");
		
			Console.WriteLine("  Fill data set via adapter...");
			da.Fill(ds, "mono_adapter_test");
			DataRow myRow;

			Console.WriteLine("   New Row...");
			myRow = ds.Tables["mono_adapter_test"].Rows[0];

			Console.WriteLine("Tables Count: " + ds.Tables.Count.ToString());

			DataTable table = ds.Tables["mono_adapter_test"];
			DataRowCollection rows;
			rows = table.Rows;
			Console.WriteLine("   Row Count: " + rows.Count.ToString());
			myRow = rows[0];

			byte[] bytes = new byte[] { 0x62,0x63,0x64,0x65,0x66,0x67 };

			Console.WriteLine("   Set values in the new DataRow...");

			myRow["varchar2_value"] = "Super Power!";
		
			myRow["number_scaled_value"] = 12.35;
			myRow["number_integer_value"] = 457;
			myRow["float_value"] = 198.76;
			myRow["date_value"] = new DateTime(2002,08,09);
			myRow["char_value"] = "Juliet";
			myRow["clob_value"] = "this is a clob";
			myRow["blob_value"] = bytes;

			Console.WriteLine("da.Update(ds...");
			da.Update(ds, "mono_adapter_test");

			transaction.Commit();

			mycb.Dispose();
			mycb = null;
		}

		public static void DataAdapterTest2_Delete(OracleConnection con) 
		{
			Console.WriteLine("================================");
			Console.WriteLine("=== Adapter Delete =============");
			Console.WriteLine("================================");
			OracleTransaction transaction = con.BeginTransaction();
		
			Console.WriteLine("   Create adapter...");
			OracleDataAdapter da = new OracleDataAdapter("SELECT * FROM mono_adapter_test", con);
			Console.WriteLine("   Create command builder...");
			OracleCommandBuilder mycb = new OracleCommandBuilder(da);
			Console.WriteLine("   set transr...");
			da.SelectCommand.Transaction = transaction;

			Console.WriteLine("   Create data set ...");
			DataSet ds = new DataSet();
		
			Console.WriteLine("Fill data set via adapter...");
			da.Fill(ds, "mono_adapter_test");

			Console.WriteLine("delete row...");
			ds.Tables["mono_adapter_test"].Rows[0].Delete();

			Console.WriteLine("da.Update(table...");
			da.Update(ds, "mono_adapter_test");

			Console.WriteLine("Commit...");
			transaction.Commit();

			mycb.Dispose();
			mycb = null;
		}

		static void TestNonQueryUsingExecuteReader(OracleConnection con) 
		{
			OracleDataReader reader = null;
			OracleTransaction trans = null;

			Console.WriteLine("   drop table mono_adapter_test...");
			OracleCommand cmd = con.CreateCommand();

			cmd.CommandText = "DROP TABLE MONO_ADAPTER_TEST";
			trans = con.BeginTransaction();
			cmd.Transaction = trans;
			try {
				reader = cmd.ExecuteReader();
				Console.WriteLine("   RowsAffected before read: " + reader.RecordsAffected.ToString());
				reader.Read();
				Console.WriteLine("   RowsAffected after read: " + reader.RecordsAffected.ToString());
				reader.Close();
				Console.WriteLine("   RowsAffected after close: " + reader.RecordsAffected.ToString());
				trans.Commit();
			}
			catch(OracleException e){
				Console.WriteLine("   OracleException caught: " + e.Message);
				trans.Commit();
			}

			Console.WriteLine("   Create table mono_adapter_test...");
			cmd.CommandText = "CREATE TABLE MONO_ADAPTER_TEST( " +
				" varchar2_value VarChar2(32),  " +
				" number_whole_value Number(18,0) PRIMARY KEY ) ";
			trans = con.BeginTransaction();
			cmd.Transaction = trans;
			reader = cmd.ExecuteReader();
			Console.WriteLine("   RowsAffected before read: " + reader.RecordsAffected.ToString());
			reader.Read();
			Console.WriteLine("   RowsAffected after read: " + reader.RecordsAffected.ToString());
			reader.Close();
			Console.WriteLine("   RowsAffected after close: " + reader.RecordsAffected.ToString());
			trans.Commit();

			Console.WriteLine("Insert into table mono_adapter_test...");
			
			string sql =
				"INSERT INTO MONO_ADAPTER_TEST " +
				"(VARCHAR2_VALUE,NUMBER_WHOLE_VALUE) " +
				"VALUES(:p1,:p2)";

			OracleCommand cmd2 = con.CreateCommand();
			trans = con.BeginTransaction();
			cmd2.Transaction = trans;
			cmd2.CommandText = sql;
			
			OracleParameter myParameter1 = new OracleParameter("p1", OracleType.VarChar, 32);
			myParameter1.Direction = ParameterDirection.Input;
		
			OracleParameter myParameter2 = new OracleParameter("p2", OracleType.Number);
			myParameter2.Direction = ParameterDirection.Input;

			myParameter2.Value = 182;
			myParameter1.Value = "Mono";

			cmd2.Parameters.Add(myParameter1);
			cmd2.Parameters.Add(myParameter2);
			
			// insert 1 record
			reader = cmd2.ExecuteReader();
			Console.WriteLine("   RowsAffected before read: " + reader.RecordsAffected.ToString());
			reader.Read();
			Console.WriteLine("   RowsAffected after read: " + reader.RecordsAffected.ToString());
			reader.Close();
			Console.WriteLine("   RowsAffected after close: " + reader.RecordsAffected.ToString());

			// insert another record
			Console.WriteLine("   Insert another record...");
			myParameter2.Value = 183;
			myParameter1.Value = "Oracle";
			reader = cmd2.ExecuteReader();
			Console.WriteLine("   RowsAffected before read: " + reader.RecordsAffected.ToString());
			reader.Read();
			Console.WriteLine("   RowsAffected after read: " + reader.RecordsAffected.ToString());
			reader.Close();
			Console.WriteLine("   RowsAffected after close: " + reader.RecordsAffected.ToString());

			trans.Commit();
			trans = null;
			
			ReadSimpleTest(con, "SELECT * FROM MONO_ADAPTER_TEST");
		}

		static void CommitCursor(OracleConnection con) 
		{
			OracleCommand cmd = con.CreateCommand();
			cmd.CommandText = "COMMIT";
			cmd.ExecuteNonQuery();
			cmd.Dispose();
			cmd = null;
		}

		static void RollbackTest(OracleConnection connection)
		{
			OracleTransaction transaction = connection.BeginTransaction();

			OracleCommand insert = connection.CreateCommand();
			insert.Transaction = transaction;
			insert.CommandText = "INSERT INTO SYSTEM.EMP(EMPNO, ENAME, JOB) VALUES(8787, 'T Coleman', 'Monoist')";

			Console.WriteLine("  Inserting record ...");

			insert.ExecuteNonQuery();

			OracleCommand select = connection.CreateCommand();
			select.CommandText = "SELECT COUNT(*) FROM SYSTEM.EMP WHERE EMPNO = 8787";
			select.Transaction = transaction;
			OracleDataReader reader = select.ExecuteReader();
			reader.Read();

			Console.WriteLine("  Row count SHOULD BE 1, VALUE IS {0}", reader.GetValue(0));
			reader.Close();

			Console.WriteLine("  Rolling back transaction ...");

			transaction.Rollback();

			select = connection.CreateCommand();
			select.CommandText = "SELECT COUNT(*) FROM SYSTEM.EMP WHERE EMPNO = 8787";

			reader = select.ExecuteReader();
			reader.Read();
			Console.WriteLine("  Row count SHOULD BE 0, VALUE IS {0}", reader.GetValue(0));
			reader.Close();
		}
		
		static void CommitTest(OracleConnection connection)
		{
			OracleTransaction transaction = connection.BeginTransaction();

			OracleCommand insert = connection.CreateCommand();
			insert.Transaction = transaction;
			insert.CommandText = "INSERT INTO SYSTEM.EMP(EMPNO, ENAME, JOB) VALUES(8787, 'T Coleman', 'Monoist')";

			Console.WriteLine("  Inserting record ...");

			insert.ExecuteNonQuery();

			OracleCommand select = connection.CreateCommand();
			select.CommandText = "SELECT COUNT(*) FROM SYSTEM.EMP WHERE EMPNO = 8787";
			select.Transaction = transaction;

			Console.WriteLine("  Row count SHOULD BE 1, VALUE IS {0}", select.ExecuteScalar());

			Console.WriteLine("  Committing transaction ...");

			transaction.Commit();

			select = connection.CreateCommand();
			select.CommandText = "SELECT COUNT(*) FROM SYSTEM.EMP WHERE EMPNO = 8787";

			Console.WriteLine("Row count SHOULD BE 1, VALUE IS {0}", select.ExecuteScalar());
			transaction = connection.BeginTransaction();
			OracleCommand delete = connection.CreateCommand();
			delete.Transaction = transaction;
			delete.CommandText = "DELETE FROM SYSTEM.EMP WHERE EMPNO = 8787";
			delete.ExecuteNonQuery();
			transaction.Commit();
		}

		public static void ParameterTest2(OracleConnection connection)
		{
			Console.WriteLine("  Setting NLS_DATE_FORMAT...");

			OracleCommand cmd2 = connection.CreateCommand();
			cmd2.CommandText = "ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD HH24:MI:SS'";
		
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  Drop table MONO_TEST_TABLE2...");
			try {
				cmd2.CommandText = "DROP TABLE MONO_TEST_TABLE7";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}

			Console.WriteLine("  Create table MONO_TEST_TABLE7...");

			cmd2.CommandText = "CREATE TABLE MONO_TEST_TABLE7(" +
				" COL1 VARCHAR2(8) NOT NULL, " +
				" COL2 VARCHAR2(32), " +
				" COL3 NUMBER(18,2), " +
				" COL4 NUMBER(18,2), " +
				" COL5 DATE NOT NULL, " +
				" COL6 DATE, " +
				" COL7 BLOB NOT NULL, " +
				" COL8 BLOB, " +
				" COL9 CLOB NOT NULL, " +
				" COL10 CLOB " +
				")";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  COMMIT...");
			cmd2.CommandText = "COMMIT";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  create insert command...");

			OracleTransaction trans = connection.BeginTransaction();
			OracleCommand cmd = connection.CreateCommand();
			cmd.Transaction = trans;

			cmd.CommandText = "INSERT INTO MONO_TEST_TABLE7 " + 
				"(COL1,COL2,COL3,COL4,COL5,COL6,COL7,COL8,COL9,COL10) " + 
				"VALUES(:P1,:P2,:P3,:P4,:P5,:P6,:P7,:P8,:P9,:P10)";
		
			Console.WriteLine("  Add parameters...");

			OracleParameter parm1 = cmd.Parameters.Add(":P1", OracleType.VarChar, 8);
			OracleParameter parm2 = cmd.Parameters.Add(":P2", OracleType.VarChar, 32);
		
			OracleParameter parm3 = cmd.Parameters.Add(":P3", OracleType.Number);
			OracleParameter parm4 = cmd.Parameters.Add(":P4", OracleType.Number);
		
			OracleParameter parm5 = cmd.Parameters.Add(":P5", OracleType.DateTime);
			OracleParameter parm6 = cmd.Parameters.Add(":P6", OracleType.DateTime);

			// FIXME: fix BLOBs and CLOBs in OracleParameter

			OracleParameter parm7 = cmd.Parameters.Add(":P7", OracleType.Blob);
			OracleParameter parm8 = cmd.Parameters.Add(":P8", OracleType.Blob);

			OracleParameter parm9 = cmd.Parameters.Add(":P9", OracleType.Clob);
			OracleParameter parm10 = cmd.Parameters.Add(":P10", OracleType.Clob);

			// TODO: implement out, return, and ref parameters

			string s = "Mono";
			decimal d = 123456789012345.678M;
			DateTime dt = DateTime.Now;

			string clob = "Clob";
			byte[] blob = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 };
		
			Console.WriteLine("  Set Values...");

			parm1.Value = s;
			parm2.Value = DBNull.Value;
		
			parm3.Value = d;
			parm4.Value = DBNull.Value;
		
			parm5.Value = dt;
			parm6.Value = DBNull.Value;
		
			parm7.Value = blob;
			parm8.Value = DBNull.Value;

			parm9.Value = clob;
			parm10.Value = DBNull.Value;
		
			Console.WriteLine("  ExecuteNonQuery...");

			cmd.ExecuteNonQuery();
			trans.Commit();
		}

		public static void ParameterTest(OracleConnection connection) 
		{
			Console.WriteLine("  Setting NLS_DATE_FORMAT...");

			OracleCommand cmd2 = connection.CreateCommand();
			cmd2.CommandText = "ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD HH24:MI:SS'";
		
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  Drop table MONO_TEST_TABLE2...");
			try {
				cmd2.CommandText = "DROP TABLE MONO_TEST_TABLE7";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}

			Console.WriteLine("  Create table MONO_TEST_TABLE7...");

			cmd2.CommandText = "CREATE TABLE MONO_TEST_TABLE7(" +
				" COL1 VARCHAR2(8) NOT NULL, " +
				" COL2 VARCHAR2(32), " +
				" COL3 NUMBER(18,2) NOT NULL, " +
				" COL4 NUMBER(18,2), " +
				" COL5 DATE NOT NULL, " +
				" COL6 DATE, " +
				" COL7 BLOB NOT NULL, " +
				" COL8 BLOB, " +
				" COL9 CLOB NOT NULL, " +
				" COL10 CLOB " +
				")";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  COMMIT...");
			cmd2.CommandText = "COMMIT";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  create insert command...");

			OracleTransaction trans = connection.BeginTransaction();
			OracleCommand cmd = connection.CreateCommand();
			cmd.Transaction = trans;

			cmd.CommandText = "INSERT INTO MONO_TEST_TABLE7 " + 
				"(COL1,COL2,COL3,COL4,COL5,COL6,COL7,COL8,COL9,COL10) " + 
				"VALUES(:P1,:P2,:P3,:P4,:P5,:P6,:P7,:P8,:P9,:P10)";
		
			Console.WriteLine("  Add parameters...");

			OracleParameter parm1 = cmd.Parameters.Add(":P1", OracleType.VarChar, 8);
			OracleParameter parm2 = cmd.Parameters.Add(":P2", OracleType.VarChar, 32);
		
			OracleParameter parm3 = cmd.Parameters.Add(":P3", OracleType.Number);
			OracleParameter parm4 = cmd.Parameters.Add(":P4", OracleType.Number);
		
			OracleParameter parm5 = cmd.Parameters.Add(":P5", OracleType.DateTime);
			OracleParameter parm6 = cmd.Parameters.Add(":P6", OracleType.DateTime);

			// FIXME: fix BLOBs and CLOBs in OracleParameter

			OracleParameter parm7 = cmd.Parameters.Add(":P7", OracleType.Blob);
			OracleParameter parm8 = cmd.Parameters.Add(":P8", OracleType.Blob);

			OracleParameter parm9 = cmd.Parameters.Add(":P9", OracleType.Clob);
			OracleParameter parm10 = cmd.Parameters.Add(":P10", OracleType.Clob);

			// TODO: implement out, return, and ref parameters

			string s = "Mono";
			decimal d = 123456789012345.678M;
			DateTime dt = DateTime.Now;

			string clob = "Clob";
			byte[] blob = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 };
		
			Console.WriteLine("  Set Values...");

			parm1.Value = s;
			parm2.Value = DBNull.Value;
		
			parm3.Value = d;
			parm4.Value = DBNull.Value;
		
			parm5.Value = dt;
			parm6.Value = DBNull.Value;
		
			parm7.Value = blob;
			parm8.Value = DBNull.Value;

			parm9.Value = clob;
			parm10.Value = DBNull.Value;
		
			Console.WriteLine("  ExecuteNonQuery...");

			cmd.ExecuteNonQuery();
			trans.Commit();
		}

		static void Wait(string msg) 
		{
			Console.WriteLine(msg);
			if(msg.Equals(""))
				Console.WriteLine("Waiting...  Press Enter to continue...");
			Console.ReadLine();
		}

		static void StoredProcedureTest1(OracleConnection con) 
		{
			// test stored procedure with no parameters
			
			
			OracleCommand cmd2 = con.CreateCommand();

			Console.WriteLine("  Drop table MONO_TEST_TABLE1...");
			try {
				cmd2.CommandText = "DROP TABLE MONO_TEST_TABLE1";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table did not exist
			}

			Console.WriteLine("  Drop procedure SP_TEST1...");
			try {
				cmd2.CommandText = "DROP PROCEDURE SP_TEST1";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if procedure did not exist
			}

			Console.WriteLine("  Create table MONO_TEST_TABLE1...");
			cmd2.CommandText = "CREATE TABLE MONO_TEST_TABLE1(" +
					" COL1 VARCHAR2(8), "+
					" COL2 VARCHAR2(32))";
			cmd2.ExecuteNonQuery();
			
			Console.WriteLine("  Create stored procedure SP_TEST1...");
			cmd2.CommandText = "CREATE PROCEDURE SP_TEST1 " +
				" IS " +
				" BEGIN " +
				"	INSERT INTO MONO_TEST_TABLE1(COL1,COL2) VALUES('aaa','bbbb');" +
				"	COMMIT;" +
				" END;";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("COMMIT...");
			cmd2.CommandText = "COMMIT";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  Call stored procedure sp_test1...");
			OracleCommand cmd3 = con.CreateCommand();
			cmd3.CommandType = CommandType.StoredProcedure;
			cmd3.CommandText = "sp_test1";
			cmd3.ExecuteNonQuery();
		}

		static void StoredProcedureTest2(OracleConnection con) 
		{
			// test stored procedure with 2 parameters

			Console.WriteLine("  Drop table MONO_TEST_TABLE2...");
			OracleCommand cmd2 = con.CreateCommand();

			try {
				cmd2.CommandText = "DROP TABLE MONO_TEST_TABLE2";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}

			Console.WriteLine("  Drop procedure SP_TEST2...");
			try {
				cmd2.CommandText = "DROP PROCEDURE SP_TEST2";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if procedure does not exist
			}

			Console.WriteLine("  Create table MONO_TEST_TABLE2...");
						
			cmd2.CommandText = "CREATE TABLE MONO_TEST_TABLE2(" +
				" COL1 VARCHAR2(8), "+
				" COL2 VARCHAR2(32))";
			cmd2.ExecuteNonQuery();
			
			Console.WriteLine("  Create stored procedure SP_TEST2...");
			cmd2.CommandText = "CREATE PROCEDURE SP_TEST2(parm1 VARCHAR2,parm2 VARCHAR2) " +
				" IS " +
				" BEGIN " +
				"	INSERT INTO MONO_TEST_TABLE2(COL1,COL2) VALUES(parm1,parm2);" +
				"	COMMIT;" +
				" END;";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  COMMIT...");
			cmd2.CommandText = "COMMIT";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  Call stored procedure SP_TEST2 with two parameters...");
			OracleCommand cmd3 = con.CreateCommand();
			cmd3.CommandType = CommandType.StoredProcedure;
			cmd3.CommandText = "sp_test2";

			OracleParameter myParameter1 = new OracleParameter("parm1", OracleType.VarChar);
			myParameter1.Value = "yyy13";
			myParameter1.Size = 8;
			myParameter1.Direction = ParameterDirection.Input;
		
			OracleParameter myParameter2 = new OracleParameter("parm2", OracleType.VarChar);
			myParameter2.Value = "iii13";
			myParameter2.Size = 32;
			myParameter2.Direction = ParameterDirection.Input;

			cmd3.Parameters.Add(myParameter1);
			cmd3.Parameters.Add(myParameter2);

			cmd3.ExecuteNonQuery();
		}

		static void OutParmTest1(OracleConnection con)
		{
		    // test stored fuctions with 4 parameters
		    // 1. input varchar2
		    // 2. output varchar2
		    // 3. input output varchar2
		    // 4. return varchar2

		    Console.WriteLine("  Create stored function SP_OUTPUTPARMTEST1 for testing VARCHAR2 Input, Output, InputOutput, Return parameters...");
		    
		    OracleCommand cmd2 = con.CreateCommand();
		    cmd2.CommandText =
		        "CREATE OR REPLACE FUNCTION SF_TESTOUTPARM1(parm1 IN VARCHAR2, parm2 OUT VARCHAR2, parm3 IN OUT VARCHAR2) RETURN VARCHAR2 " +
		        "IS " +
		        "    returnValue VARCHAR2(32) := 'Anywhere';" +
		        "BEGIN " +
		        "   IF parm1 IS NULL THEN " +
		        "        parm2 := 'parm1 is null'; " +
		        "   ELSE " +
		        "	     parm2 := 'One' || parm1 || 'Three'; " +
		        "   END IF; " +
		        "   IF parm3 IS NOT NULL THEN " +
		        "       parm3 := parm2 || parm3 || 'Five'; " +
		        "   ELSE " +
		        "       parm3 := 'parm3 in was NULL'; " +
		        "   END IF; " +
		        "   IF parm1 IS NOT NULL THEN " +
		        "       IF parm1 = '999' THEN " +
		        "          parm2 := NULL; " +
		        "          parm3 := NULL; " +
		        "          returnValue := NULL; " +
		        "       END IF; " +
		        "   END IF; " +
		        "   RETURN returnValue; " +
		        "END;";

		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  COMMIT...");
		    cmd2.CommandText = "COMMIT";
		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  Call stored function SF_TESTOUTPARM1 with 4 parameters...");
		    OracleCommand cmd3 = con.CreateCommand();
		    cmd3.CommandType = CommandType.Text;
		    cmd3.CommandText =
		        "BEGIN " +
		        "	:ReturnValue := SF_TESTOUTPARM1(:p1, :p2, :p3); " +
		        "END;";
		    OracleParameter myParameter1 = new OracleParameter("p1", OracleType.VarChar);
		    myParameter1.Value = "Two";
		    myParameter1.Size = 32;
		    myParameter1.Direction = ParameterDirection.Input;

		    OracleParameter myParameter2 = new OracleParameter("p2", OracleType.VarChar);
		    myParameter2.Size = 32;
		    myParameter2.Direction = ParameterDirection.Output;

		    OracleParameter myParameter3 = new OracleParameter("p3", OracleType.VarChar);
		    myParameter3.Value = "Four";
		    myParameter3.Size = 32;
		    myParameter3.Direction = ParameterDirection.InputOutput;

		    OracleParameter myParameter4 = new OracleParameter("ReturnValue", OracleType.VarChar);
		    myParameter4.Size = 32;
		    myParameter4.Direction = ParameterDirection.ReturnValue;

		    cmd3.Parameters.Add(myParameter1);
		    cmd3.Parameters.Add(myParameter2);
		    cmd3.Parameters.Add(myParameter3);
		    cmd3.Parameters.Add(myParameter4);

		    cmd3.ExecuteNonQuery();
		    string outValue = (string)myParameter2.Value;
		    string inOutValue = (string)myParameter3.Value;
		    string returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    1Out Value should be: OneTwoThree");
		    Console.WriteLine("    1Out Value: " + outValue);
		    Console.WriteLine("    1InOut Value should be: OneTwoThreeFourFive");
		    Console.WriteLine("    1InOut Value: " + inOutValue);
		    Console.WriteLine("    1Return Value should be: Anywhere");
		    Console.WriteLine("    1Return Value: " + returnValue);
		    Console.WriteLine();

		    myParameter1.Value = DBNull.Value;
		    myParameter3.Value = "Hello";
		    cmd3.ExecuteNonQuery();
		    outValue = (string)myParameter2.Value;
		    inOutValue = (string)myParameter3.Value;
		    returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    2Out Value should be: parm1 is null");
		    Console.WriteLine("    2Out Value: " + outValue);
		    Console.WriteLine("    2InOut Value should be: parm1 is nullHelloFive");
		    Console.WriteLine("    2InOut Value: " + inOutValue);
		    Console.WriteLine("    2Return Value should be: Anywhere");
		    Console.WriteLine("    2Return Value: " + returnValue);
		    Console.WriteLine();

		    myParameter1.Value = "999";
		    myParameter3.Value = "Bye";
		    cmd3.ExecuteNonQuery();
		    if(myParameter2.Value == DBNull.Value)
		        outValue = "Value is DBNull.Value";
		    else
		        outValue = (string)myParameter2.Value;
		    if( myParameter3.Value == DBNull.Value)
		        inOutValue = "Value is DBNull.Value";
		    else
		        inOutValue = (string)myParameter3.Value;
		    if(myParameter4.Value == DBNull.Value)
		        returnValue = "Value is DBNull.Value";
		    else
		        returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    3Out Value should be: Value is DBNull.Value");
		    Console.WriteLine("    3Out Value: " + outValue);
		    Console.WriteLine("    3InOut Value should be: Value is DBNull.Value");
		    Console.WriteLine("    3InOut Value: " + inOutValue);
		    Console.WriteLine("    3Return Value should be: Value is DBNull.Value");
		    Console.WriteLine("    3Return Value: " + returnValue);
		    Console.WriteLine();

		    myParameter1.Value = "***";
		    myParameter3.Value = DBNull.Value;
		    cmd3.ExecuteNonQuery();
		    outValue = (string)myParameter2.Value;
		    inOutValue = (string)myParameter3.Value;
		    returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    4Out Value should be: One***Three");
		    Console.WriteLine("    4Out Value: " + outValue);
		    Console.WriteLine("    4InOut Value should be: parm3 in was NULL");
		    Console.WriteLine("    4InOut Value: " + inOutValue);
		    Console.WriteLine("    4Return Value should be: Anywhere");
		    Console.WriteLine("    4Return Value: " + returnValue);
		    Console.WriteLine();
		}

		static void OutParmTest2(OracleConnection con) 
		{
		    // test stored function with 4 parameters
		    // 1. input number(18,2)
		    // 2. output number(18,2)
		    // 3. input output number(18,2)
		    // 4. return number(18,2)

		    Console.WriteLine("  Create stored function SF_TESTOUTPARM2 to test NUMBER parameters...");

		    // stored procedure addes two numbers
		    OracleCommand cmd2 = con.CreateCommand();
		    cmd2.CommandText =
		        "CREATE OR REPLACE FUNCTION SF_TESTOUTPARM2(parm1 IN NUMBER, parm2 OUT NUMBER, parm3 IN OUT NUMBER) RETURN NUMBER " +
		        "IS " +
		        "   returnValue NUMBER := 123.45; " +
		        "BEGIN " +
		        "   IF parm1 IS NULL THEN " +
		        "      parm2 := 18; " +
		        "	   parm3 := parm3 + 8000; " +
		        "      returnValue := 78; " +
		        "   ELSIF parm1 = 999 THEN " +
		        "         parm2 := NULL;" +
		        "         parm3 := NULL;" +
		        "         returnValue := NULL;" +
		        "   ELSIF parm3 IS NULL THEN " +
		        "         parm2 := 0; " +
		        "         parm3 := 1234567890123.12345678; " +
		        "   ELSE " +
		        "	   parm2 := parm1 + 3; " +
		        "      parm3 := parm3 + 70; " +
		        "   END IF;" +
		        "   RETURN returnValue;" +
		        "END;";

		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  COMMIT...");
		    cmd2.CommandText = "COMMIT";
		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  Call stored function SP_TESTOUTPARM2 with 4 parameters...");
		    OracleCommand cmd3 = con.CreateCommand();
		    cmd3.CommandType = CommandType.Text;
		    cmd3.CommandText =
		        "BEGIN " +
		        "	:returnValue := SF_TESTOUTPARM2(:p1, :p2, :p3);" +
		        "END;";
		    OracleParameter myParameter1 = new OracleParameter("p1", OracleType.Number);
		    myParameter1.Value = 2.2;
		    myParameter1.Direction = ParameterDirection.Input;

		    OracleParameter myParameter2 = new OracleParameter("p2", OracleType.Number);
		    myParameter2.Direction = ParameterDirection.Output;

		    OracleParameter myParameter3 = new OracleParameter("p3", OracleType.Number);
		    myParameter3.Value = 33.4;
		    myParameter3.Direction = ParameterDirection.InputOutput;

		    OracleParameter myParameter4 = new OracleParameter("returnValue", OracleType.Number);
		    myParameter4.Direction = ParameterDirection.ReturnValue;

		    cmd3.Parameters.Add(myParameter1);
		    cmd3.Parameters.Add(myParameter2);
		    cmd3.Parameters.Add(myParameter3);
		    cmd3.Parameters.Add(myParameter4);

		    cmd3.ExecuteNonQuery();
		    decimal outValue = (decimal)myParameter2.Value;
		    decimal inOutValue = (decimal)myParameter3.Value;
		    decimal returnValue = (decimal)myParameter4.Value;
		    Console.WriteLine("    1Out Value should be: 5.20");
		    Console.WriteLine("    1Out Value: {0}", outValue);
		    Console.WriteLine("    1InOut Value should be: 103.40");
		    Console.WriteLine("    1InOut Value: {0}", inOutValue);
		    Console.WriteLine("    1Return Value should be: 123.45");
		    Console.WriteLine("    1Return Value: {0}", returnValue);
		    Console.WriteLine();

		    myParameter1.Value = DBNull.Value;
		    myParameter3.Value = 23;
		    cmd3.ExecuteNonQuery();
		    outValue = (decimal)myParameter2.Value;
		    inOutValue = (decimal)myParameter3.Value;
		    returnValue = (decimal)myParameter4.Value;
		    Console.WriteLine("    2Out Value should be: 18");
		    Console.WriteLine("    2Out Value: {0}", outValue);
		    Console.WriteLine("    2InOut Value should be: 8023");
		    Console.WriteLine("    2InOut Value: {0}", inOutValue);
		    Console.WriteLine("    2Return Value should be: 78");
		    Console.WriteLine("    2Return Value: {0}", returnValue);
		    Console.WriteLine();

		    string soutValue = "";
		    string sinOutValue = "";
		    string sreturnValue = "";
		    myParameter1.Value = 999;
		    myParameter3.Value = 66;
		    cmd3.ExecuteNonQuery();
		    if(myParameter2.Value == DBNull.Value)
		        soutValue = "DBNull.Value";
		    else
		        soutValue = myParameter2.Value.ToString();
		    if(myParameter3.Value == DBNull.Value)
		        sinOutValue = "DBNull.Value";
		    else
		        sinOutValue = myParameter3.Value.ToString();
		    if(myParameter4.Value == DBNull.Value)
		        sreturnValue = "DBNull.Value";
		    else
		        sreturnValue = myParameter4.Value.ToString();
		    Console.WriteLine("    3Out Value should be: DBNull.Value");
		    Console.WriteLine("    3Out Value: {0}", soutValue);
		    Console.WriteLine("    3InOut Value should be: DBNull.Value");
		    Console.WriteLine("    3InOut Value: {0}", sinOutValue);
		    Console.WriteLine("    3Return Value should be: DBNull.Value");
		    Console.WriteLine("    3Return Value: {0}", sreturnValue);
		    Console.WriteLine();

		    myParameter1.Value = 111;
		    myParameter3.Value = DBNull.Value;
		    cmd3.ExecuteNonQuery();
		    outValue = (decimal)myParameter2.Value;
		    inOutValue = (decimal)myParameter3.Value;
		    returnValue = (decimal)myParameter4.Value;
		    Console.WriteLine("    4Out Value should be: 0(as in digit zero)");
		    Console.WriteLine("    4Out Value: {0}", outValue);
		    Console.WriteLine("    4InOut Value should be: 1234567890123.12345678");
		    Console.WriteLine("    4InOut Value: {0}", inOutValue);
		    Console.WriteLine("    4Return Value should be: 123.45");
		    Console.WriteLine("    4Return Value: {0}", returnValue);
		    Console.WriteLine();

		}

		static void OutParmTest3(OracleConnection con) 
		{
		    // test stored function with 4 parameters
		    // 1. input date
		    // 2. output date
		    // 3. input output date
		    // 4. return dae

		    // a DATE type in Oracle has Date and Time          

		    Console.WriteLine("  Create stored function SF_TESTOUTPARM3 to test Date parameters...");

		    OracleCommand cmd2 = con.CreateCommand();
		    cmd2.CommandText =
		        "CREATE OR REPLACE FUNCTION SF_TESTOUTPARM3(parm1 IN DATE, parm2 OUT DATE, parm3 IN OUT DATE) RETURN DATE " +
		        "IS " +
		        "   returnValue DATE := TO_DATE('2001-07-01 15:32:52', 'YYYY-MM-DD HH24:MI:SS');" +
		        "BEGIN " +
		        "   IF parm1 IS NULL THEN " +
		        "      parm2 := TO_DATE('1900-12-31', 'YYYY-MM-DD'); " +
		        "      parm3 := TO_DATE('1900-12-31', 'YYYY-MM-DD'); " +
		        "   ELSIF parm1 = TO_DATE('1979-11-25','YYYY-MM-DD') THEN " +
		        "      parm2 := NULL;" +
		        "      parm3 := NULL;" +
		        "      returnValue := NULL;"+
		        "   ELSIF parm3 IS NULL THEN " +
		        "      parm2 := TO_DATE('2008-08-08', 'YYYY-MM-DD');" +
		        "      parm3 := TO_DATE('2000-01-01', 'YYYY-MM-DD');" +
		        "   ELSE " +
		        "      -- add 3 days to date\n " +
		        "	   parm2 := parm1 + 3; " +
		        "      parm3 := parm3 + 5; " +
		        "   END IF; " +
		        "   RETURN returnValue;" +
		        "END;";

		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  COMMIT...");
		    cmd2.CommandText = "COMMIT";
		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  Call stored function SF_TESTOUTPARM3 with 4 parameters...");
		    OracleCommand cmd3 = con.CreateCommand();
		    cmd3.CommandType = CommandType.Text;
		    cmd3.CommandText =
		        "BEGIN " +
		        "	:returnValue := SF_TESTOUTPARM3(:p1, :p2, :p3);" +
		        "END;";
		    OracleParameter myParameter1 = new OracleParameter("p1", OracleType.DateTime);
		    myParameter1.Value = new DateTime(2004, 12, 15);
		    myParameter1.Direction = ParameterDirection.Input;

		    OracleParameter myParameter2 = new OracleParameter("p2", OracleType.DateTime);
		    myParameter2.Direction = ParameterDirection.Output;

		    OracleParameter myParameter3 = new OracleParameter("p3", OracleType.DateTime);
		    myParameter3.Value = new DateTime(2008, 10, 14, 20, 21, 22);
		    myParameter3.Direction = ParameterDirection.InputOutput;

		    OracleParameter myParameter4 = new OracleParameter("returnValue", OracleType.DateTime);
		    myParameter4.Direction = ParameterDirection.ReturnValue;

		    cmd3.Parameters.Add(myParameter1);
		    cmd3.Parameters.Add(myParameter2);
		    cmd3.Parameters.Add(myParameter3);
		    cmd3.Parameters.Add(myParameter4);

		    cmd3.ExecuteNonQuery();
		    DateTime outValue = (DateTime)myParameter2.Value;
		    DateTime inOutValue = (DateTime)myParameter3.Value;
		    DateTime returnValue = (DateTime)myParameter4.Value;
		    Console.WriteLine("    1Out Value should be: 2004-12-18 00:00:00");
		    Console.WriteLine("    1Out Value: {0}", outValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    1InOut Value should be: 2008-10-19 20:21:22");
		    Console.WriteLine("    1InOut Value: {0}", inOutValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    1Return Value should be: 2001-07-01 15:32:52");
		    Console.WriteLine("    1Return Value: {0}", returnValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine();

		    myParameter1.Value = DBNull.Value;
		    myParameter3.Value = new DateTime(1980, 11, 22);
		    cmd3.ExecuteNonQuery();
		    outValue = (DateTime)myParameter2.Value;
		    inOutValue = (DateTime)myParameter3.Value;
		    returnValue = (DateTime)myParameter4.Value;
		    Console.WriteLine("    2Out Value should be: 1900-12-31 00:00:00");
		    Console.WriteLine("    2Out Value: {0}", outValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    2InOut Value should be: 1900-12-31 00:00:00");
		    Console.WriteLine("    2InOut Value: {0}", inOutValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    2Return Value should be: 2001-07-01 15:32:52");
		    Console.WriteLine("    2Return Value: {0}", returnValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine();

		    myParameter1.Value = new DateTime(1979, 11, 25);
		    myParameter3.Value = new DateTime(1981, 12, 14);
		    cmd3.ExecuteNonQuery();
		    string soutValue = "";
		    string sinOutValue = "";
		    string sreturnValue = "";
		    if(myParameter2.Value == DBNull.Value) 
		        soutValue = "DBNull.Value";
		    else {
		        outValue = (DateTime)myParameter2.Value;
		        soutValue = outValue.ToString("yyyy-MM-dd HH:mm:ss");
		    }
		    if(myParameter3.Value == DBNull.Value) 
		        sinOutValue = "DBNull.Value";
		    else {
		        inOutValue = (DateTime)myParameter3.Value;
		        sinOutValue = inOutValue.ToString("yyyy-MM-dd HH:mm:ss");
		    }
		    if(myParameter4.Value == DBNull.Value) 
		        sreturnValue = "DBNull.Value";
		    else {
		        returnValue = (DateTime)myParameter4.Value;
		        sreturnValue = returnValue.ToString("yyyy-MM-dd HH:mm:ss");
		    }
		    Console.WriteLine("    3Out Value should be: DBNull.Value");
		    Console.WriteLine("    3Out Value: {0}", soutValue);
		    Console.WriteLine("    3InOut Value should be: DBNull.Value");
		    Console.WriteLine("    3InOut Value: {0}", sinOutValue);
		    Console.WriteLine("    3Return Value should be: DBNull.Value");
		    Console.WriteLine("    3Return Value: {0}", sreturnValue);
		    Console.WriteLine();

		    myParameter1.Value = new DateTime(1976, 7, 4);
		    myParameter3.Value = DBNull.Value;
		    cmd3.ExecuteNonQuery();
		    outValue = (DateTime)myParameter2.Value;
		    inOutValue = (DateTime)myParameter3.Value;
		    returnValue = (DateTime)myParameter4.Value;
		    Console.WriteLine("    4Out Value should be: 2008-08-08 00:00:00");
		    Console.WriteLine("    4Out Value: {0}", outValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    4InOut Value should be: 2000-01-01 00:00:00");
		    Console.WriteLine("    4InOut Value: {0}", inOutValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    4Return Value should be: 2001-07-01 15:32:52");
		    Console.WriteLine("    4Return Value: {0}", returnValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine();

		}

		static void OutParmTest4(OracleConnection con)
		{
		    // test stored fuctions with 4 parameters
		    // 1. input long
		    // 2. output long
		    // 3. input output long
		    // 4. return long

		    Console.WriteLine("  Create stored function SP_OUTPUTPARMTEST4 for testing LONG VARCHAR Input, Output, InputOutput, Return parameters...");

		    OracleCommand cmd2 = con.CreateCommand();
		    cmd2.CommandText =
			"CREATE OR REPLACE FUNCTION SP_OUTPUTPARMTEST4(parm1 IN LONG, parm2 OUT LONG, parm3 IN OUT LONG) RETURN LONG " +
			"IS " +
			"    returnValue LONG := 'A very, very, very long value in a far away memory space.'; " +
			"BEGIN " +
			"   IF parm1 IS NULL THEN " +
			"        parm2 := 'parm1 is null'; " +
			"        returnValue := 'Another one bytes the dust.'; " +
			"   ELSE " +
			"	     parm2 := 'One' || parm1 || 'Three'; " +
			"   END IF; " +
			"   IF parm3 IS NOT NULL THEN " +
			"       parm3 := parm2 || parm3 || 'Five'; " +
			"   ELSE " +
			"       parm3 := 'parm3 in was NULL'; " +
			"   END IF; " +
			"   IF parm1 IS NOT NULL THEN " +
			"       IF parm1 = '999' THEN " +
			"          parm2 := NULL; " +
			"          parm3 := NULL; " +
			"          returnValue := NULL; " +
			"       END IF; " +
			"   END IF; " +
			"   RETURN returnValue; " +
			"END;";

		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  COMMIT...");
		    cmd2.CommandText = "COMMIT";
		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  Call stored procedure SP_OUTPUTPARMTEST4 with 4 parameters...");
		    OracleCommand cmd3 = con.CreateCommand();
		    cmd3.CommandType = CommandType.Text;
		    cmd3.CommandText =
			"BEGIN " +
			"	:ReturnValue := SP_OUTPUTPARMTEST4(:p1, :p2, :p3); " +
			"END;";
		    OracleParameter myParameter1 = new OracleParameter("p1", OracleType.LongVarChar);
		    myParameter1.Size = 1000;
		    myParameter1.Direction = ParameterDirection.Input;
		    myParameter1.Value = "Two";

		    OracleParameter myParameter2 = new OracleParameter("p2", OracleType.LongVarChar);
		    myParameter2.Size = 1000;
		    myParameter2.Direction = ParameterDirection.Output;

		    OracleParameter myParameter3 = new OracleParameter("p3", OracleType.LongVarChar);
		    myParameter3.Value = "Four";
		    myParameter3.Size = 1000;
		    myParameter3.Direction = ParameterDirection.InputOutput;

		    OracleParameter myParameter4 = new OracleParameter("ReturnValue", OracleType.LongVarChar);
		    myParameter4.Size = 1000;
		    myParameter4.Direction = ParameterDirection.ReturnValue;

		    cmd3.Parameters.Add(myParameter1);
		    cmd3.Parameters.Add(myParameter2);
		    cmd3.Parameters.Add(myParameter3);
		    cmd3.Parameters.Add(myParameter4);

		    cmd3.ExecuteNonQuery();
		    string outValue = (string)myParameter2.Value;
		    string inOutValue = (string)myParameter3.Value;
		    string returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    1Out Value should be: OneTwoThree");
		    Console.WriteLine("    1Out Value: " + outValue);
		    Console.WriteLine("    1InOut Value should be: OneTwoThreeFourFive");
		    Console.WriteLine("    1InOut Value: " + inOutValue);
		    Console.WriteLine("    1Return Value should be: A very, very, very long value in a far away memory space.");
		    Console.WriteLine("    1Return Value: " + returnValue);
		    Console.WriteLine();

		    myParameter1.Value = DBNull.Value;
		    myParameter3.Value = "Hello";
		    cmd3.ExecuteNonQuery();
		    outValue = (string)myParameter2.Value;
		    inOutValue = (string)myParameter3.Value;
		    returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    2Out Value should be: parm1 is null");
		    Console.WriteLine("    2Out Value: " + outValue);
		    Console.WriteLine("    2InOut Value should be: parm1 is nullHelloFive");
		    Console.WriteLine("    2InOut Value: " + inOutValue);
		    Console.WriteLine("    2Return Value should be: Another one bytes the dust.");
		    Console.WriteLine("    2Return Value: " + returnValue);
		    Console.WriteLine();

		    myParameter1.Value = "999";
		    myParameter3.Value = "Bye";
		    cmd3.ExecuteNonQuery();
		    if(myParameter2.Value == DBNull.Value)
			outValue = "Value is DBNull.Value";
		    else
			outValue = (string)myParameter2.Value;
		    if(myParameter3.Value == DBNull.Value)
			inOutValue = "Value is DBNullValue";
		    else
			inOutValue = (string)myParameter3.Value;
		    if(myParameter4.Value == DBNull.Value)
			returnValue = "Value is DBNull.Value";
		    else
			returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    3Out Value should be: Value is DBNull.Value");
		    Console.WriteLine("    3Out Value: " + outValue);
		    Console.WriteLine("    3InOut Value should be: Value is DBNull.Value");
		    Console.WriteLine("    3InOut Value: " + inOutValue);
		    Console.WriteLine("    3Return Value should be: Value is DBNull.Value");
		    Console.WriteLine("    3Return Value: " + returnValue);
		    Console.WriteLine();

		    myParameter1.Value = "***";
		    myParameter3.Value = DBNull.Value;
		    cmd3.ExecuteNonQuery();
		    outValue = (string)myParameter2.Value;
		    inOutValue = (string)myParameter3.Value;
		    returnValue = (string)myParameter4.Value;
		    Console.WriteLine("    4Out Value should be: One***Three");
		    Console.WriteLine("    4Out Value: " + outValue);
		    Console.WriteLine("    4InOut Value should be: parm3 in was NULL");
		    Console.WriteLine("    4InOut Value: " + inOutValue);
		    Console.WriteLine("    4Return Value should be: A very, very, very long value in a far away memory space.");
		    Console.WriteLine("    4Return Value: " + returnValue);
		    Console.WriteLine();
		}

		static void OutParmTest5(OracleConnection con)
		{
			// test stored fuctions with 4 parameters
			// 1. input CLOB
			// 2. output CLOB
			// 3. input output CLOB
			// 4. return CLOB

			Console.WriteLine("  Create stored function SP_OUTPUTPARMTEST5 for testing CLOB Input, Output, InputOutput, Return parameters...");

			OracleCommand cmd2 = con.CreateCommand();
			cmd2.CommandText =
			    "CREATE OR REPLACE FUNCTION SP_OUTPUTPARMTEST5(parm1 IN CLOB, parm2 OUT CLOB, parm3 IN OUT CLOB) RETURN CLOB " +
			    " IS " +
			    "    returnValue CLOB := 'Clobber'; " +
			    " BEGIN " +
			    "   IF parm1 IS NULL THEN " +
			    "        parm2 := 'parm1 is null'; " +
			    "   ELSE " +
			    "	     parm2 := 'One' || parm1 || 'Three'; " +
			    "   END IF; " +
			    "   IF parm3 IS NOT NULL THEN " +
			    "       parm3 := parm2 || parm3 || 'Five'; " +
			    "   ELSE " +
			    "       parm3 := 'parm3 in was NULL'; " +
			    "   END IF; " +
			    "   IF parm1 IS NOT NULL THEN " +
			    "       IF parm1 = '999' THEN " +
			    "          parm2 := NULL; " +
			    "          parm3 := NULL; " +
			    "          returnValue := NULL; " +
			    "       ELSIF LENGTH(parm1) = 0 THEN " +
			    "          parm2 := 'parm1 is zero length'; " +
			    "          IF LENGTH(parm3) = 0 THEN " +
			    "              parm3 := 'parm3 is zero length';" +
			    "          ELSE " +
			    "              parm3 := 'Uh oh, parm3 is not zero length like we thought'; " +
			    "          END IF; " +
			    "          returnValue := 'parm1 is zero length'; " +
			    "       ELSIF parm1 = '888' THEN " +
			    "          parm2 := EMPTY_CLOB(); " +
			    "          parm3 := EMPTY_CLOB(); " +
			    "          returnValue := EMPTY_CLOB(); " +
			    "       END IF; " +
			    "   END IF; " +
			    "   RETURN returnValue; " +
			    "END;";

			cmd2.ExecuteNonQuery();

			Console.WriteLine("  COMMIT...");
			cmd2.CommandText = "COMMIT";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  Call stored procedure SP_OUTPUTPARMTEST5 with 4 parameters...");
			//OracleTransaction trans = con.BeginTransaction();
			//OracleCommand cmd4 = con.CreateCommand();
			//cmd4.Transaction = trans;
			//OracleLob lob = CreateTemporaryLobLocator(cmd4, OracleType.Clob);

			OracleCommand cmd3 = con.CreateCommand();
			//cmd3.Transaction = trans;
			cmd3.CommandType = CommandType.Text;
			cmd3.CommandText =
			    "DECLARE " +
			    "       tempP3 CLOB; " +
			    "BEGIN " +
			    "	tempP3 := :inp3; " +
			    "	:ReturnValue := SP_OUTPUTPARMTEST5(:p1, :p2, tempP3); " +
			    "	:outp3 := tempP3;" +
			    "END;";
			OracleParameter myParameter1 = new OracleParameter("p1", OracleType.Clob);
			myParameter1.Size = 1000;
			myParameter1.Direction = ParameterDirection.Input;
			myParameter1.Value = "Two";

			OracleParameter myParameter2 = new OracleParameter("p2", OracleType.Clob);
			myParameter2.Size = 1000;
			myParameter2.Direction = ParameterDirection.Output;

			// impossible to use one OracleParameter for an CLOB IN OUT parameter?
			// I had to create two parameters for the 3rd parameter: in3 as input and out3 as output
			// and in the anonymous PL/SQL block, get and set the 3rd parameter appropriately

			OracleParameter myParameterIn3 = new OracleParameter("inp3", OracleType.Clob);
			myParameterIn3.Size = 1000;
			myParameterIn3.Direction = ParameterDirection.Input;
			string s = "Everything";
			myParameterIn3.Value = s;

			OracleParameter myParameterOut3 = new OracleParameter("outp3", OracleType.Clob);
			myParameterOut3.Size = 1000;
			myParameterOut3.Direction = ParameterDirection.Output;

			OracleParameter myParameter4 = new OracleParameter("ReturnValue", OracleType.Clob);
			myParameter4.Size = 1000;
			myParameter4.Direction = ParameterDirection.ReturnValue;

			cmd3.Parameters.Add(myParameter1);
			cmd3.Parameters.Add(myParameter2);
			cmd3.Parameters.Add(myParameterIn3);
			cmd3.Parameters.Add(myParameterOut3);
			cmd3.Parameters.Add(myParameter4);

			cmd3.ExecuteNonQuery();

			string outValue = GetOracleClobValue(myParameter2); 
			string inOutValue = GetOracleClobValue(myParameterOut3);
			string returnValue = GetOracleClobValue(myParameter4);
			Console.WriteLine("    1Out Value should be: OneTwoThree");
			Console.WriteLine("    1Out Value: " + outValue);
			Console.WriteLine("    1InOut Value should be: OneTwoThreeEverythingFive");
			Console.WriteLine("    1InOut Value: " + inOutValue);
			Console.WriteLine("    1Return Value should be: Clobber");
			Console.WriteLine("    1Return Value: " + returnValue);
			Console.WriteLine();

			myParameter1.Value = DBNull.Value;
			myParameterIn3.Value = "Hello";
			cmd3.ExecuteNonQuery();
			outValue = GetOracleClobValue(myParameter2);
			inOutValue = GetOracleClobValue(myParameterOut3);
			returnValue = GetOracleClobValue(myParameter4);
			Console.WriteLine("    2Out Value should be: parm1 is null");
			Console.WriteLine("    2Out Value: " + outValue);
			Console.WriteLine("    2InOut Value should be: parm1 is nullHelloFive");
			Console.WriteLine("    2InOut Value: " + inOutValue);
			Console.WriteLine("    2Return Value should be: Clobber");
			Console.WriteLine("    2Return Value: " + returnValue);
			Console.WriteLine();

			myParameter1.Value = "999";
			myParameterIn3.Value = "Bye";
			cmd3.ExecuteNonQuery();
			outValue = GetOracleClobValue(myParameter2);
			inOutValue = GetOracleClobValue(myParameterOut3);
			returnValue = GetOracleClobValue(myParameter4);
			Console.WriteLine("    3Out Value should be: Value is DBNull.Value");
			Console.WriteLine("    3Out Value: " + outValue);
			Console.WriteLine("    3InOut Value should be: Value is DBNull.Value");
			Console.WriteLine("    3InOut Value: " + inOutValue);
			Console.WriteLine("    3Return Value should be: Value is DBNull.Value");
			Console.WriteLine("    3Return Value: " + returnValue);
			Console.WriteLine();

			myParameter1.Value = "***";
			myParameterIn3.Value = DBNull.Value;
			cmd3.ExecuteNonQuery();
			outValue = GetOracleClobValue(myParameter2);
			inOutValue = GetOracleClobValue(myParameterOut3);
			returnValue = GetOracleClobValue(myParameter4);
			Console.WriteLine("    4Out Value should be: One***Three");
			Console.WriteLine("    4Out Value: " + outValue);
			Console.WriteLine("    4InOut Value should be: parm3 in was NULL");
			Console.WriteLine("    4InOut Value: " + inOutValue);
			Console.WriteLine("    4Return Value should be: Clobber");
			Console.WriteLine("    4Return Value: " + returnValue);
			Console.WriteLine();
			
			myParameter1.Value = OracleLob.Null;
			myParameterIn3.Value = "bass";
			cmd3.ExecuteNonQuery();
			outValue = GetOracleClobValue(myParameter2);
			inOutValue = GetOracleClobValue(myParameterOut3);
			returnValue = GetOracleClobValue(myParameter4);
			Console.WriteLine("    5Out Value should be: parm1 is null");
			Console.WriteLine("    5Out Value: " + outValue);
			Console.WriteLine("    5InOut Value should be: parm1 is nullbassFive");
			Console.WriteLine("    5InOut Value: " + inOutValue);
			Console.WriteLine("    5Return Value should be: Clobber");
			Console.WriteLine("    5Return Value: " + returnValue);
			Console.WriteLine();
			
			myParameter1.Value = "888";
			myParameterIn3.Value = "777";
			cmd3.ExecuteNonQuery();
			outValue = GetOracleClobValue(myParameter2);
			inOutValue = GetOracleClobValue(myParameterOut3);
			returnValue = GetOracleClobValue(myParameter4);
			Console.WriteLine("    6Out Value should be: Zero Length");
			Console.WriteLine("    6Out Value: " + outValue);
			Console.WriteLine("    6InOut Value should be: Zero Length");
			Console.WriteLine("    6InOut Value: " + inOutValue);
			Console.WriteLine("    6Return Value should be: Zero Length");
			Console.WriteLine("    6Return Value: " + returnValue);
			Console.WriteLine();		
		}

		public static string GetOracleClobValue(OracleParameter parm)
		{
			if(parm.Value.Equals(DBNull.Value))
				return "Clob is DBNull.Value";
			OracleLob lob = (OracleLob) parm.Value;
			if(lob.Length == 0)
				return "Zero Length";
			return lob.Value.ToString();
		}

		public static OracleLob CreateTemporaryLobLocator(OracleCommand cmd, OracleType lobType)
		{
			cmd.CommandText =
				"DECLARE TEMP_LOB " + lobType.ToString() + "; " +
				"   BEGIN " +
				"       SYS.DBMS_LOB.CREATETEMPORARY(TEMP_LOB, FALSE); " +
				"       :TempLobLocator := TEMP_LOB; " +
				" END;";

			OracleParameter parm = cmd.Parameters.Add("TempLobLocator", lobType);
			parm.Direction = ParameterDirection.Output;

			cmd.ExecuteNonQuery();

			return(OracleLob)parm.Value;
		}

		static void OutParmTest6(OracleConnection con) 
		{
		    // test stored function with 4 parameters
		    // 1. input timestamp
		    // 2. output timestamp
		    // 3. input output timestamp
		    // 4. return timestamp

		    // a TIMESTAMP type in Oracle has Date and Time          

		    Console.WriteLine("  Create stored function SF_TESTOUTPARM6 to test Date parameters...");

		    OracleCommand cmd2 = con.CreateCommand();
		    cmd2.CommandText =
		        "CREATE OR REPLACE FUNCTION SF_TESTOUTPARM6(parm1 IN TIMESTAMP, parm2 OUT TIMESTAMP, parm3 IN OUT TIMESTAMP) RETURN TIMESTAMP " +
		        "IS " +
		        "   returnValue TIMESTAMP := TO_TIMESTAMP('2001-07-01 15:32:52', 'YYYY-MM-DD HH24:MI:SS');" +
		        "BEGIN " +
		        "   IF parm1 IS NULL THEN " +
		        "      parm2 := TO_TIMESTAMP('1900-12-31', 'YYYY-MM-DD'); " +
		        "      parm3 := TO_TIMESTAMP('1900-12-31', 'YYYY-MM-DD'); " +
		        "   ELSIF parm1 = TO_TIMESTAMP('1979-11-25','YYYY-MM-DD') THEN " +
		        "      parm2 := NULL;" +
		        "      parm3 := NULL;" +
		        "      returnValue := NULL;"+
		        "   ELSIF parm3 IS NULL THEN " +
		        "      parm2 := TO_TIMESTAMP('2008-08-08', 'YYYY-MM-DD');" +
		        "      parm3 := TO_TIMESTAMP('2000-01-01', 'YYYY-MM-DD');" +
		        "   ELSE " +
		        "      -- add 3 days to date\n " +
		        "	   parm2 := parm1 + 3; " +
		        "      parm3 := parm3 + 5; " +
		        "   END IF; " +
		        "   RETURN returnValue;" +
		        "END;";

		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  COMMIT...");
		    cmd2.CommandText = "COMMIT";
		    cmd2.ExecuteNonQuery();

		    Console.WriteLine("  Call stored function SF_TESTOUTPARM6 with 4 parameters...");
		    OracleCommand cmd3 = con.CreateCommand();
		    cmd3.CommandType = CommandType.Text;
		    cmd3.CommandText =
		        "BEGIN " +
		        "	:returnValue := SF_TESTOUTPARM6(:p1, :p2, :p3);" +
		        "END;";
		    OracleParameter myParameter1 = new OracleParameter("p1", OracleType.Timestamp);
		    myParameter1.Value = new DateTime(2004, 12, 15);
		    myParameter1.Direction = ParameterDirection.Input;

		    OracleParameter myParameter2 = new OracleParameter("p2", OracleType.Timestamp);
		    myParameter2.Direction = ParameterDirection.Output;

		    OracleParameter myParameter3 = new OracleParameter("p3", OracleType.Timestamp);
		    myParameter3.Value = new DateTime(2008, 10, 14, 20, 21, 22);
		    myParameter3.Direction = ParameterDirection.InputOutput;

		    OracleParameter myParameter4 = new OracleParameter("returnValue", OracleType.Timestamp);
		    myParameter4.Direction = ParameterDirection.ReturnValue;

		    cmd3.Parameters.Add(myParameter1);
		    cmd3.Parameters.Add(myParameter2);
		    cmd3.Parameters.Add(myParameter3);
		    cmd3.Parameters.Add(myParameter4);

		    cmd3.ExecuteNonQuery();
		    DateTime outValue = (DateTime)myParameter2.Value;
		    DateTime inOutValue = (DateTime)myParameter3.Value;
		    DateTime returnValue = (DateTime)myParameter4.Value;
		    Console.WriteLine("    1Out Value should be: 2004-12-18 00:00:00");
		    Console.WriteLine("    1Out Value: {0}", outValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    1InOut Value should be: 2008-10-19 20:21:22");
		    Console.WriteLine("    1InOut Value: {0}", inOutValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    1Return Value should be: 2001-07-01 15:32:52");
		    Console.WriteLine("    1Return Value: {0}", returnValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine();

		    myParameter1.Value = DBNull.Value;
		    myParameter3.Value = new DateTime(1980, 11, 22);
		    cmd3.ExecuteNonQuery();
		    outValue = (DateTime)myParameter2.Value;
		    inOutValue = (DateTime)myParameter3.Value;
		    returnValue = (DateTime)myParameter4.Value;
		    Console.WriteLine("    2Out Value should be: 1900-12-31 00:00:00");
		    Console.WriteLine("    2Out Value: {0}", outValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    2InOut Value should be: 1900-12-31 00:00:00");
		    Console.WriteLine("    2InOut Value: {0}", inOutValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    2Return Value should be: 2001-07-01 15:32:52");
		    Console.WriteLine("    2Return Value: {0}", returnValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine();

		    myParameter1.Value = new DateTime(1979, 11, 25);
		    myParameter3.Value = new DateTime(1981, 12, 14);
		    cmd3.ExecuteNonQuery();
		    string soutValue = "";
		    string sinOutValue = "";
		    string sreturnValue = "";
		    if(myParameter2.Value == DBNull.Value) 
		        soutValue = "DBNull.Value";
		    else {
		        outValue = (DateTime)myParameter2.Value;
		        soutValue = outValue.ToString("yyyy-MM-dd HH:mm:ss");
		    }
		    if(myParameter3.Value == DBNull.Value) 
		        sinOutValue = "DBNull.Value";
		    else {
		        inOutValue = (DateTime)myParameter3.Value;
		        sinOutValue = inOutValue.ToString("yyyy-MM-dd HH:mm:ss");
		    }
		    if(myParameter4.Value == DBNull.Value) 
		        sreturnValue = "DBNull.Value";
		    else {
		        returnValue = (DateTime)myParameter4.Value;
		        sreturnValue = returnValue.ToString("yyyy-MM-dd HH:mm:ss");
		    }
		    Console.WriteLine("    3Out Value should be: DBNull.Value");
		    Console.WriteLine("    3Out Value: {0}", soutValue);
		    Console.WriteLine("    3InOut Value should be: DBNull.Value");
		    Console.WriteLine("    3InOut Value: {0}", sinOutValue);
		    Console.WriteLine("    3Return Value should be: DBNull.Value");
		    Console.WriteLine("    3Return Value: {0}", sreturnValue);
		    Console.WriteLine();

		    myParameter1.Value = new DateTime(1976, 7, 4);
		    myParameter3.Value = DBNull.Value;
		    cmd3.ExecuteNonQuery();
		    outValue = (DateTime)myParameter2.Value;
		    inOutValue = (DateTime)myParameter3.Value;
		    returnValue = (DateTime)myParameter4.Value;
		    Console.WriteLine("    4Out Value should be: 2008-08-08 00:00:00");
		    Console.WriteLine("    4Out Value: {0}", outValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    4InOut Value should be: 2000-01-01 00:00:00");
		    Console.WriteLine("    4InOut Value: {0}", inOutValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine("    4Return Value should be: 2001-07-01 15:32:52");
		    Console.WriteLine("    4Return Value: {0}", returnValue.ToString("yyyy-MM-dd HH:mm:ss"));
		    Console.WriteLine();

		}

		static void NullAggregateTest(OracleConnection con)
		{
			Console.WriteLine("  Drop table MONO_TEST_TABLE3...");
			OracleCommand cmd2 = con.CreateCommand();

			try {
				cmd2.CommandText = "DROP TABLE MONO_TEST_TABLE3";
				cmd2.ExecuteNonQuery();
			}
			catch(OracleException){
				// ignore if table already exists
			}

			Console.WriteLine("  Create table MONO_TEST_TABLE3...");
						
			cmd2.CommandText = "CREATE TABLE MONO_TEST_TABLE3(" +
				" COL1 VARCHAR2(8), "+
				" COL2 VARCHAR2(32))";

			cmd2.ExecuteNonQuery();

			Console.WriteLine("  Insert some rows into table MONO_TEST_TABLE3...");
			cmd2.CommandText = "INSERT INTO MONO_TEST_TABLE3(COL1, COL2) VALUES('1','one')";
			cmd2.ExecuteNonQuery();

			cmd2.CommandText = "INSERT INTO MONO_TEST_TABLE3(COL1, COL2) VALUES('1','uno')";
			cmd2.ExecuteNonQuery();
			
			cmd2.CommandText = "INSERT INTO MONO_TEST_TABLE3(COL1, COL2) VALUES('3','three')";
			cmd2.ExecuteNonQuery();
			
			cmd2.CommandText = "INSERT INTO MONO_TEST_TABLE3(COL1, COL2) VALUES('3', null)";
			cmd2.ExecuteNonQuery();

			cmd2.CommandText = "INSERT INTO MONO_TEST_TABLE3(COL1, COL2) VALUES('3','few')";
			cmd2.ExecuteNonQuery();

			Console.WriteLine("  ExecuteScalar...");
			cmd2.CommandText = "SELECT COL1, COUNT(COL2) AS MAX_COL1 FROM MONO_TEST_TABLE3 GROUP BY COL1";
			OracleDataReader reader = cmd2.ExecuteReader();
			Console.WriteLine(" Read...");
			while(reader.Read()){

				object obj0 = reader.GetValue(0);
				Console.WriteLine("Value 0: " + obj0.ToString());
				object obj1 = reader.GetValue(1);
				Console.WriteLine("Value 1: " + obj1.ToString());
			
				Console.WriteLine(" Read...");
			}

			Console.WriteLine(" No more records.");
		}

		static void RefCursorTests(OracleConnection con) 
		{
			SetupRefCursorTests(con); // for ref cursor tests 1 thru 3
			RefCursorTest1(con); // using BEGIN/END
			RefCursorTest2(con); // using call
			RefCursorTest3(con); // using CommandType.StoredProcedure
		
			RefCursorTest4(con);
		}

		static void SetupRefCursorTests(OracleConnection con) 
		{
			Console.WriteLine("Setup Oracle package curspkg_join...");
		
			OracleCommand cmd = con.CreateCommand();

			Console.Error.WriteLine("    create or replace package curspkg_join...");
			cmd.CommandText = 
				"CREATE OR REPLACE PACKAGE curspkg_join AS\n" +
				"TYPE t_cursor IS REF CURSOR;\n" +
				"Procedure open_join_cursor1(n_EMPNO IN NUMBER, io_cursor IN OUT t_cursor);\n" +
				"END curspkg_join;";
			cmd.ExecuteNonQuery();

			Console.Error.WriteLine("    create or replace package body curspkg_join...");			
			cmd.CommandText = 
				"CREATE OR REPLACE PACKAGE BODY curspkg_join AS\n" +
				"   Procedure open_join_cursor1(n_EMPNO IN NUMBER, io_cursor IN OUT t_cursor)\n" +
				"   IS\n" +
				"        v_cursor t_cursor;\n" +
				"   BEGIN\n" +
				"        IF n_EMPNO <> 0 THEN\n" +
				"             OPEN v_cursor FOR\n" +
				"             SELECT EMP.EMPNO, EMP.ENAME, DEPT.DEPTNO, DEPT.DNAME\n" +
				"                  FROM SYSTEM.EMP, SYSTEM.DEPT\n" +
				"                  WHERE EMP.DEPTNO = DEPT.DEPTNO\n" +
				"                  AND EMP.EMPNO = n_EMPNO;\n" +
				"\n" +
				"        ELSE\n" +
				"             OPEN v_cursor FOR\n" +
				"             SELECT EMP.EMPNO, EMP.ENAME, DEPT.DEPTNO, DEPT.DNAME\n" +
				"                  FROM SYSTEM.EMP, SYSTEM.DEPT\n" +
				"                  WHERE EMP.DEPTNO = DEPT.DEPTNO;\n" +
				"\n" +
				"        END IF;\n" +
				"        io_cursor := v_cursor;\n" +
				"   END open_join_cursor1;\n" +
				"END curspkg_join;";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "commit";
			cmd.ExecuteNonQuery();
		}

		public static void RefCursorTest4(OracleConnection connection) 
		{
			Console.WriteLine("Setup test package and data for RefCursorTest4...");
			OracleCommand cmddrop = connection.CreateCommand();

			cmddrop.CommandText = "DROP TABLE TESTTABLE";
			try { 
				cmddrop.ExecuteNonQuery(); 
			} 
			catch(OracleException e){
				Console.WriteLine("Ignore this error: " + e.Message); 
			}
			cmddrop.Dispose();
			cmddrop = null;

			OracleCommand cmd = connection.CreateCommand();

			// create table TESTTABLE
			cmd.CommandText = 
				"create table TESTTABLE(\n" +
				" col1 numeric(18,0),\n" +
				" col2 char(32),\n" +
				" col3 date)";
			cmd.ExecuteNonQuery();

			// insert some rows into TESTTABLE
			cmd.CommandText = 
				"insert into TESTTABLE\n" +
				"(col1, col2, col3)\n" +
				"values(45, 'Mono', sysdate)";
			cmd.ExecuteNonQuery();

			cmd.CommandText = 
				"insert into TESTTABLE\n" +
				"(col1, col2, col3)\n" +
				"values(136, 'Fun', sysdate)";
			cmd.ExecuteNonQuery();

			cmd.CommandText = 
				"insert into TESTTABLE\n" +
				"(col1, col2, col3)\n" +
				"values(526, 'System.Data.OracleClient', sysdate)";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "commit";
			cmd.ExecuteNonQuery();

			// create Oracle package TestTablePkg
			cmd.CommandText = 
				"CREATE OR REPLACE PACKAGE TestTablePkg\n" +
				"AS\n" +
				"	TYPE T_CURSOR IS REF CURSOR;\n" +
				"\n" +
				"	PROCEDURE GetData(tableCursor OUT T_CURSOR);\n" +
				"END TestTablePkg;";
			cmd.ExecuteNonQuery();

			// create Oracle package body for package TestTablePkg
			cmd.CommandText = 
				"CREATE OR REPLACE PACKAGE BODY TestTablePkg AS\n" +
				"  PROCEDURE GetData(tableCursor OUT T_CURSOR)\n" +
				"  IS\n" +
				"  BEGIN\n" +
				"    OPEN tableCursor FOR\n" +
				"    SELECT *\n" +
				"    FROM TestTable;\n" +
				"  END GetData;\n" +
				"END TestTablePkg;";
			cmd.ExecuteNonQuery();

			cmd.Dispose();
			cmd = null;

			Console.WriteLine("Set up command and parameters to call stored proc...");
			OracleCommand command = new OracleCommand("TestTablePkg.GetData", connection);
			command.CommandType = CommandType.StoredProcedure;
			OracleParameter parameter = new OracleParameter("tableCursor", OracleType.Cursor);
			parameter.Direction = ParameterDirection.Output;
			command.Parameters.Add(parameter);

			Console.WriteLine("Execute...");
			command.ExecuteNonQuery();

			Console.WriteLine("Get OracleDataReader for cursor output parameter...");
			OracleDataReader reader = (OracleDataReader) parameter.Value;
			
			Console.WriteLine("Read data...");
			int r = 0;
			while(reader.Read()){
				Console.WriteLine("Row {0}", r);
				for(int f = 0; f < reader.FieldCount; f ++){
					object val = reader.GetValue(f);
					Console.WriteLine("    Field {0} Value: {1}", f, val.ToString());
				}
				r ++;
			}
			Console.WriteLine("Rows retrieved: {0}", r);

			Console.WriteLine("Clean up...");
			reader.Close();
			reader = null;
			command.Dispose();
			command = null;
		}

		static void RefCursorTest1(OracleConnection con) 
		{
			Console.WriteLine("Ref Cursor Test 1 - using BEGIN/END for proc - Begin...");

			Console.WriteLine("Create command...");
			OracleCommand cmd = new OracleCommand();
			cmd.Connection = con;

			cmd.CommandText = 
				"BEGIN\n" +
				"	curspkg_join.open_join_cursor1(:n_Empno,:io_cursor);\n" +
				"END;";
		
			// PL/SQL definition of stored procedure in package curspkg_join
			// open_join_cursor1(n_EMPNO IN NUMBER, io_cursor IN OUT t_cursor)

			Console.WriteLine("Create parameters...");

			OracleParameter parm1 = new OracleParameter("n_Empno", OracleType.Number);
			parm1.Direction = ParameterDirection.Input;
			parm1.Value = 7902;

			OracleParameter parm2 = new OracleParameter("io_cursor", OracleType.Cursor);
			parm2.Direction = ParameterDirection.Output;

			cmd.Parameters.Add(parm1);
			cmd.Parameters.Add(parm2);

			// positional parm
			//cmd.Parameters.Add(new OracleParameter("io_cursor", OracleType.Cursor)).Direction = ParameterDirection.Output;
			// named parm
			//cmd.Parameters.Add("n_Empno", OracleType.Number, 4).Value = 7902;

			OracleDataReader reader;
			Console.WriteLine("Execute Non Query...");
			cmd.ExecuteNonQuery();

			Console.WriteLine("Get data reader(ref cursor) from out parameter...");
			reader = (OracleDataReader) cmd.Parameters["io_cursor"].Value;

			int x, count;
			count = 0;

			Console.WriteLine("Get data from ref cursor...");
			while(reader.Read()){
				for(x = 0; x < reader.FieldCount; x++) 
					Console.Write(reader[x] + " ");
			
				Console.WriteLine();
				count += 1;
			}
			Console.WriteLine(count.ToString() + " Rows Returned.");

			reader.Close();
		}

		static void RefCursorTest2(OracleConnection con) 
		{
			Console.WriteLine("Ref Cursor Test 2 - using call - Begin...");

			Console.WriteLine("Create command...");
			OracleCommand cmd = new OracleCommand();
			cmd.Connection = con;
			cmd.CommandText = "call curspkg_join.open_join_cursor1(:n_Empno,:io_cursor)";
		
			// PL/SQL definition of stored procedure in package curspkg_join
			// open_join_cursor1(n_EMPNO IN NUMBER, io_cursor IN OUT t_cursor)

			Console.WriteLine("Create parameters...");

			OracleParameter parm1 = new OracleParameter("n_Empno", OracleType.Number);
			parm1.Direction = ParameterDirection.Input;
			parm1.Value = 7902;

			OracleParameter parm2 = new OracleParameter("io_cursor", OracleType.Cursor);
			parm2.Direction = ParameterDirection.Output;

			cmd.Parameters.Add(parm1);
			cmd.Parameters.Add(parm2);

			// positional parm
			//cmd.Parameters.Add(new OracleParameter("io_cursor", OracleType.Cursor)).Direction = ParameterDirection.Output;
			// named parm
			//cmd.Parameters.Add("n_Empno", OracleType.Number, 4).Value = 7902;

			OracleDataReader reader;
			Console.WriteLine("Execute Non Query...");
			cmd.ExecuteNonQuery();

			Console.WriteLine("Get data reader(ref cursor) from out parameter...");
			reader = (OracleDataReader) cmd.Parameters["io_cursor"].Value;

			int x, count;
			count = 0;

			Console.WriteLine("Get data from ref cursor...");
			while(reader.Read()){
				for(x = 0; x < reader.FieldCount; x++) 
					Console.Write(reader[x] + " ");
			
				Console.WriteLine();
				count += 1;
			}
			Console.WriteLine(count.ToString() + " Rows Returned.");

			reader.Close();
		}

		static void RefCursorTest3(OracleConnection con) 
		{
			Console.WriteLine("Ref Cursor Test 3 - CommandType.StoredProcedure - Begin...");

			Console.WriteLine("Create command...");
			OracleCommand cmd = new OracleCommand();
			cmd.Connection = con;
			cmd.CommandText = "curspkg_join.open_join_cursor1";
			cmd.CommandType = CommandType.StoredProcedure;
		
			// PL/SQL definition of stored procedure in package curspkg_join
			// open_join_cursor1(n_EMPNO IN NUMBER, io_cursor IN OUT t_cursor)

			Console.WriteLine("Create parameters...");

			OracleParameter parm1 = new OracleParameter("n_Empno", OracleType.Number);
			parm1.Direction = ParameterDirection.Input;
			parm1.Value = 7902;

			OracleParameter parm2 = new OracleParameter("io_cursor", OracleType.Cursor);
			parm2.Direction = ParameterDirection.Output;

			cmd.Parameters.Add(parm1);
			cmd.Parameters.Add(parm2);

			// positional parm
			//cmd.Parameters.Add(new OracleParameter("io_cursor", OracleType.Cursor)).Direction = ParameterDirection.Output;
			// named parm
			//cmd.Parameters.Add("n_Empno", OracleType.Number, 4).Value = 7902;

			OracleDataReader reader;
			Console.WriteLine("Execute Non Query...");
			cmd.ExecuteNonQuery();

			Console.WriteLine("Get data reader(ref cursor) from out parameter...");
			reader = (OracleDataReader) cmd.Parameters["io_cursor"].Value;

			int x, count;
			count = 0;

			Console.WriteLine("Get data from ref cursor...");
			while(reader.Read()){
				for(x = 0; x < reader.FieldCount; x++) 
					Console.Write(reader[x] + " ");
			
				Console.WriteLine();
				count += 1;
			}
			Console.WriteLine(count.ToString() + " Rows Returned.");

			reader.Close();
		}

		static void ExternalAuthenticationTest() 
		{
			string user = Environment.UserName;
			if(!Environment.UserDomainName.Equals(String.Empty))
				user = Environment.UserDomainName + "\\" + Environment.UserName;
			Console.WriteLine("Environment UserDomainName and UserName: " + user);
			Console.WriteLine("Open connection using external authentication...");
			OracleConnection con = new OracleConnection("Data Source=palis;Integrated Security=true");
			try {
				con.Open();
				OracleCommand cmd = con.CreateCommand();
				cmd.CommandText = "SELECT USER FROM DUAL";
				OracleDataReader reader = cmd.ExecuteReader();
				if(reader.Read())
					Console.WriteLine("User: " + reader.GetString(reader.GetOrdinal("USER")));
				con.Close();
			}
			catch(Exception e){
				Console.WriteLine("Exception caught: " + e.Message);
				Console.WriteLine("Probably not setup for external authentication.");
			}
			con.Dispose();
			con = null;
		}

		public static void TestPersistSucurityInfo1() 
		{
			Console.WriteLine("\nTestPersistSucurityInfo1 - persist security info=false");
			OracleConnection con = new OracleConnection("data source=palis;user id=SYSTEM;password=oracle;persist security info=false");
			Console.WriteLine("ConnectionString before open: " + con.ConnectionString);
			con.Open();
			Console.WriteLine("ConnectionString after open: " + con.ConnectionString);
			con.Close();
			Console.WriteLine("ConnectionString after close: " + con.ConnectionString);
			con = null;
		}

		public static void TestPersistSucurityInfo2() 
		{
			Console.WriteLine("\nTestPersistSucurityInfo2 - persist security info=true");
			OracleConnection con = new OracleConnection("data source=palis;user id=SYSTEM;password=oracle;persist security info=true");
			Console.WriteLine("ConnectionString before open: " + con.ConnectionString);
			con.Open();
			Console.WriteLine("ConnectionString after open: " + con.ConnectionString);
			con.Close();
			Console.WriteLine("ConnectionString after close: " + con.ConnectionString);
			con = null;
		}

		public static void TestPersistSucurityInfo3() 
		{
			Console.WriteLine("\nTestPersistSucurityInfo3 - use default for persist security info which is false");
			OracleConnection con = new OracleConnection("data source=palis;user id=SYSTEM;password=oracle");
			Console.WriteLine("ConnectionString before open: " + con.ConnectionString);
			con.Open();
			Console.WriteLine("ConnectionString after open: " + con.ConnectionString);
			con.Close();
			Console.WriteLine("ConnectionString after close: " + con.ConnectionString);
			con = null;
		}

		public static void TestPersistSucurityInfo4() 
		{
			Console.WriteLine("\nTestPersistSucurityInfo4 - persist security info=false with password at front");
			OracleConnection con = new OracleConnection(";password=oracle;data source=palis;user id=SYSTEM;persist security info=false");
			Console.WriteLine("ConnectionString before open: " + con.ConnectionString);
			con.Open();
			Console.WriteLine("ConnectionString after open: " + con.ConnectionString);
			con.Close();
			Console.WriteLine("ConnectionString after close: " + con.ConnectionString);
			con = null;
		}

		public static void TestPersistSucurityInfo5() 
		{
			Console.WriteLine("\nTestPersistSucurityInfo5 - persist security info=false");
			OracleConnection con = new OracleConnection("data source=palis;user id=SYSTEM;password=oracle;persist security info=false");
			Console.WriteLine("ConnectionString before open: " + con.ConnectionString);
			con.Open();
			Console.WriteLine("ConnectionString after open: " + con.ConnectionString);
			Console.WriteLine("ConnectionState for con: " + con.State.ToString() + "\n");
		
			Console.WriteLine("Clone OracleConnection...");
			OracleConnection con2 = (OracleConnection)((ICloneable) con).Clone();
		
			Console.WriteLine("ConnectionState for con2: " + con2.State.ToString());
			Console.WriteLine("con2 ConnectionString before open: " + con2.ConnectionString);
			con2.Open();
			Console.WriteLine("con2 ConnectionString after open: " + con2.ConnectionString);
			con2.Close();
			Console.WriteLine("con2 ConnectionString after close: " + con2.ConnectionString);
		
			con.Close();
		}

		public static void TestPersistSucurityInfo6() 
		{
			Console.WriteLine("\nTestPersistSucurityInfo6 - external auth using persist security info");

			string user = Environment.UserName;
			if(!Environment.UserDomainName.Equals(String.Empty))
				user = Environment.UserDomainName + "\\" + Environment.UserName;
			Console.WriteLine("Environment UserDomainName and UserName: " + user);
			Console.WriteLine("Open connection using external authentication...");
			OracleConnection con = new OracleConnection("Data Source=palis;Integrated Security=true");
			Console.WriteLine("ConnectionString before open: " + con.ConnectionString);
			try {
				con.Open();
				OracleCommand cmd = con.CreateCommand();
				cmd.CommandText = "SELECT USER FROM DUAL";
				OracleDataReader reader = cmd.ExecuteReader();
				if(reader.Read())
					Console.WriteLine("User: " + reader.GetString(reader.GetOrdinal("USER")));
				con.Close();
				Console.WriteLine("ConnectionString after close: " + con.ConnectionString);
			}
			catch(Exception e){
				Console.WriteLine("Exception caught: " + e.Message);
				Console.WriteLine("Probably not setup for external authentication. This is fine.");
			}
			con.Dispose();
			Console.WriteLine("ConnectionString after dispose: " + con.ConnectionString);
			con = null;
			Console.WriteLine("\n\n");
		}

		public static void ConnectionPoolingTest1() 
		{
			Console.WriteLine("Start Connection Pooling Test 1...");
			OracleConnection[] connections = null;
			int maxCon = MAX_CONNECTIONS + 1; // add 1 more over the max connections to cause it to wait for the next available connection
			int i = 0;

			try {
				connections = new OracleConnection[maxCon];			
		
				for(i = 0; i < maxCon; i++){
					Console.WriteLine("   Open connection: {0}", i);
					connections[i] = new OracleConnection(conStr);
					connections[i].Open();
				}
			} catch(InvalidOperationException e){
				Console.WriteLine("Expected exception InvalidOperationException caught.");
				Console.WriteLine(e);
			}

			for(i = 0; i < maxCon; i++){
				if(connections[i] != null){
					Console.WriteLine("   Close connection: {0}", i);
					if(connections[i].State == ConnectionState.Open)
						connections[i].Close();
					connections[i].Dispose();
					connections[i] = null;
				}
			}

			connections = null;

			Console.WriteLine("Done Connection Pooling Test 1.");
		}

		public static void ConnectionPoolingTest2() 
		{
			Console.WriteLine("Start Connection Pooling Test 2...");
			OracleConnection[] connections = null;
			int maxCon = MAX_CONNECTIONS;
			int i = 0;

			connections = new OracleConnection[maxCon];			
		
			for(i = 0; i < maxCon; i++){
				Console.WriteLine("   Open connection: {0}", i);
				connections[i] = new OracleConnection(conStr);
				connections[i].Open();
			}
		
			Console.WriteLine("Start another thread...");
			t = new Thread(new ThreadStart(AnotherThreadProc));
			t.Start();

			Console.WriteLine("Sleep...");
			Thread.Sleep(100);

			Console.WriteLine("Closing...");
			for(i = 0; i < maxCon; i++){
				if(connections[i] != null){
					Console.WriteLine("   Close connection: {0}", i);
					if(connections[i].State == ConnectionState.Open)
						connections[i].Close();
					connections[i].Dispose();
					connections[i] = null;
				}
			}

			connections = null;
		}

		private static void AnotherThreadProc() 
		{
			Console.WriteLine("Open connection via another thread...");
			OracleConnection[] connections = null;
			int maxCon = MAX_CONNECTIONS; 
			int i = 0;

			connections = new OracleConnection[maxCon];			
		
			for(i = 0; i < maxCon; i++){
				Console.WriteLine("   Open connection: {0}", i);
				connections[i] = new OracleConnection(conStr);
				connections[i].Open();
			}

			Console.WriteLine("Done Connection Pooling Test 2.");
			System.Environment.Exit(0);
		}

		private static void SetParameterOracleType(OracleConnection con) 
		{
			Console.WriteLine();
			OracleParameter p = con.CreateCommand().CreateParameter();
			Console.WriteLine("p.OracleType[VarChar]: " + p.OracleType.ToString());
			p.OracleType = OracleType.Clob;
			Console.WriteLine("p.OracleType[Clob]: " + p.OracleType.ToString());
			p.Value = "SomeString";
			Console.WriteLine("p.OracleType[Clob]: " + p.OracleType.ToString());
			Console.WriteLine();

			OracleParameter p2 = con.CreateCommand().CreateParameter();
			Console.WriteLine("p2.OracleType[VarChar]: " + p2.OracleType.ToString());
			p2.Value = new byte[] { 0x01, 0x02, 0x03, 0x04 };
			Console.WriteLine("p2.OracleType[VarChar]: " + p2.OracleType.ToString());
			p2.OracleType = OracleType.Blob;
			Console.WriteLine("p2.OracleType[Blob]: " + p2.OracleType.ToString());
			Console.WriteLine();

			OracleParameter p3 = new OracleParameter("test", OracleType.Clob);
			Console.WriteLine("p3.OracleType[Clob]: " + p3.OracleType.ToString());
			p3.Value = "blah";
			Console.WriteLine("p3.OracleType[Clob]: " + p3.OracleType.ToString());
			Console.WriteLine();

			OracleParameter p4 = new OracleParameter("test", "blah");
			Console.WriteLine("p4.OracleType[VarChar]: " + p4.OracleType.ToString());
			p4.OracleType = OracleType.Clob;
			Console.WriteLine("p4.OracleType[Clob]: " + p4.OracleType.ToString());
			Console.WriteLine();

			OracleParameter p5 = new OracleParameter((string) null, new DateTime(2005, 3, 8));
			Console.WriteLine("p5.OracleType[DateTime]: " + p5.OracleType.ToString());
		}

		static void Main(string[] args) 
		{ 	
			
			Console.WriteLine("DataAdapter Test 2 BEGIN...");
			// FIXME: test is failing in NET_2_0 profile but not in NET_1_1 profile
			// Unhandled Exception: System.Data.OracleClient.OracleException: ORA-01400: cannot insert NULL 
			// into("SYSTEM"."MONO_ADAPTER_TEST"."NUMBER_WHOLE_VALUE")
			// NUMBER_WHOLE_VALUE is a primary key on the table.
			//DataAdapterTest2(con1);
			Console.WriteLine("***DataAdapter Test 2 FAILS!");
			Console.WriteLine("DataAdapter Test 2 END.");


			Console.WriteLine("Parameter Test BEGIN...");
                        ParameterTest(con1);
			ReadSimpleTest(con1, "SELECT * FROM MONO_TEST_TABLE7");
			Console.WriteLine("Parameter Test END.");

			Wait("");
			
			Console.WriteLine("Stored Proc Test 1 BEGIN...");
			StoredProcedureTest1(con1);
			ReadSimpleTest(con1, "SELECT * FROM MONO_TEST_TABLE1");
			Console.WriteLine("Stored Proc Test 1 END...");

			Wait("");

			Console.WriteLine("Stored Proc Test 2 BEGIN...");
			StoredProcedureTest2(con1);
			ReadSimpleTest(con1, "SELECT * FROM MONO_TEST_TABLE2");
			Console.WriteLine("Stored Proc Test 2 END...");

			SetParameterOracleType(con1);

			Console.WriteLine("Out Parameter and PL/SQL Block Test 1 BEGIN...");
			OutParmTest1(con1); 
			Console.WriteLine("Out Parameter and PL/SQL Block Test 1 END...");

			Console.WriteLine("Out Parameter and PL/SQL Block Test 2 BEGIN...");
			OutParmTest2(con1); 
			Console.WriteLine("Out Parameter and PL/SQL Block Test 2 END...");

			Console.WriteLine("Out Parameter and PL/SQL Block Test 3 BEGIN...");
			OutParmTest3(con1); 
			Console.WriteLine("Out Parameter and PL/SQL Block Test 3 END...");

			Console.WriteLine("Out Parameter and PL/SQL Block Test 4 BEGIN...");
			OutParmTest4(con1); 
			Console.WriteLine("Out Parameter and PL/SQL Block Test 4 END...");

			Console.WriteLine("Out Parameter and PL/SQL Block Test 5 BEGIN...");
			OutParmTest5(con1); 
			Console.WriteLine("Out Parameter and PL/SQL Block Test 5 END...");

			Console.WriteLine("Out Parameter and PL/SQL Block Test 6 BEGIN...");
			OutParmTest6(con1); 
			Console.WriteLine("Out Parameter and PL/SQL Block Test 6 END...");

			Wait("");

			Console.WriteLine("Test a Non Query using Execute Reader BEGIN...");
			TestNonQueryUsingExecuteReader(con1);
			Console.WriteLine("Test a Non Query using Execute Reader END...");

			Wait("");

			Console.WriteLine("Null Aggregate Warning BEGIN test...");
			NullAggregateTest(con1);
			Console.WriteLine("Null Aggregate Warning END test...");

			Console.WriteLine("Ref Cursor BEGIN tests...");
			RefCursorTests(con1);
			Console.WriteLine("Ref Cursor END tests...");

			Console.WriteLine("Closing...");
			con1.Close();
			Console.WriteLine("Closed.");

			conStr = conStr + ";pooling=true;min pool size=4;max pool size=" + MAX_CONNECTIONS.ToString();
			ConnectionPoolingTest1();
			ConnectionPoolingTest2();

			// Need to have an external authentication user setup in Linux and oracle
			// before running this test
			//ExternalAuthenticationTest();

			TestPersistSucurityInfo1();
			TestPersistSucurityInfo2();
			TestPersistSucurityInfo3();
			TestPersistSucurityInfo4();
			TestPersistSucurityInfo5();
			TestPersistSucurityInfo6();
			
			Console.WriteLine("Done.");
            Assert.True(true);
		}

        */

        //[Fact]
        // public void InsertTable()
        // {
        //     using(var connection = new OracleConnection("Data Source=XE;User ID=system;Password=oracle"))
        //     {
        //         connection.Open();
        //         using(var command = connection.CreateCommand())
        //         {

        //             command.Transaction = connection.BeginTransaction();
                    

        //             command.CommandText = "SELECT TABLE_NAME FROM ALL_TABLES WHERE TABLE_NAME = :p1 ORDER BY 1";
                    
        //             OracleParameter myParameter1 = new OracleParameter(":p1", OracleType.VarChar);
		//             myParameter1.Direction = ParameterDirection.Input;
        //             myParameter1.Value = "WWV_FLOW_LIST_ITEMS";
		//             command.Parameters.Add(myParameter1);
			
            
        //     // trans = con.BeginTransaction();
		// 	// cmd2.Transaction = trans;
		// 	// cmd2.CommandText = sql;
			
		// 	// OracleParameter myParameter1 = new OracleParameter("p1", OracleType.VarChar, 32);
		// 	// myParameter1.Direction = ParameterDirection.Input;
		
		// 	// OracleParameter myParameter2 = new OracleParameter("p2", OracleType.Number);
		// 	// myParameter2.Direction = ParameterDirection.Input;

		// 	// myParameter2.Value = 182;
		// 	// myParameter1.Value = "Mono";

		// 	// cmd2.Parameters.Add(myParameter1);
        //     // cmd2.Parameters.Add(myParameter2);



                    

        //             Console.WriteLine("Execute reader...");
        //             using(var reader = command.ExecuteReader())
        //             {
        //                 Console.WriteLine("Tables:");

        //                 while(reader.Read())
        //                 {
        //                     string tableName = reader.GetString(reader.GetOrdinal("TABLE_NAME"));
        //                     Console.WriteLine(tableName);
        //                 };
        //             }
        //             command.Transaction.Rollback();
        //         }
        //     }
        //     Console.WriteLine("Done.");
        //     Assert.True(true);
        // }
    }
}
