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

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using dotNetCore.Data.OracleClient.System.Data.OracleClient.Oci.OciNativeCalls;

namespace System.Data.OracleClient.Oci
{
    internal sealed class OciCalls
    {
        private static string OCI_DLL;
        private static IOciNativeCalls mOciNativeCalls = default(IOciNativeCalls);
        private static readonly object mLockNewOciNativeCalls = new object();
        private static IOciNativeCalls OciNativeCalls
        {
            get
            {
                if (mOciNativeCalls == default(IOciNativeCalls))
                {
                    lock (mLockNewOciNativeCalls)
                    {
                        if (mOciNativeCalls == default(IOciNativeCalls))
                        {
                            if ((int)Environment.OSVersion.Platform == 4 || (int)Environment.OSVersion.Platform == 128)
                            {
                                mOciNativeCalls = new OciNativeCallsLinux();
                            }
                            else if ((int)Environment.OSVersion.Platform == 6)
                            {
                                mOciNativeCalls = new OciNativeCallsMacOSX();
                            }
                            else
                            {
                                mOciNativeCalls = new OciNativeCallsWindows();
                            }
                            OCI_DLL = mOciNativeCalls.getOCI_DLL();
                        }
                    }
                }
                return mOciNativeCalls;
            }
            set { mOciNativeCalls = value; }
        }


#if TRACE
        private static bool traceOci;

        static OciCalls()
        {
            string env = Environment.GetEnvironmentVariable("OCI_TRACE");

            traceOci = (env != null && env.Length > 0);
        }
#endif

        private OciCalls() { }

        #region OCI call wrappers

        internal static int OCIAttrSet(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            IntPtr attributep,
            uint size,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, string.Format("OCIAttrSet ({0}, {1})", trghndltyp, attrtype), OCI_DLL);
#endif
            return OciNativeCalls.OCIAttrSet(trgthndlp, trghndltyp, attributep, size, attrtype, errhp);

        }

        internal static int OCIAttrSetString(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            string attributep,
            uint size,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, string.Format("OCIAttrSetString ({0}, {1})", trghndltyp, attrtype), OCI_DLL);
#endif
            return OciNativeCalls.OCIAttrSetString(trgthndlp, trghndltyp, attributep, size, attrtype, errhp);
        }
        // #if ORACLE_DATA_ACCESS
        // 		internal static int OCIPasswordChange (IntPtr svchp, IntPtr errhp,
        // 				int usernm_len,
        // 				byte [] opasswd,
        // 				int opasswd_len,
        // 				byte [] npasswd,
        // 				int npasswd_len,
        // 				uint mode)
        // 		{
        // #if TRACE
        // 			Trace.WriteLineIf(traceOci, string.Format("OCIPasswordChange"), OCI_DLL);
        // #endif
        // 			return OciNativeCalls.OCIPasswordChange (svchp, errhp, user_name, (uint) usernm_len, opasswd, (uint) opasswd_len, npasswd, (uint) npasswd_len, mode);
        // 		}
        // #endif
        internal static int OCIErrorGet(IntPtr hndlp,
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
            return OciNativeCalls.OCIErrorGet(hndlp, recordno, sqlstate, out errcodep, bufp, bufsize, type);
        }

        internal static int OCIBindByName(IntPtr stmtp,
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
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIBindByName", OCI_DLL);
#endif
            return OciNativeCalls.OCIBindByName(stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
                    value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
        }

        internal static int OCIBindByNameRef(IntPtr stmtp,
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
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIBindByName", OCI_DLL);
#endif
            return OciNativeCalls.OCIBindByNameRef(stmtp, out bindpp, errhp, placeholder, placeh_len, ref valuep,
                    value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
        }

        internal static int OCIBindByNameBytes(IntPtr stmtp,
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
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIBindByName", OCI_DLL);
#endif
            return OciNativeCalls.OCIBindByNameBytes(stmtp, out bindpp, errhp, placeholder, placeh_len, valuep,
                    value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
        }

        internal static int OCIBindByPos(IntPtr stmtp,
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
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIBindByPos", OCI_DLL);
#endif
            return OciNativeCalls.OCIBindByPos(stmtp, out bindpp, errhp, position, valuep,
                    value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);
        }

        internal static int OCIBindByPosRef(IntPtr stmtp,
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
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIBindByPos", OCI_DLL);
#endif
            return OciNativeCalls.OCIBindByPosRef(stmtp, out bindpp, errhp, position, ref valuep,
                    value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);


        }

        internal static int OCIBindByPosBytes(IntPtr stmtp,
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
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIBindByPos", OCI_DLL);
#endif
            return OciNativeCalls.OCIBindByPosBytes(stmtp, out bindpp, errhp, position, valuep,
                    value_sz, dty, indp, alenp, rcodep, maxarr_len, curelp, mode);

        }

        internal static void OCIDateTimeConstruct(IntPtr hndl,
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
            OciNativeCalls.OCIDateTimeConstruct(hndl, err, datetime, year, month, day, hour, min,
                                                    sec, fsec, timezone, timezone_length);


        }
        internal static void OCIDateTimeGetDate(IntPtr hndl,
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

            OciNativeCalls.OCIDateTimeGetDate(hndl, err, datetime, out retYear, out retMonth, out retDay);

            year = retYear;
            month = retMonth;
            day = retDay;
        }

        internal static void OCIDateTimeGetTime(IntPtr hndl,
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

            OciNativeCalls.OCIDateTimeGetTime(hndl, err, datetime, out retHour, out retMin, out retSec, out retFsec);

            hour = retHour;
            min = retMin;
            sec = retSec;
            fsec = retFsec;
        }

        internal static int OCIIntervalGetDaySecond(IntPtr hndl,
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

            rc = OciNativeCalls.OCIIntervalGetDaySecond(hndl, err, out retDays, out retHours, out retMins, out retSecs, out retFsec, interval);


            days = retDays;
            hours = retHours;
            mins = retMins;
            secs = retSecs;
            fsec = retFsec;
            return rc;
        }

        internal static int OCIIntervalGetYearMonth(IntPtr hndl,
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

            rc = OciNativeCalls.OCIIntervalGetYearMonth(hndl, err, out retYears, out retMonths, interval);

            years = retYears;
            months = retMonths;
            return rc;
        }

        internal static int OCIDefineByPos(IntPtr stmtp,
            out IntPtr defnpp,
            IntPtr errhp,
            int position,
            IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr rlenp,
            IntPtr rcodep,
            uint mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIDefineByPos", OCI_DLL);
#endif
            return OciNativeCalls.OCIDefineByPos(stmtp, out defnpp, errhp, position, valuep,
                    value_sz, dty, indp, rlenp, rcodep, mode);


        }

        internal static int OCIDefineByPosPtr(IntPtr stmtp,
            out IntPtr defnpp,
            IntPtr errhp,
            int position,
            ref IntPtr valuep,
            int value_sz,
            [MarshalAs(UnmanagedType.U4)] OciDataType dty,
            IntPtr indp,
            IntPtr rlenp,
            IntPtr rcodep,
            uint mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIDefineByPosPtr", OCI_DLL);
#endif
            return OciNativeCalls.OCIDefineByPosPtr(stmtp, out defnpp, errhp, position, ref valuep,
                    value_sz, dty, indp, rlenp, rcodep, mode);


        }

        internal static int OCIDescriptorFree(IntPtr hndlp,
            OciHandleType type)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, string.Format("OCIDescriptorFree ({0})", type), OCI_DLL);
#endif

            return OciNativeCalls.OCIDescriptorFree(hndlp, type);

        }

        internal static int OCIEnvCreate(out IntPtr envhpp,
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
            return OciNativeCalls.OCIEnvCreate(out envhpp, mode, ctxp, malocfp, ralocfp, mfreep,
                    xtramem_sz, usrmempp);

        }

        internal static int OCICacheFree(IntPtr envhp,
            IntPtr svchp,
            IntPtr stmthp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCICacheFree", OCI_DLL);
#endif

            return OciNativeCalls.OCICacheFree(envhp, svchp, stmthp);

        }

        internal static int OCIAttrGet(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            out IntPtr attributep,
            out int sizep,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIAttrGet", OCI_DLL);
#endif

            return OciNativeCalls.OCIAttrGet(trgthndlp, trghndltyp, out attributep, out sizep, attrtype, errhp);

        }

        internal static int OCIAttrGetSByte(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            out sbyte attributep,
            IntPtr sizep,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIAttrGetSByte", OCI_DLL);
#endif

            return OciNativeCalls.OCIAttrGetSByte(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);

        }

        internal static int OCIAttrGetByte(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            out byte attributep,
            IntPtr sizep,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIAttrGetByte", OCI_DLL);
#endif

            return OciNativeCalls.OCIAttrGetByte(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);

        }

        internal static int OCIAttrGetUInt16(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            out ushort attributep,
            IntPtr sizep,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIAttrGetUInt16", OCI_DLL);
#endif

            return OciNativeCalls.OCIAttrGetUInt16(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);

        }

        internal static int OCIAttrGetInt32(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            out int attributep,
            IntPtr sizep,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIAttrGetInt32", OCI_DLL);
#endif

            return OciNativeCalls.OCIAttrGetInt32(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);

        }

        internal static int OCIAttrGetIntPtr(IntPtr trgthndlp,
            OciHandleType trghndltyp,
            out IntPtr attributep,
            IntPtr sizep,
            OciAttributeType attrtype,
            IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIAttrGetIntPtr", OCI_DLL);
#endif

            return OciNativeCalls.OCIAttrGetIntPtr(trgthndlp, trghndltyp, out attributep, sizep, attrtype, errhp);

        }

        internal static int OCIDescriptorAlloc(IntPtr parenth,
            out IntPtr hndlpp,
            OciHandleType type,
            int xtramem_sz,
            IntPtr usrmempp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIDescriptorAlloc", OCI_DLL);
#endif

            return OciNativeCalls.OCIDescriptorAlloc(parenth, out hndlpp, type, xtramem_sz, usrmempp);

        }

        internal static int OCIHandleAlloc(IntPtr parenth,
            out IntPtr descpp,
            OciHandleType type,
            int xtramem_sz,
            IntPtr usrmempp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, string.Format("OCIHandleAlloc ({0})", type), OCI_DLL);
#endif

            return OciNativeCalls.OCIHandleAlloc(parenth, out descpp, type, xtramem_sz, usrmempp);
        }

        internal static int OCIHandleFree(IntPtr hndlp,
            OciHandleType type)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, string.Format("OCIHandleFree ({0})", type), OCI_DLL);
#endif

            return OciNativeCalls.OCIHandleFree(hndlp, type);
        }

        internal static int OCILobClose(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCILobClose", OCI_DLL);
#endif

            return OciNativeCalls.OCILobClose(svchp, errhp, locp);

        }

        internal static int OCILobCopy(IntPtr svchp,
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

            return OciNativeCalls.OCILobCopy(svchp, errhp, dst_locp, src_locp, amount, dst_offset, src_offset);

        }

        internal static int OCILobErase(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            ref uint amount,
            uint offset)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCILobErase", OCI_DLL);
#endif

            return OciNativeCalls.OCILobErase(svchp, errhp, locp, ref amount, offset);

        }

        internal static int OCILobGetChunkSize(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            out uint chunk_size)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCILobGetChunkSize", OCI_DLL);
#endif

            return OciNativeCalls.OCILobGetChunkSize(svchp, errhp, locp, out chunk_size);

        }

        internal static int OCILobGetLength(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            out uint lenp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCILobGetLength", OCI_DLL);
#endif
            return OciNativeCalls.OCILobGetLength(svchp, errhp, locp, out lenp);

        }

        internal static int OCILobOpen(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            byte mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCILobOpen", OCI_DLL);
#endif

            return OciNativeCalls.OCILobOpen(svchp, errhp, locp, mode);

        }

        internal static int OCILobRead(IntPtr svchp,
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
            return OciNativeCalls.OCILobRead(svchp, errhp, locp, ref amtp, offset, bufp, bufl,
                ctxp, cbfp, csid, csfrm);
        }

        internal static int OCILobTrim(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            uint newlen)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCILobTrim", OCI_DLL);
#endif

            return OciNativeCalls.OCILobTrim(svchp, errhp, locp, newlen);

        }

        internal static int OCILobWrite(IntPtr svchp,
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
            return OciNativeCalls.OCILobWrite(svchp, errhp, locp, ref amtp, offset, bufp, bufl,
                piece, ctxp, cbfp, csid, csfrm);
        }

        internal static int OCILobCharSetForm(IntPtr svchp,
            IntPtr errhp,
            IntPtr locp,
            out byte csfrm)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCILobCharSetForm", OCI_DLL);
#endif

            return OciNativeCalls.OCILobCharSetForm(svchp, errhp, locp, out csfrm);

        }

        internal static int OCINlsGetInfo(IntPtr hndl,
            IntPtr errhp,
            ref byte[] bufp,
            uint buflen,
            ushort item)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCINlsGetInfo", OCI_DLL);
#endif

            return OciNativeCalls.OCINlsGetInfo(hndl, errhp, bufp, buflen, item);

        }

        internal static int OCIServerAttach(IntPtr srvhp,
            IntPtr errhp,
            string dblink,
            int dblink_len,
            uint mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIServerAttach", OCI_DLL);
#endif

            return OciNativeCalls.OCIServerAttach(srvhp, errhp, dblink, dblink_len, mode);
        }

        internal static int OCIServerDetach(IntPtr srvhp,
            IntPtr errhp,
            uint mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIServerDetach", OCI_DLL);
#endif

            return OciNativeCalls.OCIServerDetach(srvhp, errhp, mode);
        }

        internal static int OCIServerVersion(IntPtr hndlp,
            IntPtr errhp,
            ref byte[] bufp,
            uint bufsz,
            OciHandleType hndltype)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIServerVersion", OCI_DLL);
#endif

            return OciNativeCalls.OCIServerVersion(hndlp, errhp, bufp, bufsz, hndltype);
        }

        internal static int OCISessionBegin(IntPtr svchp,
            IntPtr errhp,
            IntPtr usrhp,
            OciCredentialType credt,
            OciSessionMode mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCISessionBegin", OCI_DLL);
#endif


            return OciNativeCalls.OCISessionBegin(svchp, errhp, usrhp, credt, mode);
        }

        internal static int OCISessionEnd(IntPtr svchp,
            IntPtr errhp,
            IntPtr usrhp,
            uint mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCISessionEnd", OCI_DLL);
#endif

            return OciNativeCalls.OCISessionEnd(svchp, errhp, usrhp, mode);
        }

        internal static int OCIParamGet(IntPtr hndlp,
            OciHandleType htype,
            IntPtr errhp,
            out IntPtr parmdpp,
            int pos)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIParamGet", OCI_DLL);
#endif

            return OciNativeCalls.OCIParamGet(hndlp, htype, errhp, out parmdpp, pos);
        }

        internal static int OCIStmtExecute(IntPtr svchp,
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

            return OciNativeCalls.OCIStmtExecute(svchp, stmthp, errhp, it, rowoff,
                snap_in, snap_out, mode);

        }

        internal static int OCIStmtFetch(IntPtr stmtp,
            IntPtr errhp,
            uint nrows,
            ushort orientation,
            uint mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIStmtFetch", OCI_DLL);
#endif

            return OciNativeCalls.OCIStmtFetch(stmtp, errhp, nrows, orientation, mode);
        }


        internal static int OCIStmtPrepare(IntPtr stmthp,
            IntPtr errhp,
            byte[] stmt,
            int stmt_length,
            OciStatementLanguage language,
            OciStatementMode mode)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, string.Format("OCIStmtPrepare ({0})", System.Text.Encoding.UTF8.GetString(stmt)), OCI_DLL);
#endif
            return OciNativeCalls.OCIStmtPrepare(stmthp, errhp, stmt, stmt_length, language, mode);
        }

        internal static int OCITransCommit(IntPtr svchp,
            IntPtr errhp,
            uint flags)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCITransCommit", OCI_DLL);
#endif

            return OciNativeCalls.OCITransCommit(svchp, errhp, flags);
        }

        internal static int OCITransRollback(IntPtr svchp,
            IntPtr errhp,
            uint flags)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCITransRollback", OCI_DLL);
#endif
            return OciNativeCalls.OCITransRollback(svchp, errhp, flags);
        }

        internal static int OCITransStart(IntPtr svchp,
            IntPtr errhp,
            uint timeout,
            OciTransactionFlags flags)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCITransStart", OCI_DLL);
#endif
            return OciNativeCalls.OCITransStart(svchp, errhp, timeout, flags);
        }

        internal static int OCICharSetToUnicode(
            IntPtr svchp,
            StringBuilder dst,
            byte[] src,
            out int rsize)
        {
            int rc;
            long retSize;

#if TRACE
            Trace.WriteLineIf(traceOci, "OCICharSetToUnicode", OCI_DLL);
#endif

            rc = OciNativeCalls.OCICharSetToUnicode(svchp, dst,
                (dst != null ? dst.Capacity : 0),
                src, src.Length, out retSize);

            rsize = (int)retSize;
            return (rc);
        }

        internal static int OCIUnicodeToCharSet(
            IntPtr svchp,
            byte[] dst,
            string src,
            out int rsize)
        {
            int rc;
            long retSize;

#if TRACE
            Trace.WriteLineIf(traceOci, "OCIUnicodeToCharSet", OCI_DLL);
#endif
            rc = OciNativeCalls.OCIUnicodeToCharSet(svchp, dst,
                    (dst != null ? dst.Length : 0),
                    src, src.Length, out retSize);

            rsize = (int)retSize;
            return (rc);
        }

        internal static int OCIDateTimeCheck(IntPtr hndl,
            IntPtr err, IntPtr date, out uint valid)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIDateTimeCheck", OCI_DLL);
#endif
            uint retValid;
            int rc;
            rc = OciNativeCalls.OCIDateTimeCheck(hndl, err, date, out retValid);
            valid = retValid;
            return rc;
        }

        internal static int OCIAttrGetRowIdDesc(IntPtr trgthndlp,
                [MarshalAs(UnmanagedType.U4)] OciHandleType trghndltyp,
                IntPtr attributep,
                ref uint sizep,
                [MarshalAs(UnmanagedType.U4)] OciAttributeType attrtype,
                IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIAttrGetRowIdDesc", OCI_DLL);
#endif
            return OciNativeCalls.OCIAttrGetRowIdDesc(trgthndlp, trghndltyp, attributep, ref sizep, attrtype, errhp);
        }

        internal static int OCIRowidToChar(IntPtr rowidDesc,
                IntPtr outbfp,
                ref ushort outbflp,
                IntPtr errhp)
        {
#if TRACE
            Trace.WriteLineIf(traceOci, "OCIRowidToChar", OCI_DLL);
#endif
            return OciNativeCalls.OCIRowidToChar(rowidDesc, outbfp, ref outbflp, errhp);
        }
        #endregion

        #region AllocateClear

        //http://download-uk.oracle.com/docs/cd/B14117_01/appdev.101/b10779/oci05bnd.htm#423147
        internal static IntPtr AllocateClear(int cb)
        {
            return OciNativeCalls.AllocateClear(cb);
        }

        #endregion AllocateClear
    }
}

