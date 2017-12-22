//
// OciCalls.cs
//
// Part of the Mono class libraries at
// mcs/class/System.Data.OracleClient/System.Data.OracleClient.Oci
//
// Assembly: System.Data.OracleClient.dll
// Namespace: System.Data.OracleClient.Oci
//
// Authors: Joerg Rosenkranz <joergr@voelcker.com>
//          Daniel Morgan <monodanmorg@yahoo.com>
//
// Copyright (C) Joerg Rosenkranz, 2004
// Copyright (C) Daniel Morgan, 2005, 2009
//
// Licensed under the MIT/X11 License.
//

//#define ORACLE_DATA_ACCESS

//#define OCI_LINUX

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.OracleClient.Oci
{
	internal sealed class OciCalls
	{
// #if OCI_WINDOWS
//         const string OCI_DLL = "oci";
// #elif OCI_LINUX
//         const string OCI_DLL = "libclntsh.so";
// #elif OCI_MACOS
//         const string OCI_DLL = "libclntsh.dynlib";  // no sure what is on Mac
// #else 
// #error platform not set in OciCalls
// #endif


		private static string OCI_DLL;

		//ToDo Melhorar o processo de multiplas plataformas (interface e injecao de dependencia)
		private static int OS; //1-Linux; 2-MacOSX; others Windows.
		//private static object OciNativeCalls;

#if TRACE
		private static bool traceOci;

		static OciCalls()
		{
			string env = Environment.GetEnvironmentVariable("OCI_TRACE");
	
			traceOci = (env != null && env.Length > 0);
			setNativeCalls();
		}
#endif
		private static void setNativeCalls()
		{
			if ((int)Environment.OSVersion.Platform == 4 || (int)Environment.OSVersion.Platform == 128)
			{
				OS = 1;
				OCI_DLL = OciNativeCallsLinux.OCI_DLL;
			}
			else if ((int)Environment.OSVersion.Platform == 6)
			{
				OS = 2;
				OCI_DLL = OciNativeCallsOs.OCI_DLL;
			}
			else
			{
				OS = 3;
				OCI_DLL = OciNativeCallsWindows.OCI_DLL;
			}
		}
		
		private OciCalls ()
		{
			setNativeCalls();
		}

		#region OCI native calls

		#region OCI native calls Windows
		private sealed class OciNativeCallsWindows
		{
			public const string OCI_DLL = "oci";

			private OciNativeCallsWindows ()
			{}

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrSet")]
			internal static extern int OCIAttrSet (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				IntPtr attributep,
				uint size,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrSet")]
			internal static extern int OCIAttrSetString (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				string attributep,
				uint size,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

#if ORACLE_DATA_ACCESS
			[DllImport (OCI_DLL, EntryPoint = "OCIPasswordChange")]
			internal static extern int OCIPasswordChange (IntPtr svchp, 
				IntPtr errhp,
				byte [] user_name, 
				[MarshalAs (UnmanagedType.U4)] int usernm_len,
				byte [] opasswd,
				[MarshalAs (UnmanagedType.U4)] int opasswd_len,
				byte [] npasswd,
				[MarshalAs (UnmanagedType.U4)] int npasswd_len,
				[MarshalAs (UnmanagedType.U4)] uint mode);
#endif

			[DllImport (OCI_DLL)]
			internal static extern int OCIErrorGet (IntPtr hndlp,
				uint recordno,
				IntPtr sqlstate,
				out int errcodep,
				IntPtr bufp,
				uint bufsize,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByName (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByNameRef (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByNameBytes (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				byte[] valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPos (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPosBytes (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				byte[] valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPosRef (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDateTimeFromText (IntPtr hndl,
				IntPtr errhp, [In][Out] byte[] date_str, uint dstr_length,
				[In][Out] byte[] fmt, uint fmt_length,
				[In][Out] byte[] lang_name, uint lang_length, IntPtr datetime);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDefineByPos (IntPtr stmtp,
				out IntPtr defnpp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] int position,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr rlenp,
				IntPtr rcodep,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint="OCIDefineByPos")]
			internal static extern int OCIDefineByPosPtr (IntPtr stmtp,
				out IntPtr defnpp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] int position,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr rlenp,
				IntPtr rcodep,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDescriptorFree (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCIEnvCreate (out IntPtr envhpp,
				[MarshalAs (UnmanagedType.U4)] OciEnvironmentMode mode,
				IntPtr ctxp,
				IntPtr malocfp,
				IntPtr ralocfp,
				IntPtr mfreep,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCICacheFree (IntPtr envhp,
				IntPtr errhp,
				IntPtr stmthp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIAttrGet (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out IntPtr attributep,
				out int sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetSByte (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out sbyte attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetByte (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out byte attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetUInt16 (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out ushort attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetInt32 (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out int attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetIntPtr (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out IntPtr attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDescriptorAlloc (IntPtr parenth,
				out IntPtr hndlpp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIHandleAlloc (IntPtr parenth,
				out IntPtr descpp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIHandleFree (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobClose (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobCopy (IntPtr svchp,
				IntPtr errhp,
				IntPtr dst_locp,
				IntPtr src_locp,
				uint amount,
				uint dst_offset,
				uint src_offset);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobErase (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amount,
				uint offset);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobGetChunkSize (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				out uint chunk_size);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobGetLength (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				out uint lenp);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobOpen (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				byte mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobRead (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amtp,
				uint offset,
				byte[] bufp,
				uint bufl,
				IntPtr ctxp,
				IntPtr cbfp,
				ushort csid,
				byte csfrm);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobTrim (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				uint newlen);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobWrite (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amtp,
				uint offset,
				byte[] bufp,
				uint bufl,
				byte piece,
				IntPtr ctxp,
				IntPtr cbfp,
				ushort csid,
				byte csfrm);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobCharSetForm (IntPtr svchp, 
				IntPtr errhp,
				IntPtr locp,
				out byte csfrm);
			
			[DllImport (OCI_DLL)]
			internal static extern int OCINlsGetInfo (IntPtr hndl,
				IntPtr errhp,
				[In][Out] byte[] bufp,
				uint buflen,
				ushort item);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerAttach (IntPtr srvhp,
				IntPtr errhp,
				string dblink,
				[MarshalAs (UnmanagedType.U4)] int dblink_len,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerDetach (IntPtr srvhp,
				IntPtr errhp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerVersion (IntPtr hndlp,
				IntPtr errhp,
				[In][Out] byte[] bufp,
				uint bufsz,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCISessionBegin (IntPtr svchp,
				IntPtr errhp,
				IntPtr usrhp,
				[MarshalAs (UnmanagedType.U4)] OciCredentialType credt,
				[MarshalAs (UnmanagedType.U4)] OciSessionMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCISessionEnd (IntPtr svchp,
				IntPtr errhp,
				IntPtr usrhp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIParamGet (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType htype,
				IntPtr errhp,
				out IntPtr parmdpp,
				[MarshalAs (UnmanagedType.U4)] int pos);

			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtExecute (IntPtr svchp,
				IntPtr stmthp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] uint iters,
				uint rowoff,
				IntPtr snap_in,
				IntPtr snap_out,
				[MarshalAs (UnmanagedType.U4)] OciExecuteMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtFetch (IntPtr stmtp,
				IntPtr errhp,
				uint nrows,
				ushort orientation,
				uint mode);


			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtPrepare (IntPtr stmthp,
				IntPtr errhp,
				byte [] stmt,
				[MarshalAs (UnmanagedType.U4)] int stmt_length,
				[MarshalAs (UnmanagedType.U4)] OciStatementLanguage language,
				[MarshalAs (UnmanagedType.U4)] OciStatementMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransCommit (IntPtr svchp,
				IntPtr errhp,
				uint flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransRollback (IntPtr svchp,
				IntPtr errhp,
				uint flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransStart (IntPtr svchp,
				IntPtr errhp,
				uint timeout,
				[MarshalAs (UnmanagedType.U4)] OciTransactionFlags flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCICharSetToUnicode (
				IntPtr svchp,
				[MarshalAs (UnmanagedType.LPWStr)] StringBuilder dst,
				[MarshalAs (UnmanagedType.I4)] int dstlen,
				byte [] src,
				[MarshalAs (UnmanagedType.I4)] int srclen,
				out long rsize);

			[DllImport (OCI_DLL)]
			internal static extern int OCIUnicodeToCharSet (
				IntPtr svchp,
				byte [] dst,
				[MarshalAs (UnmanagedType.I4)] int dstlen,
				[MarshalAs (UnmanagedType.LPWStr)] string src,
				[MarshalAs (UnmanagedType.I4)] int srclen,
				out long rsize);

			[DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeConstruct (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				short year,
				byte month,
				byte day,
				byte hour,
				byte min,
				byte sec,
				uint fsec,
				byte[] timezone,
				uint timezone_length);
[			DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeGetDate (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				out short year,
				out byte month,
				out byte day);
			[DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeGetTime (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				out byte hour,
				out byte min,
				out byte sec,
				out uint fsec);
					
			[DllImport (OCI_DLL)]
			internal static extern int OCIIntervalGetDaySecond (IntPtr hndl,
				IntPtr err,
				out int days,
				out int hours,
				out int mins,
				out int secs,
				out int fsec,
				IntPtr interval);

			[DllImport (OCI_DLL)]
			internal static extern int OCIIntervalGetYearMonth (IntPtr hndl,
				IntPtr err,
				out int years,
				out int months,
				IntPtr interval);
			[DllImport (OCI_DLL)]
			internal static extern int OCIDateTimeCheck (IntPtr hndl,
				IntPtr err, IntPtr date, out uint valid);
		}
		#endregion
		
		#region OCI native calls Os
		private sealed class OciNativeCallsOs
		{
			public const string OCI_DLL = "libclntsh.dynlib";

			private OciNativeCallsOs ()
			{}

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrSet")]
			internal static extern int OCIAttrSet (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				IntPtr attributep,
				uint size,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrSet")]
			internal static extern int OCIAttrSetString (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				string attributep,
				uint size,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

#if ORACLE_DATA_ACCESS
			[DllImport (OCI_DLL, EntryPoint = "OCIPasswordChange")]
			internal static extern int OCIPasswordChange (IntPtr svchp, 
				IntPtr errhp,
				byte [] user_name, 
				[MarshalAs (UnmanagedType.U4)] int usernm_len,
				byte [] opasswd,
				[MarshalAs (UnmanagedType.U4)] int opasswd_len,
				byte [] npasswd,
				[MarshalAs (UnmanagedType.U4)] int npasswd_len,
				[MarshalAs (UnmanagedType.U4)] uint mode);
#endif

			[DllImport (OCI_DLL)]
			internal static extern int OCIErrorGet (IntPtr hndlp,
				uint recordno,
				IntPtr sqlstate,
				out int errcodep,
				IntPtr bufp,
				uint bufsize,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByName (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByNameRef (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByNameBytes (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				byte[] valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPos (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPosBytes (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				byte[] valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPosRef (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDateTimeFromText (IntPtr hndl,
				IntPtr errhp, [In][Out] byte[] date_str, uint dstr_length,
				[In][Out] byte[] fmt, uint fmt_length,
				[In][Out] byte[] lang_name, uint lang_length, IntPtr datetime);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDefineByPos (IntPtr stmtp,
				out IntPtr defnpp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] int position,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr rlenp,
				IntPtr rcodep,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint="OCIDefineByPos")]
			internal static extern int OCIDefineByPosPtr (IntPtr stmtp,
				out IntPtr defnpp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] int position,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr rlenp,
				IntPtr rcodep,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDescriptorFree (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCIEnvCreate (out IntPtr envhpp,
				[MarshalAs (UnmanagedType.U4)] OciEnvironmentMode mode,
				IntPtr ctxp,
				IntPtr malocfp,
				IntPtr ralocfp,
				IntPtr mfreep,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCICacheFree (IntPtr envhp,
				IntPtr errhp,
				IntPtr stmthp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIAttrGet (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out IntPtr attributep,
				out int sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetSByte (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out sbyte attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetByte (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out byte attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetUInt16 (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out ushort attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetInt32 (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out int attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetIntPtr (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out IntPtr attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDescriptorAlloc (IntPtr parenth,
				out IntPtr hndlpp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIHandleAlloc (IntPtr parenth,
				out IntPtr descpp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIHandleFree (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobClose (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobCopy (IntPtr svchp,
				IntPtr errhp,
				IntPtr dst_locp,
				IntPtr src_locp,
				uint amount,
				uint dst_offset,
				uint src_offset);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobErase (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amount,
				uint offset);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobGetChunkSize (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				out uint chunk_size);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobGetLength (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				out uint lenp);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobOpen (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				byte mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobRead (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amtp,
				uint offset,
				byte[] bufp,
				uint bufl,
				IntPtr ctxp,
				IntPtr cbfp,
				ushort csid,
				byte csfrm);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobTrim (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				uint newlen);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobWrite (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amtp,
				uint offset,
				byte[] bufp,
				uint bufl,
				byte piece,
				IntPtr ctxp,
				IntPtr cbfp,
				ushort csid,
				byte csfrm);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobCharSetForm (IntPtr svchp, 
				IntPtr errhp,
				IntPtr locp,
				out byte csfrm);
			
			[DllImport (OCI_DLL)]
			internal static extern int OCINlsGetInfo (IntPtr hndl,
				IntPtr errhp,
				[In][Out] byte[] bufp,
				uint buflen,
				ushort item);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerAttach (IntPtr srvhp,
				IntPtr errhp,
				string dblink,
				[MarshalAs (UnmanagedType.U4)] int dblink_len,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerDetach (IntPtr srvhp,
				IntPtr errhp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerVersion (IntPtr hndlp,
				IntPtr errhp,
				[In][Out] byte[] bufp,
				uint bufsz,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCISessionBegin (IntPtr svchp,
				IntPtr errhp,
				IntPtr usrhp,
				[MarshalAs (UnmanagedType.U4)] OciCredentialType credt,
				[MarshalAs (UnmanagedType.U4)] OciSessionMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCISessionEnd (IntPtr svchp,
				IntPtr errhp,
				IntPtr usrhp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIParamGet (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType htype,
				IntPtr errhp,
				out IntPtr parmdpp,
				[MarshalAs (UnmanagedType.U4)] int pos);

			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtExecute (IntPtr svchp,
				IntPtr stmthp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] uint iters,
				uint rowoff,
				IntPtr snap_in,
				IntPtr snap_out,
				[MarshalAs (UnmanagedType.U4)] OciExecuteMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtFetch (IntPtr stmtp,
				IntPtr errhp,
				uint nrows,
				ushort orientation,
				uint mode);


			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtPrepare (IntPtr stmthp,
				IntPtr errhp,
				byte [] stmt,
				[MarshalAs (UnmanagedType.U4)] int stmt_length,
				[MarshalAs (UnmanagedType.U4)] OciStatementLanguage language,
				[MarshalAs (UnmanagedType.U4)] OciStatementMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransCommit (IntPtr svchp,
				IntPtr errhp,
				uint flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransRollback (IntPtr svchp,
				IntPtr errhp,
				uint flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransStart (IntPtr svchp,
				IntPtr errhp,
				uint timeout,
				[MarshalAs (UnmanagedType.U4)] OciTransactionFlags flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCICharSetToUnicode (
				IntPtr svchp,
				[MarshalAs (UnmanagedType.LPWStr)] StringBuilder dst,
				[MarshalAs (UnmanagedType.I4)] int dstlen,
				byte [] src,
				[MarshalAs (UnmanagedType.I4)] int srclen,
				out long rsize);

			[DllImport (OCI_DLL)]
			internal static extern int OCIUnicodeToCharSet (
				IntPtr svchp,
				byte [] dst,
				[MarshalAs (UnmanagedType.I4)] int dstlen,
				[MarshalAs (UnmanagedType.LPWStr)] string src,
				[MarshalAs (UnmanagedType.I4)] int srclen,
				out long rsize);

			[DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeConstruct (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				short year,
				byte month,
				byte day,
				byte hour,
				byte min,
				byte sec,
				uint fsec,
				byte[] timezone,
				uint timezone_length);
[			DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeGetDate (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				out short year,
				out byte month,
				out byte day);
			[DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeGetTime (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				out byte hour,
				out byte min,
				out byte sec,
				out uint fsec);
					
			[DllImport (OCI_DLL)]
			internal static extern int OCIIntervalGetDaySecond (IntPtr hndl,
				IntPtr err,
				out int days,
				out int hours,
				out int mins,
				out int secs,
				out int fsec,
				IntPtr interval);

			[DllImport (OCI_DLL)]
			internal static extern int OCIIntervalGetYearMonth (IntPtr hndl,
				IntPtr err,
				out int years,
				out int months,
				IntPtr interval);
			[DllImport (OCI_DLL)]
			internal static extern int OCIDateTimeCheck (IntPtr hndl,
				IntPtr err, IntPtr date, out uint valid);
		}
		#endregion
		
		#region OCI native calls Linux
		private sealed class OciNativeCallsLinux
		{
			public const string OCI_DLL = "libclntsh.so";

			private OciNativeCallsLinux ()
			{}

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrSet")]
			internal static extern int OCIAttrSet (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				IntPtr attributep,
				uint size,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrSet")]
			internal static extern int OCIAttrSetString (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				string attributep,
				uint size,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

#if ORACLE_DATA_ACCESS
			[DllImport (OCI_DLL, EntryPoint = "OCIPasswordChange")]
			internal static extern int OCIPasswordChange (IntPtr svchp, 
				IntPtr errhp,
				byte [] user_name, 
				[MarshalAs (UnmanagedType.U4)] int usernm_len,
				byte [] opasswd,
				[MarshalAs (UnmanagedType.U4)] int opasswd_len,
				byte [] npasswd,
				[MarshalAs (UnmanagedType.U4)] int npasswd_len,
				[MarshalAs (UnmanagedType.U4)] uint mode);
#endif

			[DllImport (OCI_DLL)]
			internal static extern int OCIErrorGet (IntPtr hndlp,
				uint recordno,
				IntPtr sqlstate,
				out int errcodep,
				IntPtr bufp,
				uint bufsize,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByName (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByNameRef (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByName")]
			internal static extern int OCIBindByNameBytes (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				string placeholder,
				int placeh_len,
				byte[] valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPos (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPosBytes (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				byte[] valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint = "OCIBindByPos")]
			internal static extern int OCIBindByPosRef (IntPtr stmtp,
				out IntPtr bindpp,
				IntPtr errhp,
				uint position,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr alenp,
				IntPtr rcodep,
				uint maxarr_len,
				IntPtr curelp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDateTimeFromText (IntPtr hndl,
				IntPtr errhp, [In][Out] byte[] date_str, uint dstr_length,
				[In][Out] byte[] fmt, uint fmt_length,
				[In][Out] byte[] lang_name, uint lang_length, IntPtr datetime);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDefineByPos (IntPtr stmtp,
				out IntPtr defnpp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] int position,
				IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr rlenp,
				IntPtr rcodep,
				uint mode);

			[DllImport (OCI_DLL, EntryPoint="OCIDefineByPos")]
			internal static extern int OCIDefineByPosPtr (IntPtr stmtp,
				out IntPtr defnpp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] int position,
				ref IntPtr valuep,
				int value_sz,
				[MarshalAs (UnmanagedType.U4)] OciDataType dty,
				IntPtr indp,
				IntPtr rlenp,
				IntPtr rcodep,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDescriptorFree (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCIEnvCreate (out IntPtr envhpp,
				[MarshalAs (UnmanagedType.U4)] OciEnvironmentMode mode,
				IntPtr ctxp,
				IntPtr malocfp,
				IntPtr ralocfp,
				IntPtr mfreep,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCICacheFree (IntPtr envhp,
				IntPtr errhp,
				IntPtr stmthp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIAttrGet (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out IntPtr attributep,
				out int sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetSByte (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out sbyte attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetByte (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out byte attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetUInt16 (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out ushort attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetInt32 (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out int attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL, EntryPoint = "OCIAttrGet")]
			internal static extern int OCIAttrGetIntPtr (IntPtr trgthndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
				out IntPtr attributep,
				IntPtr sizep,
				[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
				IntPtr errhp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIDescriptorAlloc (IntPtr parenth,
				out IntPtr hndlpp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIHandleAlloc (IntPtr parenth,
				out IntPtr descpp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type,
				int xtramem_sz,
				IntPtr usrmempp);

			[DllImport (OCI_DLL)]
			internal static extern int OCIHandleFree (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobClose (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobCopy (IntPtr svchp,
				IntPtr errhp,
				IntPtr dst_locp,
				IntPtr src_locp,
				uint amount,
				uint dst_offset,
				uint src_offset);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobErase (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amount,
				uint offset);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobGetChunkSize (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				out uint chunk_size);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobGetLength (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				out uint lenp);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobOpen (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				byte mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobRead (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amtp,
				uint offset,
				byte[] bufp,
				uint bufl,
				IntPtr ctxp,
				IntPtr cbfp,
				ushort csid,
				byte csfrm);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobTrim (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				uint newlen);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobWrite (IntPtr svchp,
				IntPtr errhp,
				IntPtr locp,
				ref uint amtp,
				uint offset,
				byte[] bufp,
				uint bufl,
				byte piece,
				IntPtr ctxp,
				IntPtr cbfp,
				ushort csid,
				byte csfrm);

			[DllImport (OCI_DLL)]
			internal static extern int OCILobCharSetForm (IntPtr svchp, 
				IntPtr errhp,
				IntPtr locp,
				out byte csfrm);
			
			[DllImport (OCI_DLL)]
			internal static extern int OCINlsGetInfo (IntPtr hndl,
				IntPtr errhp,
				[In][Out] byte[] bufp,
				uint buflen,
				ushort item);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerAttach (IntPtr srvhp,
				IntPtr errhp,
				string dblink,
				[MarshalAs (UnmanagedType.U4)] int dblink_len,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerDetach (IntPtr srvhp,
				IntPtr errhp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIServerVersion (IntPtr hndlp,
				IntPtr errhp,
				[In][Out] byte[] bufp,
				uint bufsz,
				[MarshalAs (UnmanagedType.U4)] OciHandleType type);

			[DllImport (OCI_DLL)]
			internal static extern int OCISessionBegin (IntPtr svchp,
				IntPtr errhp,
				IntPtr usrhp,
				[MarshalAs (UnmanagedType.U4)] OciCredentialType credt,
				[MarshalAs (UnmanagedType.U4)] OciSessionMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCISessionEnd (IntPtr svchp,
				IntPtr errhp,
				IntPtr usrhp,
				uint mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIParamGet (IntPtr hndlp,
				[MarshalAs (UnmanagedType.U4)] OciHandleType htype,
				IntPtr errhp,
				out IntPtr parmdpp,
				[MarshalAs (UnmanagedType.U4)] int pos);

			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtExecute (IntPtr svchp,
				IntPtr stmthp,
				IntPtr errhp,
				[MarshalAs (UnmanagedType.U4)] uint iters,
				uint rowoff,
				IntPtr snap_in,
				IntPtr snap_out,
				[MarshalAs (UnmanagedType.U4)] OciExecuteMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtFetch (IntPtr stmtp,
				IntPtr errhp,
				uint nrows,
				ushort orientation,
				uint mode);


			[DllImport (OCI_DLL)]
			internal static extern int OCIStmtPrepare (IntPtr stmthp,
				IntPtr errhp,
				byte [] stmt,
				[MarshalAs (UnmanagedType.U4)] int stmt_length,
				[MarshalAs (UnmanagedType.U4)] OciStatementLanguage language,
				[MarshalAs (UnmanagedType.U4)] OciStatementMode mode);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransCommit (IntPtr svchp,
				IntPtr errhp,
				uint flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransRollback (IntPtr svchp,
				IntPtr errhp,
				uint flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCITransStart (IntPtr svchp,
				IntPtr errhp,
				uint timeout,
				[MarshalAs (UnmanagedType.U4)] OciTransactionFlags flags);

			[DllImport (OCI_DLL)]
			internal static extern int OCICharSetToUnicode (
				IntPtr svchp,
				[MarshalAs (UnmanagedType.LPWStr)] StringBuilder dst,
				[MarshalAs (UnmanagedType.I4)] int dstlen,
				byte [] src,
				[MarshalAs (UnmanagedType.I4)] int srclen,
				out long rsize);

			[DllImport (OCI_DLL)]
			internal static extern int OCIUnicodeToCharSet (
				IntPtr svchp,
				byte [] dst,
				[MarshalAs (UnmanagedType.I4)] int dstlen,
				[MarshalAs (UnmanagedType.LPWStr)] string src,
				[MarshalAs (UnmanagedType.I4)] int srclen,
				out long rsize);

			[DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeConstruct (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				short year,
				byte month,
				byte day,
				byte hour,
				byte min,
				byte sec,
				uint fsec,
				byte[] timezone,
				uint timezone_length);
[			DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeGetDate (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				out short year,
				out byte month,
				out byte day);
			[DllImport (OCI_DLL)]
			internal static extern void OCIDateTimeGetTime (IntPtr hndl,
				IntPtr err,
				IntPtr datetime,
				out byte hour,
				out byte min,
				out byte sec,
				out uint fsec);
					
			[DllImport (OCI_DLL)]
			internal static extern int OCIIntervalGetDaySecond (IntPtr hndl,
				IntPtr err,
				out int days,
				out int hours,
				out int mins,
				out int secs,
				out int fsec,
				IntPtr interval);

			[DllImport (OCI_DLL)]
			internal static extern int OCIIntervalGetYearMonth (IntPtr hndl,
				IntPtr err,
				out int years,
				out int months,
				IntPtr interval);
			[DllImport (OCI_DLL)]
			internal static extern int OCIDateTimeCheck (IntPtr hndl,
				IntPtr err, IntPtr date, out uint valid);
		}
		#endregion

		#endregion

		#region OCI call wrappers

		internal static int OCIAttrSet (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			IntPtr attributep,
			uint size,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, string.Format("OCIAttrSet ({0}, {1})", trghndltyp, attrtype), OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrSet (trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrSet (trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrSet (trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
			
		}

		internal static int OCIAttrSetString (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			string attributep,
			uint size,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, string.Format("OCIAttrSetString ({0}, {1})", trghndltyp, attrtype), OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrSetString (trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrSetString (trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrSetString (trgthndlp, trghndltyp, attributep, size, attrtype, errhp);		
		}
#if ORACLE_DATA_ACCESS
		internal static int OCIPasswordChange (IntPtr svchp, IntPtr errhp,
				int usernm_len,
				byte [] opasswd,
				int opasswd_len,
				byte [] npasswd,
				int npasswd_len,
				uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, string.Format("OCIPasswordChange"), OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIPasswordChange (svchp, errhp, user_name, (uint) usernm_len, opasswd, (uint) opasswd_len, npasswd, (uint) npasswd_len, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIPasswordChange (svchp, errhp, user_name, (uint) usernm_len, opasswd, (uint) opasswd_len, npasswd, (uint) npasswd_len, mode);
			else
				return OciNativeCallsWindows.OCIPasswordChange (svchp, errhp, user_name, (uint) usernm_len, opasswd, (uint) opasswd_len, npasswd, (uint) npasswd_len, mode);
		}
#endif
		internal static int OCIErrorGet (IntPtr hndlp,
			uint recordno,
			IntPtr sqlstate,
			out int errcodep,
			IntPtr bufp,
			uint bufsize,
			OciHandleType type)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIErrorGet", OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIErrorGet (hndlp, recordno, sqlstate, out errcodep, bufp, bufsize, type);
			else if (OS == 2)
				return OciNativeCallsOs.OCIErrorGet (hndlp, recordno, sqlstate, out errcodep, bufp, bufsize, type);
			else
				return OciNativeCallsWindows.OCIErrorGet (hndlp, recordno, sqlstate, out errcodep, bufp, bufsize, type);			
		}

		internal static int OCIBindByName (IntPtr stmtp,
			out IntPtr bindpp,
			IntPtr errhp,
			string placeholder,
			int placeh_len,
			IntPtr valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr alenp,
			IntPtr rcodep,
			uint maxarr_len,
			IntPtr curelp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIBindByName", OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIBindByName (stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIBindByName (stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else
				return OciNativeCallsWindows.OCIBindByName (stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
		}

		internal static int OCIBindByNameRef (IntPtr stmtp,
			out IntPtr bindpp,
			IntPtr errhp,
			string placeholder,
			int placeh_len,
			ref IntPtr valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr alenp,
			IntPtr rcodep,
			uint maxarr_len,
			IntPtr curelp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIBindByName", OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIBindByNameRef (stmtp, out bindpp, errhp, placeholder, placeh_len, ref valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIBindByNameRef (stmtp, out bindpp, errhp, placeholder, placeh_len, ref valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else
				return OciNativeCallsWindows.OCIBindByNameRef (stmtp, out bindpp, errhp, placeholder, placeh_len, ref valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
		}

		internal static int OCIBindByNameBytes (IntPtr stmtp,
			out IntPtr bindpp,
			IntPtr errhp,
			string placeholder,
			int placeh_len,
			byte[] valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr alenp,
			IntPtr rcodep,
			uint maxarr_len,
			IntPtr curelp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIBindByName", OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIBindByNameBytes (stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIBindByNameBytes (stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else
				return OciNativeCallsWindows.OCIBindByNameBytes (stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
		}

		internal static int OCIBindByPos (IntPtr stmtp,
			out IntPtr bindpp,
			IntPtr errhp,
			uint position,
			IntPtr valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr alenp,
			IntPtr rcodep,
			uint maxarr_len,
			IntPtr curelp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIBindByPos", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIBindByPos (stmtp, out bindpp, errhp, position, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIBindByPos (stmtp, out bindpp, errhp, position, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else
				return OciNativeCallsWindows.OCIBindByPos (stmtp, out bindpp, errhp, position, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
		}

		internal static int OCIBindByPosRef (IntPtr stmtp,
			out IntPtr bindpp,
			IntPtr errhp,
			uint position,
			ref IntPtr valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr alenp,
			IntPtr rcodep,
			uint maxarr_len,
			IntPtr curelp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIBindByPos", OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIBindByPosRef (stmtp, out bindpp, errhp, position, ref valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIBindByPosRef (stmtp, out bindpp, errhp, position, ref valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else
				return OciNativeCallsWindows.OCIBindByPosRef (stmtp, out bindpp, errhp, position, ref valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
				

		}

		internal static int OCIBindByPosBytes (IntPtr stmtp,
			out IntPtr bindpp,
			IntPtr errhp,
			uint position,
			byte[] valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr alenp,
			IntPtr rcodep,
			uint maxarr_len,
			IntPtr curelp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIBindByPos", OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIBindByPosBytes (stmtp, out bindpp, errhp, position, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIBindByPosBytes (stmtp, out bindpp, errhp, position, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
			else
				return OciNativeCallsWindows.OCIBindByPosBytes (stmtp, out bindpp, errhp, position, valuep,
					value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);

		}

		internal static void OCIDateTimeConstruct (IntPtr hndl,
			IntPtr err,
			IntPtr datetime,
			short year,
			byte month,
			byte day,
			byte hour,
			byte min,
			byte sec,
			uint fsec,
			byte[] timezone,
			uint timezone_length)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIDateTimeConstruct", OCI_DLL);
#endif
			if (OS == 1)
				OciNativeCallsLinux.OCIDateTimeConstruct (hndl, err, datetime, year, month, day, hour, min,
													sec, fsec, timezone, timezone_length);
			else if (OS == 2)
				OciNativeCallsOs.OCIDateTimeConstruct (hndl, err, datetime, year, month, day, hour, min,
													sec, fsec, timezone, timezone_length);
			else
				OciNativeCallsWindows.OCIDateTimeConstruct (hndl, err, datetime, year, month, day, hour, min,
													sec, fsec, timezone, timezone_length);


		}
		internal static void OCIDateTimeGetDate (IntPtr hndl,
			IntPtr err,
			IntPtr datetime,
			out short year,
			out byte month,
			out byte day)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIDateTimeGetDate", OCI_DLL);
#endif
			short retYear;
			byte retMonth;
			byte retDay;
			
			if (OS == 1)
				OciNativeCallsLinux.OCIDateTimeGetDate (hndl, err, datetime, out retYear, out retMonth, out retDay);
			else if (OS == 2)
				OciNativeCallsOs.OCIDateTimeGetDate (hndl, err, datetime, out retYear, out retMonth, out retDay);
			else
				OciNativeCallsWindows.OCIDateTimeGetDate (hndl, err, datetime, out retYear, out retMonth, out retDay);
			
			year =  retYear;
			month =retMonth;
			day = retDay;
		}

		internal static void OCIDateTimeGetTime (IntPtr hndl,
			IntPtr err,
			IntPtr datetime,
			out byte hour,
			out byte min,
			out byte sec,
			out uint fsec)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIDateTimeGetTime", OCI_DLL);
#endif
			byte retHour;
			byte retMin;
			byte retSec;
			uint retFsec;

			if (OS == 1)
				OciNativeCallsLinux.OCIDateTimeGetTime (hndl, err, datetime, out retHour, out retMin, out retSec, out retFsec);
			else if (OS == 2)
				OciNativeCallsOs.OCIDateTimeGetTime (hndl, err, datetime, out retHour, out retMin, out retSec, out retFsec);
			else
				OciNativeCallsWindows.OCIDateTimeGetTime (hndl, err, datetime, out retHour, out retMin, out retSec, out retFsec);

			hour = retHour;
			min = retMin;
			sec = retSec;
			fsec = retFsec;
		}
				
		internal static int OCIIntervalGetDaySecond (IntPtr hndl,
			IntPtr err,
			out int days,
			out int hours,
			out int mins,
			out int secs,
			out int fsec,
			IntPtr interval)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIIntervalGetDaySecond", OCI_DLL);
#endif
			int retDays;
			int retHours;
			int retMins;
			int retSecs;
			int retFsec;
			int rc;

			if (OS == 1)
				rc = OciNativeCallsLinux.OCIIntervalGetDaySecond (hndl, err, out retDays, out retHours, out retMins, out retSecs, out retFsec, interval);
			else if (OS == 2)
				rc = OciNativeCallsOs.OCIIntervalGetDaySecond (hndl, err, out retDays, out retHours, out retMins, out retSecs, out retFsec, interval);
			else
				rc = OciNativeCallsWindows.OCIIntervalGetDaySecond (hndl, err, out retDays, out retHours, out retMins, out retSecs, out retFsec, interval);

			
			days = retDays;
			hours = retHours;
			mins = retMins;
			secs = retSecs;
			fsec = retFsec;
			return rc;
		}

		internal static int OCIIntervalGetYearMonth (IntPtr hndl,
			IntPtr err,
			out int years,
			out int months,
			IntPtr interval)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIIntervalGetYearMonth", OCI_DLL);
#endif
			int retYears;
			int retMonths;
			int rc;

			if (OS == 1)
				rc = OciNativeCallsLinux.OCIIntervalGetYearMonth (hndl, err, out retYears, out retMonths, interval);
			else if (OS == 2)
				rc = OciNativeCallsOs.OCIIntervalGetYearMonth (hndl, err, out retYears, out retMonths, interval);
			else
				rc = OciNativeCallsWindows.OCIIntervalGetYearMonth (hndl, err, out retYears, out retMonths, interval);

			years = retYears;
			months = retMonths;
			return rc;
		}

		internal static int OCIDefineByPos (IntPtr stmtp,
			out IntPtr defnpp,
			IntPtr errhp,
			int position,
			IntPtr valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr rlenp,
			IntPtr rcodep,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIDefineByPos", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIDefineByPos (stmtp, out defnpp, errhp, position, valuep,
					value_sz, dty, indp, rlenp, rcodep, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIDefineByPos (stmtp, out defnpp, errhp, position, valuep,
					value_sz, dty, indp, rlenp, rcodep, mode);
			else
				return OciNativeCallsWindows.OCIDefineByPos (stmtp, out defnpp, errhp, position, valuep,
					value_sz, dty, indp, rlenp, rcodep, mode);


		}

		internal static int OCIDefineByPosPtr (IntPtr stmtp,
			out IntPtr defnpp,
			IntPtr errhp,
			int position,
			ref IntPtr valuep,
			int value_sz,
			[MarshalAs (UnmanagedType.U4)] OciDataType dty,
			IntPtr indp,
			IntPtr rlenp,
			IntPtr rcodep,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIDefineByPosPtr", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIDefineByPosPtr (stmtp, out defnpp, errhp, position, ref valuep,
					value_sz, dty, indp, rlenp, rcodep, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIDefineByPosPtr (stmtp, out defnpp, errhp, position, ref valuep,
					value_sz, dty, indp, rlenp, rcodep, mode);
			else
				return OciNativeCallsWindows.OCIDefineByPosPtr (stmtp, out defnpp, errhp, position, ref valuep,
					value_sz, dty, indp, rlenp, rcodep, mode);

			
		}

		internal static int OCIDescriptorFree (IntPtr hndlp,
			OciHandleType type)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, string.Format("OCIDescriptorFree ({0})", type), OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIDescriptorFree (hndlp, type);
			else if (OS == 2)
				return OciNativeCallsOs.OCIDescriptorFree (hndlp, type);
			else
				return OciNativeCallsWindows.OCIDescriptorFree (hndlp, type);
			
		}

		internal static int OCIEnvCreate (out IntPtr envhpp,
			OciEnvironmentMode mode,
			IntPtr ctxp,
			IntPtr malocfp,
			IntPtr ralocfp,
			IntPtr mfreep,
			int xtramem_sz,
			IntPtr usrmempp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIEnvCreate", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIEnvCreate (out envhpp, mode, ctxp, malocfp, ralocfp, mfreep,
					xtramem_sz, usrmempp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIEnvCreate (out envhpp, mode, ctxp, malocfp, ralocfp, mfreep,
					xtramem_sz, usrmempp);
			else
				return OciNativeCallsWindows.OCIEnvCreate (out envhpp, mode, ctxp, malocfp, ralocfp, mfreep,
					xtramem_sz, usrmempp);

		}

		internal static int OCICacheFree (IntPtr envhp,
			IntPtr svchp,
			IntPtr stmthp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCICacheFree", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCICacheFree (envhp, svchp, stmthp);
			else if (OS == 2)
				return OciNativeCallsOs.OCICacheFree (envhp, svchp, stmthp);
			else
				return OciNativeCallsWindows.OCICacheFree (envhp, svchp, stmthp);
			
		}

		internal static int OCIAttrGet (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			out IntPtr attributep,
			out int sizep,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIAttrGet", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrGet (trgthndlp, trghndltyp, out attributep, out sizep, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrGet (trgthndlp, trghndltyp, out attributep, out sizep, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrGet (trgthndlp, trghndltyp, out attributep, out sizep, attrtype, errhp);
			
		}

		internal static int OCIAttrGetSByte (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			out sbyte attributep,
			IntPtr sizep,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIAttrGetSByte", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrGetSByte (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrGetSByte (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrGetSByte (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			
		}

		internal static int OCIAttrGetByte (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			out byte attributep,
			IntPtr sizep,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIAttrGetByte", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrGetByte (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrGetByte (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrGetByte (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			
		}

		internal static int OCIAttrGetUInt16 (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			out ushort attributep,
			IntPtr sizep,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIAttrGetUInt16", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrGetUInt16 (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrGetUInt16 (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrGetUInt16 (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			
		}

		internal static int OCIAttrGetInt32 (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			out int attributep,
			IntPtr sizep,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIAttrGetInt32", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrGetInt32 (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrGetInt32 (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrGetInt32 (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			
		}

		internal static int OCIAttrGetIntPtr (IntPtr trgthndlp,
			OciHandleType trghndltyp,
			out IntPtr attributep,
			IntPtr sizep,
			OciAttributeType attrtype,
			IntPtr errhp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIAttrGetIntPtr", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIAttrGetIntPtr (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIAttrGetIntPtr (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
			else
				return OciNativeCallsWindows.OCIAttrGetIntPtr (trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);

		}

		internal static int OCIDescriptorAlloc (IntPtr parenth,
			out IntPtr hndlpp,
			OciHandleType type,
			int xtramem_sz,
			IntPtr usrmempp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIDescriptorAlloc", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIDescriptorAlloc (parenth, out hndlpp, type, xtramem_sz, usrmempp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIDescriptorAlloc (parenth, out hndlpp, type, xtramem_sz, usrmempp);
			else
				return OciNativeCallsWindows.OCIDescriptorAlloc (parenth, out hndlpp, type, xtramem_sz, usrmempp);
			
		}

		internal static int OCIHandleAlloc (IntPtr parenth,
			out IntPtr descpp,
			OciHandleType type,
			int xtramem_sz,
			IntPtr usrmempp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, string.Format("OCIHandleAlloc ({0})", type), OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIHandleAlloc (parenth, out descpp, type, xtramem_sz, usrmempp);
			else if (OS == 2)
				return OciNativeCallsOs.OCIHandleAlloc (parenth, out descpp, type, xtramem_sz, usrmempp);
			else
				return OciNativeCallsWindows.OCIHandleAlloc (parenth, out descpp, type, xtramem_sz, usrmempp);
		}

		internal static int OCIHandleFree (IntPtr hndlp,
			OciHandleType type)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, string.Format("OCIHandleFree ({0})", type), OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIHandleFree (hndlp, type);
			else if (OS == 2)
				return OciNativeCallsOs.OCIHandleFree (hndlp, type);
			else
				return OciNativeCallsWindows.OCIHandleFree (hndlp, type);			
		}

		internal static int OCILobClose (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobClose", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobClose (svchp, errhp, locp);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobClose (svchp, errhp, locp);
			else
				return OciNativeCallsWindows.OCILobClose (svchp, errhp, locp);
			
		}

		internal static int OCILobCopy (IntPtr svchp,
			IntPtr errhp,
			IntPtr dst_locp,
			IntPtr src_locp,
			uint amount,
			uint dst_offset,
			uint src_offset)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobCopy", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobCopy (svchp, errhp, dst_locp, src_locp, amount, dst_offset, src_offset);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobCopy (svchp, errhp, dst_locp, src_locp, amount, dst_offset, src_offset);
			else
				return OciNativeCallsWindows.OCILobCopy (svchp, errhp, dst_locp, src_locp, amount, dst_offset, src_offset);
			
		}

		internal static int OCILobErase (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			ref uint amount,
			uint offset)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobErase", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobErase (svchp, errhp, locp, ref amount, offset);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobErase (svchp, errhp, locp, ref amount, offset);
			else
				return OciNativeCallsWindows.OCILobErase (svchp, errhp, locp, ref amount, offset);

			//return OciNativeCalls.OCILobErase (svchp, errhp, locp, ref amount, offset);
		}

		internal static int OCILobGetChunkSize (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			out uint chunk_size)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobGetChunkSize", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobGetChunkSize (svchp, errhp, locp, out chunk_size);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobGetChunkSize (svchp, errhp, locp, out chunk_size);
			else
				return OciNativeCallsWindows.OCILobGetChunkSize (svchp, errhp, locp, out chunk_size);

			//return OciNativeCalls.OCILobGetChunkSize (svchp, errhp, locp, out chunk_size);
		}

		internal static int OCILobGetLength (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			out uint lenp)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobGetLength", OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCILobGetLength (svchp, errhp, locp, out lenp);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobGetLength (svchp, errhp, locp, out lenp);
			else
				return OciNativeCallsWindows.OCILobGetLength (svchp, errhp, locp, out lenp);

			//return OciNativeCalls.OCILobGetLength (svchp, errhp, locp, out lenp);
		}

		internal static int OCILobOpen (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			byte mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobOpen", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobOpen (svchp, errhp, locp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobOpen (svchp, errhp, locp, mode);
			else
				return OciNativeCallsWindows.OCILobOpen (svchp, errhp, locp, mode);
				
			//return OciNativeCalls.OCILobOpen (svchp, errhp, locp, mode);
		}

		internal static int OCILobRead (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			ref uint amtp,
			uint offset,
			byte[] bufp,
			uint bufl,
			IntPtr ctxp,
			IntPtr cbfp,
			ushort csid,
			byte csfrm)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobRead", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobRead (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
				ctxp, cbfp, csid, csfrm);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobRead (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
				ctxp, cbfp, csid, csfrm);
			else
				return OciNativeCallsWindows.OCILobRead (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
				ctxp, cbfp, csid, csfrm);

			// return OciNativeCalls.OCILobRead (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
			// 	ctxp, cbfp, csid, csfrm);
		}

		internal static int OCILobTrim (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			uint newlen)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobTrim", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobTrim (svchp, errhp, locp, newlen);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobTrim (svchp, errhp, locp, newlen);
			else
				return OciNativeCallsWindows.OCILobTrim (svchp, errhp, locp, newlen);

			//return OciNativeCalls.OCILobTrim (svchp, errhp, locp, newlen);
		}

		internal static int OCILobWrite (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			ref uint amtp,
			uint offset,
			byte[] bufp,
			uint bufl,
			byte piece,
			IntPtr ctxp,
			IntPtr cbfp,
			ushort csid,
			byte csfrm)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobWrite", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobWrite (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
				piece, ctxp, cbfp, csid, csfrm);
			else if (OS == 2)
				return OciNativeCallsOs.OCILobWrite (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
				piece, ctxp, cbfp, csid, csfrm);
			else
				return OciNativeCallsWindows.OCILobWrite (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
				piece, ctxp, cbfp, csid, csfrm);

			// return OciNativeCalls.OCILobWrite (svchp, errhp, locp, ref amtp, offset, bufp, bufl,
			// 	piece, ctxp, cbfp, csid, csfrm);
		}

		internal static int OCILobCharSetForm (IntPtr svchp, 
			IntPtr errhp,
			IntPtr locp,
			out byte csfrm)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCILobCharSetForm", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCILobCharSetForm (svchp, errhp, locp, out csfrm);			
			else if (OS == 2)
				return OciNativeCallsOs.OCILobCharSetForm (svchp, errhp, locp, out csfrm);			
			else
				return OciNativeCallsWindows.OCILobCharSetForm (svchp, errhp, locp, out csfrm);			

			//return OciNativeCalls.OCILobCharSetForm (svchp, errhp, locp, out csfrm);			
		}
		
		internal static int OCINlsGetInfo (IntPtr hndl,
			IntPtr errhp,
			ref byte[] bufp,
			uint buflen,
			ushort item)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCINlsGetInfo", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCINlsGetInfo (hndl, errhp, bufp, buflen, item);
			else if (OS == 2)
				return OciNativeCallsOs.OCINlsGetInfo (hndl, errhp, bufp, buflen, item);
			else
				return OciNativeCallsWindows.OCINlsGetInfo (hndl, errhp, bufp, buflen, item);

			//return OciNativeCalls.OCINlsGetInfo (hndl, errhp, bufp, buflen, item);
		}

		internal static int OCIServerAttach (IntPtr srvhp,
			IntPtr errhp,
			string dblink,
			int dblink_len,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIServerAttach", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIServerAttach (srvhp, errhp, dblink, dblink_len, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIServerAttach (srvhp, errhp, dblink, dblink_len, mode);
			else
				return OciNativeCallsWindows.OCIServerAttach (srvhp, errhp, dblink, dblink_len, mode);

			//return OciNativeCalls.OCIServerAttach (srvhp, errhp, dblink, dblink_len, mode);
		}

		internal static int OCIServerDetach (IntPtr srvhp,
			IntPtr errhp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIServerDetach", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIServerDetach (srvhp, errhp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIServerDetach (srvhp, errhp, mode);
			else
				return OciNativeCallsWindows.OCIServerDetach (srvhp, errhp, mode);

			//return OciNativeCalls.OCIServerDetach (srvhp, errhp, mode);
		}

		internal static int OCIServerVersion (IntPtr hndlp,
			IntPtr errhp,
			ref byte[] bufp,
			uint bufsz,
			OciHandleType hndltype)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIServerVersion", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIServerVersion (hndlp, errhp, bufp, bufsz, hndltype);
			else if (OS == 2)
				return OciNativeCallsOs.OCIServerVersion (hndlp, errhp, bufp, bufsz, hndltype);
			else
				return OciNativeCallsWindows.OCIServerVersion (hndlp, errhp, bufp, bufsz, hndltype);

			//return OciNativeCalls.OCIServerVersion (hndlp, errhp, bufp, bufsz, hndltype);
		}

		internal static int OCISessionBegin (IntPtr svchp,
			IntPtr errhp,
			IntPtr usrhp,
			OciCredentialType credt,
			OciSessionMode mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCISessionBegin", OCI_DLL);
#endif


			if (OS == 1)
				return OciNativeCallsLinux.OCISessionBegin (svchp, errhp, usrhp, credt, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCISessionBegin (svchp, errhp, usrhp, credt, mode);
			else
				return OciNativeCallsWindows.OCISessionBegin (svchp, errhp, usrhp, credt, mode);

			//return OciNativeCalls.OCISessionBegin (svchp, errhp, usrhp, credt, mode);
		}

		internal static int OCISessionEnd (IntPtr svchp,
			IntPtr errhp,
			IntPtr usrhp,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCISessionEnd", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCISessionEnd (svchp, errhp, usrhp, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCISessionEnd (svchp, errhp, usrhp, mode);
			else
				return OciNativeCallsWindows.OCISessionEnd (svchp, errhp, usrhp, mode);
				
			//return OciNativeCalls.OCISessionEnd (svchp, errhp, usrhp, mode);
		}

		internal static int OCIParamGet (IntPtr hndlp,
			OciHandleType htype,
			IntPtr errhp,
			out IntPtr parmdpp,
			int pos)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIParamGet", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIParamGet (hndlp, htype, errhp, out parmdpp, pos);
			else if (OS == 2)
				return OciNativeCallsOs.OCIParamGet (hndlp, htype, errhp, out parmdpp, pos);
			else
				return OciNativeCallsWindows.OCIParamGet (hndlp, htype, errhp, out parmdpp, pos);

			//return OciNativeCalls.OCIParamGet (hndlp, htype, errhp, out parmdpp, pos);
		}

		internal static int OCIStmtExecute (IntPtr svchp,
			IntPtr stmthp,
			IntPtr errhp,
			bool iters,
			uint rowoff,
			IntPtr snap_in,
			IntPtr snap_out,
			OciExecuteMode mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIStmtExecute", OCI_DLL);
#endif

			uint it = 0;
			if (iters == true)
				it = 1;

			if (OS == 1)
				return OciNativeCallsLinux.OCIStmtExecute (svchp, stmthp, errhp, it, rowoff,
				snap_in, snap_out, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIStmtExecute (svchp, stmthp, errhp, it, rowoff,
				snap_in, snap_out, mode);
			else
				return OciNativeCallsWindows.OCIStmtExecute (svchp, stmthp, errhp, it, rowoff,
				snap_in, snap_out, mode);

			// return OciNativeCalls.OCIStmtExecute (svchp, stmthp, errhp, it, rowoff,
			// 	snap_in, snap_out, mode);
		}

		internal static int OCIStmtFetch (IntPtr stmtp,
			IntPtr errhp,
			uint nrows,
			ushort orientation,
			uint mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIStmtFetch", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCIStmtFetch (stmtp, errhp, nrows, orientation, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIStmtFetch (stmtp, errhp, nrows, orientation, mode);
			else
				return OciNativeCallsWindows.OCIStmtFetch (stmtp, errhp, nrows, orientation, mode);

			//return OciNativeCalls.OCIStmtFetch (stmtp, errhp, nrows, orientation, mode);
		}


		internal static int OCIStmtPrepare (IntPtr stmthp,
			IntPtr errhp,
			byte [] stmt,
			int stmt_length,
			OciStatementLanguage language,
			OciStatementMode mode)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, string.Format("OCIStmtPrepare ({0})", System.Text.Encoding.UTF8.GetString(stmt)), OCI_DLL);
#endif
			if (OS == 1)
				return OciNativeCallsLinux.OCIStmtPrepare (stmthp, errhp, stmt, stmt_length, language, mode);
			else if (OS == 2)
				return OciNativeCallsOs.OCIStmtPrepare (stmthp, errhp, stmt, stmt_length, language, mode);
			else
				return OciNativeCallsWindows.OCIStmtPrepare (stmthp, errhp, stmt, stmt_length, language, mode);


			//return OciNativeCalls.OCIStmtPrepare (stmthp, errhp, stmt, stmt_length, language, mode);
		}

		internal static int OCITransCommit (IntPtr svchp,
			IntPtr errhp,
			uint flags)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCITransCommit", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCITransCommit (svchp, errhp, flags);
			else if (OS == 2)
				return OciNativeCallsOs.OCITransCommit (svchp, errhp, flags);
			else
				return OciNativeCallsWindows.OCITransCommit (svchp, errhp, flags);

			//return OciNativeCalls.OCITransCommit (svchp, errhp, flags);
		}

		internal static int OCITransRollback (IntPtr svchp,
			IntPtr errhp,
			uint flags)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCITransRollback", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCITransRollback (svchp, errhp, flags);
			else if (OS == 2)
				return OciNativeCallsOs.OCITransRollback (svchp, errhp, flags);
			else
				return OciNativeCallsWindows.OCITransRollback (svchp, errhp, flags);

			//return OciNativeCalls.OCITransRollback (svchp, errhp, flags);
		}

		internal static int OCITransStart (IntPtr svchp,
			IntPtr errhp,
			uint timeout,
			OciTransactionFlags flags)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCITransStart", OCI_DLL);
#endif

			if (OS == 1)
				return OciNativeCallsLinux.OCITransStart (svchp, errhp, timeout, flags);
			else if (OS == 2)
				return OciNativeCallsOs.OCITransStart (svchp, errhp, timeout, flags);
			else
				return OciNativeCallsWindows.OCITransStart (svchp, errhp, timeout, flags);

			//return OciNativeCalls.OCITransStart (svchp, errhp, timeout, flags);
		}

		internal static int OCICharSetToUnicode (
			IntPtr svchp,
			StringBuilder dst,
			byte [] src,
			out int rsize)
		{
			int rc;
			long retSize;

#if TRACE
			Trace.WriteLineIf(traceOci, "OCICharSetToUnicode", OCI_DLL);
#endif


			if (OS == 1)
				rc = OciNativeCallsLinux.OCICharSetToUnicode (svchp, dst,
					(dst != null ? dst.Capacity : 0), 
					src, src.Length, out retSize);
			else if (OS == 2)
				rc = OciNativeCallsOs.OCICharSetToUnicode (svchp, dst,
					(dst != null ? dst.Capacity : 0), 
					src, src.Length, out retSize);
			else
				rc = OciNativeCallsWindows.OCICharSetToUnicode (svchp, dst,
					(dst != null ? dst.Capacity : 0), 
					src, src.Length, out retSize);
				
			// rc = OciNativeCalls.OCICharSetToUnicode (svchp, dst,
			// 			(dst != null ? dst.Capacity : 0), 
			// 			src, src.Length, out retSize);
			rsize = (int) retSize;
			return(rc);
		}

		internal static int OCIUnicodeToCharSet (
			IntPtr svchp,
			byte [] dst,
			string src,
			out int rsize)
		{
			int rc;
			long retSize;

#if TRACE
			Trace.WriteLineIf(traceOci, "OCIUnicodeToCharSet", OCI_DLL);
#endif

			if (OS == 1)
				rc = OciNativeCallsLinux.OCIUnicodeToCharSet (svchp, dst,
						(dst != null ? dst.Length : 0), 
						src, src.Length, out retSize);
			else if (OS == 2)
				rc = OciNativeCallsOs.OCIUnicodeToCharSet (svchp, dst,
						(dst != null ? dst.Length : 0), 
						src, src.Length, out retSize);
			else
				rc = OciNativeCallsWindows.OCIUnicodeToCharSet (svchp, dst,
						(dst != null ? dst.Length : 0), 
						src, src.Length, out retSize);


			// rc = OciNativeCalls.OCIUnicodeToCharSet (svchp, dst,
			// 		(dst != null ? dst.Length : 0), 
			// 		src, src.Length, out retSize);
			rsize = (int) retSize;
			return(rc);
		}

		internal static int OCIDateTimeCheck (IntPtr hndl,
			IntPtr err, IntPtr date, out uint valid)
		{
#if TRACE
			Trace.WriteLineIf(traceOci, "OCIDateTimeCheck", OCI_DLL);
#endif
			uint retValid;
			int rc;
			
			if (OS == 1)
				rc = OciNativeCallsLinux.OCIDateTimeCheck(hndl, err, date, out retValid);
			else if (OS == 2)
				rc = OciNativeCallsOs.OCIDateTimeCheck(hndl, err, date, out retValid);
			else
				rc = OciNativeCallsWindows.OCIDateTimeCheck(hndl, err, date, out retValid);

			//rc = OciNativeCalls.OCIDateTimeCheck(hndl, err, date, out retValid);
			
			valid = retValid;
			return rc;
		}

#endregion

		#region AllocateClear

// #if OCI_WINDOWS
//         private static bool IsUnix = false;
// #else 
// 		private static bool IsUnix = true;
// #endif
		private static bool IsUnix = (int)Environment.OSVersion.Platform == 4 || 
									 (int)Environment.OSVersion.Platform == 128 || 
									 (int)Environment.OSVersion.Platform == 6;

		[DllImport("libc")]
		private static extern IntPtr calloc (int nmemb, int size);

		private const int GMEM_ZEROINIT = 0x40;

		[DllImport("kernel32")]
		private static extern IntPtr GlobalAlloc (int flags, int bytes);

		//http://download-uk.oracle.com/docs/cd/B14117_01/appdev.101/b10779/oci05bnd.htm#423147
		internal static IntPtr AllocateClear (int cb)
		{
			if (IsUnix) {
				return calloc (1, cb);
			} else {
				return GlobalAlloc (GMEM_ZEROINIT, cb);
			}
		}

		#endregion AllocateClear
	}
}

