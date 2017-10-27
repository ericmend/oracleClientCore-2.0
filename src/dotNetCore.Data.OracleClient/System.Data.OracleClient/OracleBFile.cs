//
// OracleBFile.cs 
//
// Part of the Mono class libraries at
// mcs/class/System.Data.OracleClient/System.Data.OracleClient
//
// Assembly: System.Data.OracleClient.dll
// Namespace: System.Data.OracleClient
//
// Author: Tim Coleman <tim@timcoleman.com>
//
// Copyright (C) Tim Coleman, 2003
//
// Licensed under the MIT/X11 License.
//

using System;
using System.IO;
using System.Data.SqlTypes;

namespace System.Data.OracleClient
{
	public sealed class OracleBFile : Stream, ICloneable, IDisposable, INullable
	{
		#region Fields

		public static readonly new OracleBFile Null = new OracleBFile ();

		//OracleConnection connection;
		//bool isOpen;
		//bool notNull;

		#endregion // Fields

		#region Constructors

		internal OracleBFile ()
		{
		}

		#endregion // Constructors

		#region Properties

		public override bool CanRead {
			get { 
				//return (IsNull || isOpen); 
				throw new NotImplementedException ();
			}
		}

		public override bool CanSeek {
			get { 
				//return (IsNull || isOpen);
				throw new NotImplementedException ();
			}
		}

		public override bool CanWrite {
			get { 
				//return false; 
				throw new NotImplementedException ();				
			}
		}

		public OracleConnection Connection {
			get { 				
				//return connection; 
				throw new NotImplementedException ();
			}
		}

		public string DirectoryName {
			get { 
				//if (!isOpen)
				//	throw new ObjectDisposedException ("OracleBFile");
				throw new NotImplementedException ();
			}
		}

		public bool FileExists {
			get { 
				//if (!isOpen)
				//	throw new ObjectDisposedException ("OracleBFile");
				//if (Connection.State == ConnectionState.Closed)
				//	throw new InvalidOperationException ();
				throw new NotImplementedException ();
			}
		}

		public string FileName {
			get {
				//if (!isOpen)
				//	throw new ObjectDisposedException ("OracleBFile");
				//if (IsNull)
				//	return String.Empty;
				throw new NotImplementedException ();
			}
		}

		public bool IsNull {
			get { 
				//return !notNull; 
				throw new NotImplementedException ();				
			}
		}

		public override long Length {
			get { 
				//if (!isOpen)
				//	throw new ObjectDisposedException ("OracleBFile");
				throw new NotImplementedException ();
			}
		}

		public override long Position {
			get { 
				//if (!isOpen)
				//	throw new ObjectDisposedException ("OracleBFile");
				throw new NotImplementedException ();
			}
			set {
				//if (!isOpen)
				//	throw new ObjectDisposedException ("OracleBFile");
				//if (value > Length) 
				//	throw new ArgumentOutOfRangeException ();
				throw new NotImplementedException ();
			}
		}

		public object Value {
			get { 
				throw new NotImplementedException ();
			}
		}

		#endregion // Properties

		#region Methods

		public object Clone ()
		{
			throw new NotImplementedException ();
		}

		public long CopyTo (OracleLob destination)
		{
			throw new NotImplementedException ();
		}

		public long CopyTo (OracleLob destination, long destinationOffset)
		{
			throw new NotImplementedException ();
		}

		public long CopyTo (long sourceOffset, OracleLob destination, long destinationOffset, long amount)
		{
			throw new NotImplementedException ();
		}

		protected override void Dispose (bool disposing)
		{
			throw new NotImplementedException ();
		}

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException ();
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public void SetFileName (string directory, string file)
		{
			throw new NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new InvalidOperationException ();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		#endregion // Methods
	}
}


