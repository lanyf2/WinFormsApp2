using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vpc
{
    public static class multiple_cyclone_programming
    {
        static bool RunFlag = false;
        internal static int RunDownload()
        {
            return RunDownload("1");
        }
        internal static int RunDownload(string arg)
        {
            if (false == RunFlag)
            {
                if (System.IO.File.Exists("visual_sap_control.exe"))
                {
                    RunFlag = true;
                    try
                    {
                        var proc = System.Diagnostics.Process.Start("visual_sap_control.exe", arg);
                        for (int i = 0; i < 60; i++)
                        {
                            if (proc.HasExited)
                                break;
                            //string ansi = string.Format(Cognex.VisionPro.BlockBase.WaitProgramStr, i + 1);
                            //Cognex.VisionPro.BlockBase.WaitHdl(1001, ansi);
                            //Cognex.VisionPro.BlockBase.WaitHdl(1001, ansi);
                            Application.DoEvents();
                        }
                        if (proc.HasExited)
                            return proc.ExitCode;
                    }
                    catch (Exception ex)
                    {
                        Program.ErrHdl(ex);
                    }
                    finally
                    {
                        RunFlag = false;
                    }
                }
            }
            else
                Program.ErrHdl(new Exception("重复启动编程指令"));
            return 1;
        }
        public const UInt32 PortType_USB = 5;
        public const UInt32 PortType_Ethernet = 6;
        public const UInt32 PortType_Serial = 7;

        public const UInt32 Open_by_IP_Address = 1;
        public const UInt32 Open_by_Name = 2;

        public const byte Media_Flash = 1;
        public const byte Media_CompactFlash = 2;

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enumerate_all_ports")]
        public static extern void enumerate_all_ports();

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "close_all_ports")]
        public static extern void close_all_ports();

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "version")]
        //public static extern String version();
        public static extern IntPtr version();

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "connect_to_cyclonepromax")]
        public static extern UInt32 connect_to_cyclonepromax(UInt32 port_type, UInt32 identifier_type, String port_identifier);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "connect_to_cyclonepromax_by_ip")]
        public static extern UInt32 connect_to_cyclonepromax_by_ip(String port_identifier);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "disconnect_from_cyclonepromax")]
        public static extern bool disconnect_from_cyclonepromax(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_local_machine_ip_number")]
        public static extern void set_local_machine_ip_number(String ip_number);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_COM_port")]
        public static extern bool set_COM_port(UInt32 COM_port_number);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "START_execute_all_commands")]
        public static extern bool START_execute_all_commands(UInt32 cyclonepromaxhandle, byte image_id);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "START_dynamic_program_bytes")]
        public static extern bool START_dynamic_program_bytes(UInt32 cyclonepromaxhandle, UInt32 target_address, UInt16 data_length, byte[] buffer);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "check_STARTED_cyclonepromax_status")]
        public static extern UInt32 check_STARTED_cyclonepromax_status(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dynamic_read_bytes")]
        public static extern bool dynamic_read_bytes(UInt32 cyclonepromaxhandle, UInt32 target_address, UInt16 data_length, byte[] buffer);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_last_error_code")]
        public static extern UInt16 get_last_error_code(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_last_error_addr")]
        public static extern UInt32 get_last_error_addr(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pro_set_active_security_code")]
        public static extern bool pro_set_active_security_code(UInt32 cyclonepromaxhandle, byte[] buffer);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "reset_cyclonepromax")]
        public static extern bool reset_cyclonepromax(UInt32 cyclonepromaxhandle, UInt32 reset_delay_in_ms);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_firmware_version")]
        //public static extern String get_firmware_version(UInt32 cyclonepromaxhandle);
        public static extern IntPtr get_firmware_version(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_image_description")]
        //public static extern String get_image_description(UInt32 cyclonepromaxhandle, byte image_id);
        public static extern IntPtr get_image_description(UInt32 cyclonepromaxhandle, byte image_id);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "compare_image_with_file")]
        public static extern bool compare_image_with_file(UInt32 cyclonepromaxhandle, String aFile, byte image_id);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "erase_all_cyclone_images")]
        public static extern bool erase_all_cyclone_images(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "add_image_to_cyclone")]
        public static extern UInt32 add_image_to_cyclone(UInt32 cyclonepromaxhandle, String aFile);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_image_with_file")]
        public static extern bool update_image_with_file(UInt32 cyclonepromaxhandle, String aFile);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "count_cyclonepromax_images")]
        public static extern byte count_cyclonepromax_images(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "toggle_power_no_background_entrance")]
        public static extern bool toggle_power_no_background_entrance(UInt32 cyclonepromaxhandle);

        [DllImport(@"CYCLONE_CONTROL.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_cyclonepromax_media_type")]
        public static extern void set_cyclonepromax_media_type(byte new_media_type);
    }
}
