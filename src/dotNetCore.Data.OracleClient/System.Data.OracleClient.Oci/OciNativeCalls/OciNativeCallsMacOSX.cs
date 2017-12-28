
//#define ORACLE_DATA_ACCESS

using System;
using System.Data.OracleClient.Oci;
using System.Runtime.InteropServices;
using System.Text;

namespace dotNetCore.Data.OracleClient.System.Data.OracleClient.Oci.OciNativeCalls
{
    internal sealed class OciNativeCallsMacOSX : IOciNativeCalls
    {
        internal OciNativeCallsMacOSX() { }

        public string getOCI_DLL()
        {
            return NativeCalls.OCI_DLL;
        }

        #region native calls Windows
        private sealed class NativeCalls
        {
            private NativeCalls()
            { }
            public const string KERNEL = "libc";
            public const string OCI_DLL = "libclntsh.dynlib";

            [DllImport(KERNEL)]
            public static extern IntPtr calloc(int nmemb, int size);

            [DllImport(OCI_DLL, EntryPoint = "OCIAttrSet")]
            public static extern int OCIAttrSet(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                IntPtr attributep,
                uint size,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            [DllImport(OCI_DLL, EntryPoint = "OCIAttrSet")]
            public static extern int OCIAttrSetString(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                string attributep,
                uint size,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            // #if ORACLE_DATA_ACCESS
            // 			[DllImport (OCI_DLL, EntryPoint = "OCIPasswordChange")]
            // 			public static extern int OCIPasswordChange (IntPtr svchp, 
            // 				IntPtr errhp,
            // 				byte [] user_name, 
            // 				[MarshalAs (UnmanagedType.U4)] int usernm_len,
            // 				byte [] opasswd,
            // 				[MarshalAs (UnmanagedType.U4)] int opasswd_len,
            // 				byte [] npasswd,
            // 				[MarshalAs (UnmanagedType.U4)] int npasswd_len,
            // 				[MarshalAs (UnmanagedType.U4)] uint mode);
            // #endif

            [DllImport(OCI_DLL)]
            public static extern int OCIErrorGet(IntPtr hndlp,
                uint recordno,
                IntPtr sqlstate,
                out int errcodep,
                IntPtr bufp,
                uint bufsize,
                [MarshalAs(UnmanagedType.U4)] OciHandleType type);

            [DllImport(OCI_DLL, EntryPoint = "OCIBindByName")]
            public static extern int OCIBindByName(IntPtr stmtp,
                out IntPtr bindpp,
                IntPtr errhp,
                string placeholder,
                int placeh_len,
                IntPtr valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr alenp,
                IntPtr rcodep,
                uint maxarr_len,
                IntPtr curelp,
                uint mode);

            [DllImport(OCI_DLL, EntryPoint = "OCIBindByName")]
            public static extern int OCIBindByNameRef(IntPtr stmtp,
                out IntPtr bindpp,
                IntPtr errhp,
                string placeholder,
                int placeh_len,
                ref IntPtr valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr alenp,
                IntPtr rcodep,
                uint maxarr_len,
                IntPtr curelp,
                uint mode);

            [DllImport(OCI_DLL, EntryPoint = "OCIBindByName")]
            public static extern int OCIBindByNameBytes(IntPtr stmtp,
                out IntPtr bindpp,
                IntPtr errhp,
                string placeholder,
                int placeh_len,
                byte[] valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr alenp,
                IntPtr rcodep,
                uint maxarr_len,
                IntPtr curelp,
                uint mode);

            [DllImport(OCI_DLL, EntryPoint = "OCIBindByPos")]
            public static extern int OCIBindByPos(IntPtr stmtp,
                out IntPtr bindpp,
                IntPtr errhp,
                uint position,
                IntPtr valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr alenp,
                IntPtr rcodep,
                uint maxarr_len,
                IntPtr curelp,
                uint mode);

            [DllImport(OCI_DLL, EntryPoint = "OCIBindByPos")]
            public static extern int OCIBindByPosBytes(IntPtr stmtp,
                out IntPtr bindpp,
                IntPtr errhp,
                uint position,
                byte[] valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr alenp,
                IntPtr rcodep,
                uint maxarr_len,
                IntPtr curelp,
                uint mode);

            [DllImport(OCI_DLL, EntryPoint = "OCIBindByPos")]
            public static extern int OCIBindByPosRef(IntPtr stmtp,
                out IntPtr bindpp,
                IntPtr errhp,
                uint position,
                ref IntPtr valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr alenp,
                IntPtr rcodep,
                uint maxarr_len,
                IntPtr curelp,
                uint mode);

            [DllImport(OCI_DLL)]
            public static extern int OCIDateTimeFromText(IntPtr hndl,
                IntPtr errhp, [In][Out] byte[] date_str, uint dstr_length,
                [In][Out] byte[] fmt, uint fmt_length,
                [In][Out] byte[] lang_name, uint lang_length, IntPtr datetime);

            [DllImport(OCI_DLL)]
            public static extern int OCIDefineByPos(IntPtr stmtp,
                out IntPtr defnpp,
                IntPtr errhp,
                [MarshalAs(UnmanagedType.U4)] int position,
                IntPtr valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr rlenp,
                IntPtr rcodep,
                uint mode);

            [DllImport(OCI_DLL, EntryPoint = "OCIDefineByPos")]
            public static extern int OCIDefineByPosPtr(IntPtr stmtp,
                out IntPtr defnpp,
                IntPtr errhp,
                [MarshalAs(UnmanagedType.U4)] int position,
                ref IntPtr valuep,
                int value_sz,
                [MarshalAs(UnmanagedType.U4)] OciDataType dty,
                IntPtr indp,
                IntPtr rlenp,
                IntPtr rcodep,
                uint mode);

            [DllImport(OCI_DLL)]
            public static extern int OCIDescriptorFree(IntPtr hndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType type);

            [DllImport(OCI_DLL)]
            public static extern int OCIEnvCreate(out IntPtr envhpp,
                [MarshalAs(UnmanagedType.U4)] OciEnvironmentMode mode,
                IntPtr ctxp,
                IntPtr malocfp,
                IntPtr ralocfp,
                IntPtr mfreep,
                int xtramem_sz,
                IntPtr usrmempp);

            [DllImport(OCI_DLL)]
            public static extern int OCICacheFree(IntPtr envhp,
                IntPtr errhp,
                IntPtr stmthp);

            [DllImport(OCI_DLL)]
            public static extern int OCIAttrGet(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                out IntPtr attributep,
                out int sizep,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            [DllImport(OCI_DLL, EntryPoint = "OCIAttrGet")]
            public static extern int OCIAttrGetSByte(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                out sbyte attributep,
                IntPtr sizep,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            [DllImport(OCI_DLL, EntryPoint = "OCIAttrGet")]
            public static extern int OCIAttrGetByte(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                out byte attributep,
                IntPtr sizep,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            [DllImport(OCI_DLL, EntryPoint = "OCIAttrGet")]
            public static extern int OCIAttrGetUInt16(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                out ushort attributep,
                IntPtr sizep,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            [DllImport(OCI_DLL, EntryPoint = "OCIAttrGet")]
            public static extern int OCIAttrGetInt32(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                out int attributep,
                IntPtr sizep,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            [DllImport(OCI_DLL, EntryPoint = "OCIAttrGet")]
            public static extern int OCIAttrGetIntPtr(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                out IntPtr attributep,
                IntPtr sizep,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp);

            [DllImport(OCI_DLL)]
            public static extern int OCIDescriptorAlloc(IntPtr parenth,
                out IntPtr hndlpp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType type,
                int xtramem_sz,
                IntPtr usrmempp);

            [DllImport(OCI_DLL)]
            public static extern int OCIHandleAlloc(IntPtr parenth,
                out IntPtr descpp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType type,
                int xtramem_sz,
                IntPtr usrmempp);

            [DllImport(OCI_DLL)]
            public static extern int OCIHandleFree(IntPtr hndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType type);

            [DllImport(OCI_DLL)]
            public static extern int OCILobClose(IntPtr svchp,
                IntPtr errhp,
                IntPtr locp);

            [DllImport(OCI_DLL)]
            public static extern int OCILobCopy(IntPtr svchp,
                IntPtr errhp,
                IntPtr dst_locp,
                IntPtr src_locp,
                uint amount,
                uint dst_offset,
                uint src_offset);

            [DllImport(OCI_DLL)]
            public static extern int OCILobErase(IntPtr svchp,
                IntPtr errhp,
                IntPtr locp,
                ref uint amount,
                uint offset);

            [DllImport(OCI_DLL)]
            public static extern int OCILobGetChunkSize(IntPtr svchp,
                IntPtr errhp,
                IntPtr locp,
                out uint chunk_size);

            [DllImport(OCI_DLL)]
            public static extern int OCILobGetLength(IntPtr svchp,
                IntPtr errhp,
                IntPtr locp,
                out uint lenp);

            [DllImport(OCI_DLL)]
            public static extern int OCILobOpen(IntPtr svchp,
                IntPtr errhp,
                IntPtr locp,
                byte mode);

            [DllImport(OCI_DLL)]
            public static extern int OCILobRead(IntPtr svchp,
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

            [DllImport(OCI_DLL)]
            public static extern int OCILobTrim(IntPtr svchp,
                IntPtr errhp,
                IntPtr locp,
                uint newlen);

            [DllImport(OCI_DLL)]
            public static extern int OCILobWrite(IntPtr svchp,
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

            [DllImport(OCI_DLL)]
            public static extern int OCILobCharSetForm(IntPtr svchp,
                IntPtr errhp,
                IntPtr locp,
                out byte csfrm);

            [DllImport(OCI_DLL)]
            public static extern int OCINlsGetInfo(IntPtr hndl,
                IntPtr errhp,
                [In][Out] byte[] bufp,
                uint buflen,
                ushort item);

            [DllImport(OCI_DLL)]
            public static extern int OCIServerAttach(IntPtr srvhp,
                IntPtr errhp,
                string dblink,
                [MarshalAs(UnmanagedType.U4)] int dblink_len,
                uint mode);

            [DllImport(OCI_DLL)]
            public static extern int OCIServerDetach(IntPtr srvhp,
                IntPtr errhp,
                uint mode);

            [DllImport(OCI_DLL)]
            public static extern int OCIServerVersion(IntPtr hndlp,
                IntPtr errhp,
                [In][Out] byte[] bufp,
                uint bufsz,
                [MarshalAs(UnmanagedType.U4)] OciHandleType type);

            [DllImport(OCI_DLL)]
            public static extern int OCISessionBegin(IntPtr svchp,
                IntPtr errhp,
                IntPtr usrhp,
                [MarshalAs(UnmanagedType.U4)] OciCredentialType credt,
                [MarshalAs(UnmanagedType.U4)] OciSessionMode mode);

            [DllImport(OCI_DLL)]
            public static extern int OCISessionEnd(IntPtr svchp,
                IntPtr errhp,
                IntPtr usrhp,
                uint mode);

            [DllImport(OCI_DLL)]
            public static extern int OCIParamGet(IntPtr hndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType htype,
                IntPtr errhp,
                out IntPtr parmdpp,
                [MarshalAs(UnmanagedType.U4)] int pos);

            [DllImport(OCI_DLL)]
            public static extern int OCIStmtExecute(IntPtr svchp,
                IntPtr stmthp,
                IntPtr errhp,
                [MarshalAs(UnmanagedType.U4)] uint iters,
                uint rowoff,
                IntPtr snap_in,
                IntPtr snap_out,
                [MarshalAs(UnmanagedType.U4)] OciExecuteMode mode);

            [DllImport(OCI_DLL)]
            public static extern int OCIStmtFetch(IntPtr stmtp,
                IntPtr errhp,
                uint nrows,
                ushort orientation,
                uint mode);


            [DllImport(OCI_DLL)]
            public static extern int OCIStmtPrepare(IntPtr stmthp,
                IntPtr errhp,
                byte[] stmt,
                [MarshalAs(UnmanagedType.U4)] int stmt_length,
                [MarshalAs(UnmanagedType.U4)] OciStatementLanguage language,
                [MarshalAs(UnmanagedType.U4)] OciStatementMode mode);

            [DllImport(OCI_DLL)]
            public static extern int OCITransCommit(IntPtr svchp,
                IntPtr errhp,
                uint flags);

            [DllImport(OCI_DLL)]
            public static extern int OCITransRollback(IntPtr svchp,
                IntPtr errhp,
                uint flags);

            [DllImport(OCI_DLL)]
            public static extern int OCITransStart(IntPtr svchp,
                IntPtr errhp,
                uint timeout,
                [MarshalAs(UnmanagedType.U4)] OciTransactionFlags flags);

            [DllImport(OCI_DLL)]
            public static extern int OCICharSetToUnicode(
                IntPtr svchp,
                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dst,
                [MarshalAs(UnmanagedType.I4)] int dstlen,
                byte[] src,
                [MarshalAs(UnmanagedType.I4)] int srclen,
                out long rsize);

            [DllImport(OCI_DLL)]
            public static extern int OCIUnicodeToCharSet(
                IntPtr svchp,
                byte[] dst,
                [MarshalAs(UnmanagedType.I4)] int dstlen,
                [MarshalAs(UnmanagedType.LPWStr)] string src,
                [MarshalAs(UnmanagedType.I4)] int srclen,
                out long rsize);

            [DllImport(OCI_DLL)]
            public static extern void OCIDateTimeConstruct(IntPtr hndl,
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
            [DllImport(OCI_DLL)]
            public static extern void OCIDateTimeGetDate(IntPtr hndl,
                            IntPtr err,
                            IntPtr datetime,
                            out short year,
                            out byte month,
                            out byte day);
            [DllImport(OCI_DLL)]
            public static extern void OCIDateTimeGetTime(IntPtr hndl,
                IntPtr err,
                IntPtr datetime,
                out byte hour,
                out byte min,
                out byte sec,
                out uint fsec);

            [DllImport(OCI_DLL)]
            public static extern int OCIIntervalGetDaySecond(IntPtr hndl,
                IntPtr err,
                out int days,
                out int hours,
                out int mins,
                out int secs,
                out int fsec,
                IntPtr interval);

            [DllImport(OCI_DLL)]
            public static extern int OCIIntervalGetYearMonth(IntPtr hndl,
                IntPtr err,
                out int years,
                out int months,
                IntPtr interval);
            [DllImport(OCI_DLL)]
            public static extern int OCIDateTimeCheck(IntPtr hndl,
                IntPtr err, IntPtr date, out uint valid);
        }
        #endregion

        #region OCI call wrappers
        public int OCIAttrSet(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            IntPtr attributep,
            uint size,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrSet(trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
        }

        public int OCIAttrSetString(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            string attributep,
            uint size,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrSetString(trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
        }

        // #if ORACLE_DATA_ACCESS
        // 			public int OCIPasswordChange (IntPtr svchp, 
        // 				IntPtr errhp,
        // 				byte [] user_name, 
        // 				[MarshalAs (UnmanagedType.U4)] int usernm_len,
        // 				byte [] opasswd,
        // 				[MarshalAs (UnmanagedType.U4)] int opasswd_len,
        // 				byte [] npasswd,
        // 				[MarshalAs (UnmanagedType.U4)] int npasswd_len,
        // 				[MarshalAs (UnmanagedType.U4)] uint mode)
        //             {
        //                 return NativeCalls.OCIPasswordChange (svchp, errhp, user_name, usernm_len, opasswd, opasswd_len, npasswd, npasswd_len, mode);
        //             }
        // #endif

        public int OCIErrorGet(IntPtr hndlp,
            uint recordno,
            IntPtr sqlstate,
            out int errcodep,
            IntPtr bufp,
            uint bufsize,
            [MarshalAs(UnmanagedType.U4)] OciHandleType type)
        {
            return NativeCalls.OCIErrorGet(hndlp, recordno, sqlstate, out errcodep, bufp, bufsize, type);
        }


        public int OCIBindByName(IntPtr stmtp,
            out IntPtr bindpp,
            IntPtr errhp,
            string placeholder,
            int placeh_len,
            IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr alenp,
            IntPtr rcodep,
            uint maxarr_len,
            IntPtr curelp,
            uint mode)
        {
            return NativeCalls.OCIBindByName(stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
                                                 value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
        }


        public int OCIBindByNameRef(IntPtr stmtp,
            out IntPtr bindpp,
            IntPtr errhp,
            string placeholder,
            int placeh_len,
            ref IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr alenp,
            IntPtr rcodep,
            uint maxarr_len,
            IntPtr curelp,
            uint mode)
        {
            return NativeCalls.OCIBindByNameRef(stmtp, out bindpp, errhp, placeholder, placeh_len, ref valuep,
                                                    value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
        }


        public int OCIBindByNameBytes(IntPtr stmtp,
            out IntPtr bindpp,
            IntPtr errhp,
            string placeholder,
            int placeh_len,
            byte[] valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr alenp,
            IntPtr rcodep,
            uint maxarr_len,
            IntPtr curelp,
            uint mode)
        {
            return NativeCalls.OCIBindByNameBytes(stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
                                                     value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
        }


        public int OCIBindByPos(IntPtr stmtp,
            out IntPtr bindpp,
            IntPtr errhp,
            uint position,
            IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr alenp,
            IntPtr rcodep,
            uint maxarr_len,
            IntPtr curelp,
            uint mode)
        {
            return NativeCalls.OCIBindByPos(stmtp, out bindpp, errhp, position, valuep, value_sz, dty, indp,
                                                alenp, rcodep, maxarr_len, curelp, mode);
        }


        public int OCIBindByPosBytes(IntPtr stmtp,
            out IntPtr bindpp,
            IntPtr errhp,
            uint position,
            byte[] valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr alenp,
            IntPtr rcodep,
            uint maxarr_len,
            IntPtr curelp,
            uint mode)
        {
            return NativeCalls.OCIBindByPosBytes(stmtp, out bindpp, errhp, position, valuep, value_sz, dty,
                                                     indp, alenp, rcodep, maxarr_len, curelp, mode);
        }


        public int OCIBindByPosRef(IntPtr stmtp,
            out IntPtr bindpp,
            IntPtr errhp,
            uint position,
            ref IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr alenp,
            IntPtr rcodep,
            uint maxarr_len,
            IntPtr curelp,
            uint mode)
        {
            return NativeCalls.OCIBindByPosRef(stmtp, out bindpp, errhp, position, ref valuep, value_sz, dty,
                                                   indp, alenp, rcodep, maxarr_len, curelp, mode);
        }


        public int OCIDateTimeFromText(IntPtr hndl,
            IntPtr errhp, [In][Out] byte[] date_str, uint dstr_length,
            [In][Out] byte[] fmt, uint fmt_length,
            [In][Out] byte[] lang_name, uint lang_length, IntPtr datetime)
        {
            return NativeCalls.OCIDateTimeFromText(hndl, errhp, date_str, dstr_length, fmt, fmt_length, lang_name, lang_length, datetime);
        }


        public int OCIDefineByPos(IntPtr stmtp,
            out IntPtr defnpp,
            IntPtr errhp,
            [MarshalAs(UnmanagedType.U4)] int position,
            IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr rlenp,
            IntPtr rcodep,
            uint mode)
        {
            return NativeCalls.OCIDefineByPos(stmtp, out defnpp, errhp, position, valuep, value_sz, dty,
                                                  indp, rlenp, rcodep, mode);
        }


        public int OCIDefineByPosPtr(IntPtr stmtp,
            out IntPtr defnpp,
            IntPtr errhp,
            [MarshalAs(UnmanagedType.U4)] int position,
            ref IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr rlenp,
            IntPtr rcodep,
            uint mode)
        {
            return NativeCalls.OCIDefineByPosPtr(stmtp, out defnpp, errhp, position, ref valuep, value_sz, dty,
                                                     indp, rlenp, rcodep, mode);
        }


        public int OCIDescriptorFree(IntPtr hndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType type)
        {
            return NativeCalls.OCIDescriptorFree(hndlp, type);
        }


        public int OCIEnvCreate(out IntPtr envhpp,
            [MarshalAs(UnmanagedType.U4)] OciEnvironmentMode mode,
            IntPtr ctxp,
            IntPtr malocfp,
            IntPtr ralocfp,
            IntPtr mfreep,
            int xtramem_sz,
            IntPtr usrmempp)
        {
            return NativeCalls.OCIEnvCreate(out envhpp, mode, ctxp, malocfp, ralocfp, mfreep, xtramem_sz, usrmempp);
        }


        public int OCICacheFree(IntPtr envhp,
            IntPtr errhp,
            IntPtr stmthp)
        {
            return NativeCalls.OCICacheFree(envhp, errhp, stmthp);
        }


        public int OCIAttrGet(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            out IntPtr attributep,
            out int sizep,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrGet(trgthndlp, trghndltyp, out attributep, out sizep, attrtype, errhp);
        }


        public int OCIAttrGetSByte(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            out sbyte attributep,
            IntPtr sizep,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrGetSByte(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
        }


        public int OCIAttrGetByte(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            out byte attributep,
            IntPtr sizep,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrGetByte(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
        }


        public int OCIAttrGetUInt16(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            out ushort attributep,
            IntPtr sizep,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrGetUInt16(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
        }


        public int OCIAttrGetInt32(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            out int attributep,
            IntPtr sizep,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrGetInt32(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
        }


        public int OCIAttrGetIntPtr(IntPtr trgthndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
            out IntPtr attributep,
            IntPtr sizep,
            [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
            IntPtr errhp)
        {
            return NativeCalls.OCIAttrGetIntPtr(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);
        }


        public int OCIDescriptorAlloc(IntPtr parenth,
            out IntPtr hndlpp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType type,
            int xtramem_sz,
            IntPtr usrmempp)
        {
            return NativeCalls.OCIDescriptorAlloc(parenth, out hndlpp, type, xtramem_sz, usrmempp);
        }


        public int OCIHandleAlloc(IntPtr parenth,
            out IntPtr descpp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType type,
            int xtramem_sz,
            IntPtr usrmempp)
        {
            return NativeCalls.OCIHandleAlloc(parenth, out descpp, type, xtramem_sz, usrmempp);
        }


        public int OCIHandleFree(IntPtr hndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType type)
        {
            return NativeCalls.OCIHandleFree(hndlp, type);
        }


        public int OCILobClose(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp)
        {
            return NativeCalls.OCILobClose(svchp, errhp, locp);
        }


        public int OCILobCopy(IntPtr svchp,
            IntPtr errhp,
            IntPtr dst_locp,
            IntPtr src_locp,
            uint amount,
            uint dst_offset,
            uint src_offset)
        {
            return NativeCalls.OCILobCopy(svchp, errhp, dst_locp, src_locp, amount, dst_offset, src_offset);
        }


        public int OCILobErase(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            ref uint amount,
            uint offset)
        {
            return NativeCalls.OCILobErase(svchp, errhp, locp, ref amount, offset);
        }


        public int OCILobGetChunkSize(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            out uint chunk_size)
        {
            return NativeCalls.OCILobGetChunkSize(svchp, errhp, locp, out chunk_size);
        }


        public int OCILobGetLength(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            out uint lenp)
        {
            return NativeCalls.OCILobGetLength(svchp, errhp, locp, out lenp);
        }


        public int OCILobOpen(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            byte mode)
        {
            return NativeCalls.OCILobOpen(svchp, errhp, locp, mode);
        }


        public int OCILobRead(IntPtr svchp,
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
            return NativeCalls.OCILobRead(svchp, errhp, locp, ref amtp, offset, bufp, bufl, ctxp, cbfp, csid, csfrm);
        }


        public int OCILobTrim(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            uint newlen)
        {
            return NativeCalls.OCILobTrim(svchp, errhp, locp, newlen);
        }


        public int OCILobWrite(IntPtr svchp,
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
            return NativeCalls.OCILobWrite(svchp, errhp, locp, ref amtp, offset, bufp, bufl, piece, ctxp, cbfp, csid, csfrm);
        }


        public int OCILobCharSetForm(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            out byte csfrm)
        {
            return NativeCalls.OCILobCharSetForm(svchp, errhp, locp, out csfrm);
        }


        public int OCINlsGetInfo(IntPtr hndl,
            IntPtr errhp,
            [In][Out] byte[] bufp,
            uint buflen,
            ushort item)
        {
            return NativeCalls.OCINlsGetInfo(hndl, errhp, bufp, buflen, item);
        }


        public int OCIServerAttach(IntPtr srvhp,
            IntPtr errhp,
            string dblink,
            [MarshalAs(UnmanagedType.U4)] int dblink_len,
            uint mode)
        {
            return NativeCalls.OCIServerAttach(srvhp, errhp, dblink, dblink_len, mode);
        }


        public int OCIServerDetach(IntPtr srvhp,
            IntPtr errhp,
            uint mode)
        {
            return NativeCalls.OCIServerDetach(srvhp, errhp, mode);
        }


        public int OCIServerVersion(IntPtr hndlp,
            IntPtr errhp,
            [In][Out] byte[] bufp,
            uint bufsz,
            [MarshalAs(UnmanagedType.U4)] OciHandleType type)
        {
            return NativeCalls.OCIServerVersion(hndlp, errhp, bufp, bufsz, type);
        }


        public int OCISessionBegin(IntPtr svchp,
            IntPtr errhp,
            IntPtr usrhp,
            [MarshalAs(UnmanagedType.U4)] OciCredentialType credt,
            [MarshalAs(UnmanagedType.U4)] OciSessionMode mode)
        {
            return NativeCalls.OCISessionBegin(svchp, errhp, usrhp, credt, mode);
        }


        public int OCISessionEnd(IntPtr svchp,
            IntPtr errhp,
            IntPtr usrhp,
            uint mode)
        {
            return NativeCalls.OCISessionEnd(svchp, errhp, usrhp, mode);
        }


        public int OCIParamGet(IntPtr hndlp,
            [MarshalAs(UnmanagedType.U4)] OciHandleType htype,
            IntPtr errhp,
            out IntPtr parmdpp,
            [MarshalAs(UnmanagedType.U4)] int pos)
        {
            return NativeCalls.OCIParamGet(hndlp, htype, errhp, out parmdpp, pos);
        }


        public int OCIStmtExecute(IntPtr svchp,
            IntPtr stmthp,
            IntPtr errhp,
            [MarshalAs(UnmanagedType.U4)] uint iters,
            uint rowoff,
            IntPtr snap_in,
            IntPtr snap_out,
            [MarshalAs(UnmanagedType.U4)] OciExecuteMode mode)
        {
            return NativeCalls.OCIStmtExecute(svchp, stmthp, errhp, iters, rowoff, snap_in, snap_out, mode);
        }


        public int OCIStmtFetch(IntPtr stmtp,
            IntPtr errhp,
            uint nrows,
            ushort orientation,
            uint mode)
        {
            return NativeCalls.OCIStmtFetch(stmtp, errhp, nrows, orientation, mode);
        }



        public int OCIStmtPrepare(IntPtr stmthp,
            IntPtr errhp,
            byte[] stmt,
            [MarshalAs(UnmanagedType.U4)] int stmt_length,
            [MarshalAs(UnmanagedType.U4)] OciStatementLanguage language,
            [MarshalAs(UnmanagedType.U4)] OciStatementMode mode)
        {
            return NativeCalls.OCIStmtPrepare(stmthp, errhp, stmt, stmt_length, language, mode);
        }


        public int OCITransCommit(IntPtr svchp,
            IntPtr errhp,
            uint flags)
        {
            return NativeCalls.OCITransCommit(svchp, errhp, flags);
        }


        public int OCITransRollback(IntPtr svchp,
            IntPtr errhp,
            uint flags)
        {
            return NativeCalls.OCITransRollback(svchp, errhp, flags);
        }


        public int OCITransStart(IntPtr svchp,
            IntPtr errhp,
            uint timeout,
            [MarshalAs(UnmanagedType.U4)] OciTransactionFlags flags)
        {
            return NativeCalls.OCITransStart(svchp, errhp, timeout, flags);
        }


        public int OCICharSetToUnicode(
            IntPtr svchp,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dst,
            [MarshalAs(UnmanagedType.I4)] int dstlen,
            byte[] src,
            [MarshalAs(UnmanagedType.I4)] int srclen,
            out long rsize)
        {
            return NativeCalls.OCICharSetToUnicode(svchp, dst, dstlen, src, srclen, out rsize);
        }


        public int OCIUnicodeToCharSet(
            IntPtr svchp,
            byte[] dst,
            [MarshalAs(UnmanagedType.I4)] int dstlen,
            [MarshalAs(UnmanagedType.LPWStr)] string src,
            [MarshalAs(UnmanagedType.I4)] int srclen,
            out long rsize)
        {
            return NativeCalls.OCIUnicodeToCharSet(svchp, dst, dstlen, src, srclen, out rsize);
        }


        public void OCIDateTimeConstruct(IntPtr hndl,
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
            NativeCalls.OCIDateTimeConstruct(hndl, err, datetime, year, month, day, hour, min, sec, fsec, timezone, timezone_length);
        }


        public void OCIDateTimeGetDate(IntPtr hndl,
            IntPtr err,
            IntPtr datetime,
            out short year,
            out byte month,
            out byte day)
        {
            NativeCalls.OCIDateTimeGetDate(hndl, err, datetime, out year, out month, out day);
        }

        public void OCIDateTimeGetTime(IntPtr hndl,
            IntPtr err,
            IntPtr datetime,
            out byte hour,
            out byte min,
            out byte sec,
            out uint fsec)
        {
            NativeCalls.OCIDateTimeGetTime(hndl, err, datetime, out hour, out min, out sec, out fsec);
        }


        public int OCIIntervalGetDaySecond(IntPtr hndl,
            IntPtr err,
            out int days,
            out int hours,
            out int mins,
            out int secs,
            out int fsec,
            IntPtr interval)
        {
            return NativeCalls.OCIIntervalGetDaySecond(hndl, err, out days, out hours, out mins, out secs, out fsec, interval);
        }


        public int OCIIntervalGetYearMonth(IntPtr hndl,
            IntPtr err,
            out int years,
            out int months,
            IntPtr interval)
        {
            return NativeCalls.OCIIntervalGetYearMonth(hndl, err, out years, out months, interval);
        }

        public int OCIDateTimeCheck(IntPtr hndl,
            IntPtr err, IntPtr date, out uint valid)
        {
            return NativeCalls.OCIDateTimeCheck(hndl, err, date, out valid);
        }
        #endregion

        #region AllocateClear
        public IntPtr AllocateClear(int cb)
        {
            return NativeCalls.calloc(1, cb);
        }
        #endregion
    }
}