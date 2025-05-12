/*
 * ReceiveRange.cs
 * 
 * Gocator 2000 Sample
 * Copyright (C) 2011 by LMI Technologies Inc.
 * 
 * Licensed under The MIT License.
 * Redistributions of files must retain the above copyright notice.
 *
 * Purpose: Connect to Gocator system and receive range data in Range Mode and translate to engineering units (mm).
 * Ethernet output for range data must be enabled.
 */

using System;
using System.Runtime.InteropServices;

static class Constants
{
    public const string SENSOR_IP = "192.168.1.10"; // IP of the sensor used for sensor connection GoSystem_FindSensorByIpAddress() call.
#if DEBUG
    public const string GODLLPATH = @"GoSdk.dll";
    public const string KAPIDLLPATH = @"kApi.dll";
#else
    public const string GODLLPATH = @"GoSdk.dll";
    public const string KAPIDLLPATH = @"kApi.dll";
#endif
}

namespace vpc
{
    #region TypeDef
    public class DataContext
    {
        public double zResolution;
        public double zOffset;
        public uint serialNumber;
    }
    public struct GoStamp
    {
        public UInt64 frameIndex;
        public UInt64 timestamp;
        public Int64 encoder;
        public Int64 encoderAtZ;
        public UInt64 reserved;
    }
    public struct GoPoints
    {
        public Int16 x;
        public Int16 y;
    }
    public struct ProfilePoint
    {
        public double x;
        public double z;
        byte intensity;
    }
    public struct address
    {
        public Int32 version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] IPaddress;
    }
    public struct GoMeasurementData
    {
        public double value;
        public sbyte decision;
    }
    #endregion

    internal class GocatorHdl
    {
        #region Enums
        enum GoMode
        {
            GO_MODE_UNKONWN = -1,
            GO_MODE_VIDEO = 0,
            GO_MODE_RANGE = 1,
            GO_MODE_PROFILE = 2,
            GO_MODE_SURFACE = 3
        }
        enum kStatus
        {
            kERROR_STATE = -1000,                                                // Invalid state.
            kERROR_NOT_FOUND = -999,                                             // Item is not found.
            kERROR_COMMAND = -998,                                               // Command not recognized.
            kERROR_PARAMETER = -997,                                             // Parameter is invalid.
            kERROR_UNIMPLEMENTED = -996,                                         // Feature not implemented.
            kERROR_HANDLE = -995,                                                // Handle is invalid.
            kERROR_MEMORY = -994,                                                // Out of memory.
            kERROR_TIMEOUT = -993,                                               // Action timed out.
            kERROR_INCOMPLETE = -992,                                            // Buffer not large enough for data.
            kERROR_STREAM = -991,                                                // Error in stream.
            kERROR_CLOSED = -990,                                                // Resource is no longer avaiable. 
            kERROR_VERSION = -989,                                               // Invalid version number.
            kERROR_ABORT = -988,                                                 // Operation aborted.
            kERROR_ALREADY_EXISTS = -987,                                        // Conflicts with existing item.
            kERROR_NETWORK = -986,                                               // Network setup/resource error.
            kERROR_HEAP = -985,                                                  // Heap error (leak/double-free).
            kERROR_FORMAT = -984,                                                // Data parsing/formatting error. 
            kERROR_READ_ONLY = -983,                                             // Object is read-only (cannot be written).
            kERROR_WRITE_ONLY = -982,                                            // Object is write-only (cannot be read). 
            kERROR_BUSY = -981,                                                  // Agent is busy (cannot service request).
            kERROR_CONFLICT = -980,                                              // State conflicts with another object.
            kERROR_OS = -979,                                                    // Generic error reported by underlying OS.
            kERROR_DEVICE = -978,                                                // Hardware device error.
            kERROR_FULL = -977,                                                  // Resource is already fully utilized.
            kERROR_IN_PROGRESS = -976,                                           // Operation is in progress, but not yet complete.
            kERROR = 0,                                                          // General error. 
            kOK = 1                                                              // Operation successful. 
        }
        enum GoDataMessageTypes
        {
            GO_DATA_MESSAGE_TYPE_UNKNOWN = -1,
            GO_DATA_MESSAGE_TYPE_STAMP = 0,
            GO_DATA_MESSAGE_TYPE_HEALTH = 1,
            GO_DATA_MESSAGE_TYPE_VIDEO = 2,
            GO_DATA_MESSAGE_TYPE_RANGE = 3,
            GO_DATA_MESSAGE_TYPE_RANGE_INTENSITY = 4,
            GO_DATA_MESSAGE_TYPE_PROFILE = 5,
            GO_DATA_MESSAGE_TYPE_PROFILE_INTENSITY = 6,
            GO_DATA_MESSAGE_TYPE_RESAMPLED_PROFILE = 7,
            GO_DATA_MESSAGE_TYPE_SURFACE = 8,
            GO_DATA_MESSAGE_TYPE_SURFACE_INTENSITY = 9,
            GO_DATA_MESSAGE_TYPE_MEASUREMENT = 10,
            GO_DATA_MESSAGE_TYPE_ALIGNMENT = 11,
            GO_DATA_MESSAGE_TYPE_EXPOSURE_CAL = 12
        }
        enum GoRole
        {
            GO_ROLE_MAIN = 0,                                                    // Sensor is operating as a main sensor.
            GO_ROLE_BUDDY = 1                                                    // Sensor is operating as a buddy sensor.
        }
        #endregion
        #region Dlls
        // use DLL import to access GoSdkd.dll/GoSdk.dll and kApid.dll/kApi.dll
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoMeasurementMsg_Count(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt16 GoMeasurementMsg_Id(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoMeasurementMsg_At(IntPtr msg, UInt32 index);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceIntensityMsg_Width(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceIntensityMsg_Length(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoSurfaceIntensityMsg_RowAt(IntPtr msg, UInt32 index);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceIntensityMsg_XResolution(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceIntensityMsg_YResolution(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSurfaceIntensityMsg_XOffset(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSurfaceIntensityMsg_YOffset(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSetup_SetScanMode(IntPtr setup, GoMode mode);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoSurfaceMsg_RowAt(IntPtr msg, UInt32 index);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceMsg_Width(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceMsg_Length(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceMsg_XResolution(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceMsg_YResolution(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSurfaceMsg_ZResolution(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSensor_Flush(IntPtr sensor);
        [DllImport(Constants.GODLLPATH)]
        private static extern double GoSetup_Exposure(IntPtr setup, GoRole role);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSetup_SetExposure(IntPtr setup, GoRole role, double exposure);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSdk_Construct(ref IntPtr assembly);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSystem_Construct(ref IntPtr system, IntPtr allocator);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSystem_FindSensorById(IntPtr system, UInt32 id, ref IntPtr sensor);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSystem_FindSensorByIpAddress(IntPtr system, IntPtr addr, ref IntPtr sensor);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSystem_EnableData(IntPtr system, bool enable);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSystem_ReceiveData(IntPtr system, ref IntPtr data, UInt64 timeout);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSensor_Connect(IntPtr sensor);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSystem_Start(IntPtr system);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoSystem_Stop(IntPtr system);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoSensor_Setup(IntPtr sensor);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoDataSet_Count(IntPtr dataset);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoDataSet_At(IntPtr dataset, UInt32 index);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoStampMsg_Count(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoStampMsg_At(IntPtr msg, UInt32 index);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoRangeMsg_Count(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoRangeMsg_At(IntPtr msg, UInt32 index);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoRangeMsg_ZResolution(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoRangeMsg_ZOffset(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoRangeIntensityMsg_Count(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern IntPtr GoRangeIntensityMsg_At(IntPtr msg, UInt32 index);
        [DllImport(Constants.GODLLPATH)]
        private static extern GoDataMessageTypes GoDataMsg_Type(IntPtr msg);
        [DllImport(Constants.GODLLPATH)]
        private static extern UInt32 GoSetup_XSpacingCount(IntPtr setup, GoRole role);
        [DllImport(Constants.GODLLPATH)]
        private static extern Int32 GoDestroy(IntPtr obj);
        [DllImport(Constants.KAPIDLLPATH)]
        private static extern Int32 kIpAddress_Parse(IntPtr addrPtr, [MarshalAs(UnmanagedType.LPStr)] string text);
        #endregion

        internal double Exposure
        {
            get
            {
                if (setup == IntPtr.Zero)
                {
                    return -1;
                }
                else
                {
                    return GoSetup_Exposure(setup, GoRole.GO_ROLE_MAIN);
                }
            }
            set
            {
                if (setup != IntPtr.Zero)
                {
                    GoSetup_SetExposure(setup, GoRole.GO_ROLE_MAIN, value);
                    GoSensor_Flush(sensor);
                }
            }
        }
        static IntPtr setup = IntPtr.Zero;
        static IntPtr sensor = IntPtr.Zero;
        static IntPtr api = IntPtr.Zero;
        static IntPtr system = IntPtr.Zero;

        internal int Init()
        {
            kStatus status;
            IntPtr addrPtr = IntPtr.Zero;
            address addr = new address();

            if ((status = (kStatus)GoSdk_Construct(ref api)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format("GoSdk_Construct Error:{0}", (int)status));
                return (int)status;
            }

            if ((status = (kStatus)GoSystem_Construct(ref system, IntPtr.Zero)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format("GoSystem_Construct Error:{0}", (int)status));
                return (int)status;
            }

            addrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(addr));
            Marshal.StructureToPtr(addr, addrPtr, false);

            if ((status = (kStatus)kIpAddress_Parse(addrPtr, Constants.SENSOR_IP)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format("kIpAddress_Parse Error:{0}", (int)status));
                return (int)status;
            }

            if ((status = (kStatus)GoSystem_FindSensorByIpAddress(system, addrPtr, ref sensor)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format("GoSystem_FindSensorByIpAddress Error:{0}", (int)status));
                return (int)status;
            }

            if ((status = (kStatus)GoSensor_Connect(sensor)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format("GoSensor_Connect Error:{0}", (int)status));
                return (int)status;
            }

            if ((status = (kStatus)GoSystem_EnableData(system, true)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format("GoSystem_EnableData Error:{0}", (int)status));
                return (int)status;
            }

            if ((setup = GoSensor_Setup(sensor)) == null)
            {
                Program.ErrHdl(string.Format("Error: GoSensor_Setup: Invalid Handle"));
                return 0;
            }

            if ((status = (kStatus)GoSetup_SetScanMode(setup, GoMode.GO_MODE_SURFACE)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format("GoSetup_SetScanMode Error:{0}", (int)status));
                return (int)status;
            }

            if ((status = (kStatus)GoSystem_Start(system)) != kStatus.kOK)
            {
                Program.ErrHdl(string.Format(" GoSystem_Start Error:{0}", (int)status));
                return (int)status;
            }

            //Console.WriteLine("Waiting for Whole Part Data...");

            return (int)kStatus.kOK;
        }
        internal Cognex.VisionPro.CogImage16Range TrigOne()
        {
            kStatus status;
            IntPtr dataObj = IntPtr.Zero;
            IntPtr stampMsg = IntPtr.Zero;
            IntPtr surfaceMsg = IntPtr.Zero;
            IntPtr surfaceIntMsg = IntPtr.Zero;
            IntPtr stampPtr = IntPtr.Zero;
            GoStamp stamp;
            Cognex.VisionPro.CogImage16Range img16 = null;
            try
            {
                IntPtr dataset = IntPtr.Zero;
                
                while ((status = (kStatus)GoSystem_ReceiveData(system, ref dataset, 2000000)) != kStatus.kOK) ;

                if (GoDataSet_Count(dataset) == 0)
                {
                    //Console.WriteLine("Data received, but with zero items. No Ethernet output enabled!");
                    return null;
                }

                // each result can have multiple data items
                // loop through all items in result message
                for (UInt32 i = 0; i < GoDataSet_Count(dataset); i++)
                {
                    dataObj = GoDataSet_At(dataset, i);

                    switch (GoDataMsg_Type(dataObj))
                    {// retrieve GoStamp message
                        case GoDataMessageTypes.GO_DATA_MESSAGE_TYPE_STAMP:
                            stampMsg = dataObj;
                            for (UInt32 j = 0; j < GoStampMsg_Count(stampMsg); j++)
                            {
                                stampPtr = GoStampMsg_At(stampMsg, j);
                                stamp = (GoStamp)Marshal.PtrToStructure(stampPtr, typeof(GoStamp));
                                //Console.WriteLine(" Timestamp {0}", stamp.timestamp);
                                //Console.WriteLine(" Encoder position at leading edge: {0}", stamp.encoder);
                                //Console.WriteLine(" Frame index: {0}", stamp.frameIndex);
                                //Console.WriteLine(" Stamp Message count: {0}", GoStampMsg_Count(stampMsg));
                            }
                            break;
                        case GoDataMessageTypes.GO_DATA_MESSAGE_TYPE_SURFACE:
                            // retrieve surface data
                            surfaceMsg = dataObj;
                            UInt32 width = GoSurfaceMsg_Width(surfaceMsg);
                            UInt32 height = GoSurfaceMsg_Length(surfaceMsg);
                            UInt32 bufferSize = width * height;
                            IntPtr bufferPointer = GoSurfaceMsg_RowAt(surfaceMsg, 0);

                            //Console.WriteLine("Whole Part Height Map received:");
                            //Console.WriteLine(" Buffer width: {0}", width);
                            //Console.WriteLine(" Buffer height: {0}", height);

                            //short[] ranges = new short[bufferSize];
                            //Marshal.Copy(bufferPointer, ranges, 0, ranges.Length);

                            Cognex.VisionPro.CogImage16Grey imdata = new Cognex.VisionPro.CogImage16Grey((int)width, (int)height);
                            var memory = imdata.Get16GreyPixelMemory(Cognex.VisionPro.CogImageDataModeConstants.Write, 0, 0, imdata.Width, imdata.Height);
                            Capture.CopyMemory(memory.Scan0, bufferPointer, (int)bufferSize * 2);
                            img16 = new Cognex.VisionPro.CogImage16Range(imdata, 0, new Cognex.VisionPro3D.Cog3DTransformLinear());
                            if (memory.Stride != memory.Width)
                                Program.ErrHdl("请设置图像宽度为 " + memory.Stride);
                            memory.Dispose();
                            break;
                        case GoDataMessageTypes.GO_DATA_MESSAGE_TYPE_SURFACE_INTENSITY:
                            // retreieve surface intensity data
                            surfaceIntMsg = dataObj;
                            width = GoSurfaceIntensityMsg_Width(surfaceIntMsg);
                            height = GoSurfaceIntensityMsg_Length(surfaceIntMsg);
                            bufferSize = width * height;
                            IntPtr bufferPointeri = GoSurfaceIntensityMsg_RowAt(surfaceIntMsg, 0);

                            Console.WriteLine("Whole Part Intensity Image received:");
                            Console.WriteLine(" Buffer width: {0}", width);
                            Console.WriteLine(" Buffer height: {0}", height);
                            byte[] ranges = new byte[bufferSize];
                            Marshal.Copy(bufferPointeri, ranges, 0, ranges.Length);
                            break;

                        case GoDataMessageTypes.GO_DATA_MESSAGE_TYPE_MEASUREMENT:
                            {
                                // retreieve resampled profile data
                                IntPtr measurementMsg = dataObj;
                                //Console.WriteLine("  Measurement Message batch count: {0}\n", GoMeasurementMsg_Count(measurementMsg));
                                for (UInt32 k = 0; k < GoMeasurementMsg_Count(measurementMsg); ++k)
                                {
                                    IntPtr measurementMsgPtr = GoMeasurementMsg_At(measurementMsg, k);
                                    GoMeasurementData measurementData = (GoMeasurementData)Marshal.PtrToStructure(measurementMsgPtr, typeof(GoMeasurementData));
                                    //Console.WriteLine("ID: {0}", GoMeasurementMsg_Id(measurementMsg));
                                    //Console.WriteLine("Value: {0}", measurementData.value);
                                    //Console.WriteLine("Decision: {0}", measurementData.decision);
                                }
                            }
                            break;
                    }
                }

                // destroy handles
                GoDestroy(dataset);
                dataset = IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
            return img16;
        }

        internal void Dispose()
        {
            if (system != IntPtr.Zero)
            {
                GoSystem_Stop(system);
                GoDestroy(system);
            }
            if (api != IntPtr.Zero)
                GoDestroy(api);
        }
    }
}
