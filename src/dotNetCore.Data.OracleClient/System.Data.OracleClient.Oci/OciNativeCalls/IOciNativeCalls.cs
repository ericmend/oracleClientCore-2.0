using System;
using System.Data.OracleClient.Oci;
using System.Runtime.InteropServices;
using System.Text;

namespace dotNetCore.Data.OracleClient.System.Data.OracleClient.Oci.OciNativeCalls
{
    internal interface IOciNativeCalls
    {
        string getOCI_DLL();
         
		int OCIAttrSet (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			IntPtr attributep,
			uint size,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);

		int OCIAttrSetString (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			string attributep,
			uint size,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);

// #if ORACLE_DATA_ACCESS
// 			int OCIPasswordChange (IntPtr svchp, 
// 				IntPtr errhp,
// 				byte [] user_name, 
// 				[MarshalAs (UnmanagedType.U4)] int usernm_len,
// 				byte [] opasswd,
// 				[MarshalAs (UnmanagedType.U4)] int opasswd_len,
// 				byte [] npasswd,
// 				[MarshalAs (UnmanagedType.U4)] int npasswd_len,
// 				[MarshalAs (UnmanagedType.U4)] uint mode);
// #endif

		int OCIErrorGet (IntPtr hndlp,
			uint recordno,
			IntPtr sqlstate,
			out int errcodep,
			IntPtr bufp,
			uint bufsize,
			[MarshalAs (UnmanagedType.U4)] OciHandleType type);


		int OCIBindByName (IntPtr stmtp,
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


		int OCIBindByNameRef (IntPtr stmtp,
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


		int OCIBindByNameBytes (IntPtr stmtp,
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


		int OCIBindByPos (IntPtr stmtp,
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


		int OCIBindByPosBytes (IntPtr stmtp,
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


		int OCIBindByPosRef (IntPtr stmtp,
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


		int OCIDateTimeFromText (IntPtr hndl,
			IntPtr errhp, [In][Out] byte[] date_str, uint dstr_length,
			[In][Out] byte[] fmt, uint fmt_length,
			[In][Out] byte[] lang_name, uint lang_length, IntPtr datetime);
		

		int OCIDefineByPos (IntPtr stmtp,
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


		int OCIDefineByPosPtr (IntPtr stmtp,
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


		int OCIDescriptorFree (IntPtr hndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType type);


		int OCIEnvCreate (out IntPtr envhpp,
			[MarshalAs (UnmanagedType.U4)] OciEnvironmentMode mode,
			IntPtr ctxp,
			IntPtr malocfp,
			IntPtr ralocfp,
			IntPtr mfreep,
			int xtramem_sz,
			IntPtr usrmempp);


		int OCICacheFree (IntPtr envhp,
			IntPtr errhp,
			IntPtr stmthp);


		int OCIAttrGet (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			out IntPtr attributep,
			out int sizep,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);


		int OCIAttrGetSByte (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			out sbyte attributep,
			IntPtr sizep,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);


		int OCIAttrGetByte (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			out byte attributep,
			IntPtr sizep,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);


		int OCIAttrGetUInt16 (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			out ushort attributep,
			IntPtr sizep,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);


		int OCIAttrGetInt32 (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			out int attributep,
			IntPtr sizep,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);


		int OCIAttrGetIntPtr (IntPtr trgthndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType trghndltyp,
			out IntPtr attributep,
			IntPtr sizep,
			[MarshalAs (UnmanagedType.U4)] OciAttributeType attrtype,
			IntPtr errhp);


		int OCIDescriptorAlloc (IntPtr parenth,
			out IntPtr hndlpp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType type,
			int xtramem_sz,
			IntPtr usrmempp);


		int OCIHandleAlloc (IntPtr parenth,
			out IntPtr descpp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType type,
			int xtramem_sz,
			IntPtr usrmempp);


		int OCIHandleFree (IntPtr hndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType type);


		int OCILobClose (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp);


		int OCILobCopy (IntPtr svchp,
			IntPtr errhp,
			IntPtr dst_locp,
			IntPtr src_locp,
			uint amount,
			uint dst_offset,
			uint src_offset);


		int OCILobErase (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			ref uint amount,
			uint offset);


		int OCILobGetChunkSize (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			out uint chunk_size);


		int OCILobGetLength (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			out uint lenp);


		int OCILobOpen (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			byte mode);


		int OCILobRead (IntPtr svchp,
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


		int OCILobTrim (IntPtr svchp,
			IntPtr errhp,
			IntPtr locp,
			uint newlen);


		int OCILobWrite (IntPtr svchp,
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


		int OCILobCharSetForm (IntPtr svchp, 
			IntPtr errhp,
			IntPtr locp,
			out byte csfrm);

		
		int OCINlsGetInfo (IntPtr hndl,
			IntPtr errhp,
			[In][Out] byte[] bufp,
			uint buflen,
			ushort item);


		int OCIServerAttach (IntPtr srvhp,
			IntPtr errhp,
			string dblink,
			[MarshalAs (UnmanagedType.U4)] int dblink_len,
			uint mode);


		int OCIServerDetach (IntPtr srvhp,
			IntPtr errhp,
			uint mode);


		int OCIServerVersion (IntPtr hndlp,
			IntPtr errhp,
			[In][Out] byte[] bufp,
			uint bufsz,
			[MarshalAs (UnmanagedType.U4)] OciHandleType type);


		int OCISessionBegin (IntPtr svchp,
			IntPtr errhp,
			IntPtr usrhp,
			[MarshalAs (UnmanagedType.U4)] OciCredentialType credt,
			[MarshalAs (UnmanagedType.U4)] OciSessionMode mode);


		int OCISessionEnd (IntPtr svchp,
			IntPtr errhp,
			IntPtr usrhp,
			uint mode);


		int OCIParamGet (IntPtr hndlp,
			[MarshalAs (UnmanagedType.U4)] OciHandleType htype,
			IntPtr errhp,
			out IntPtr parmdpp,
			[MarshalAs (UnmanagedType.U4)] int pos);


		int OCIStmtExecute (IntPtr svchp,
			IntPtr stmthp,
			IntPtr errhp,
			[MarshalAs (UnmanagedType.U4)] uint iters,
			uint rowoff,
			IntPtr snap_in,
			IntPtr snap_out,
			[MarshalAs (UnmanagedType.U4)] OciExecuteMode mode);


		int OCIStmtFetch (IntPtr stmtp,
			IntPtr errhp,
			uint nrows,
			ushort orientation,
			uint mode);



		int OCIStmtPrepare (IntPtr stmthp,
			IntPtr errhp,
			byte [] stmt,
			[MarshalAs (UnmanagedType.U4)] int stmt_length,
			[MarshalAs (UnmanagedType.U4)] OciStatementLanguage language,
			[MarshalAs (UnmanagedType.U4)] OciStatementMode mode);


		int OCITransCommit (IntPtr svchp,
			IntPtr errhp,
			uint flags);


		int OCITransRollback (IntPtr svchp,
			IntPtr errhp,
			uint flags);


		int OCITransStart (IntPtr svchp,
			IntPtr errhp,
			uint timeout,
			[MarshalAs (UnmanagedType.U4)] OciTransactionFlags flags);


		int OCICharSetToUnicode (
			IntPtr svchp,
			[MarshalAs (UnmanagedType.LPWStr)] StringBuilder dst,
			[MarshalAs (UnmanagedType.I4)] int dstlen,
			byte [] src,
			[MarshalAs (UnmanagedType.I4)] int srclen,
			out long rsize);


		int OCIUnicodeToCharSet (
			IntPtr svchp,
			byte [] dst,
			[MarshalAs (UnmanagedType.I4)] int dstlen,
			[MarshalAs (UnmanagedType.LPWStr)] string src,
			[MarshalAs (UnmanagedType.I4)] int srclen,
			out long rsize);


		void OCIDateTimeConstruct (IntPtr hndl,
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


		void OCIDateTimeGetDate (IntPtr hndl,
			IntPtr err,
			IntPtr datetime,
			out short year,
			out byte month,
			out byte day);

		void OCIDateTimeGetTime (IntPtr hndl,
			IntPtr err,
			IntPtr datetime,
			out byte hour,
			out byte min,
			out byte sec,
			out uint fsec);

				
		int OCIIntervalGetDaySecond (IntPtr hndl,
			IntPtr err,
			out int days,
			out int hours,
			out int mins,
			out int secs,
			out int fsec,
			IntPtr interval);


		int OCIIntervalGetYearMonth (IntPtr hndl,
			IntPtr err,
			out int years,
			out int months,
			IntPtr interval);

		int OCIDateTimeCheck (IntPtr hndl,
			IntPtr err, IntPtr date, out uint valid);

        IntPtr AllocateClear (int cb);
    }
}