using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace vpc
{
    public class port
    {
        private ComPPI200 pp;
        public port()
        {
            pp = new ComPPI200("COM2", 2, 9600);
        }
        public void writevw(int va, string add)
        {
            string s = add.ToUpper();
            if (s.StartsWith("VW"))
            {
                byte[] re = new byte[2];
                re[0] = (byte)(va / 256);
                re[1] = (byte)(va);
                pp.write(s, re);
            }
            else if (s.StartsWith("VD"))
            {
                byte[] re = new byte[2];
                re[0] = (byte)(va / 256 / 256 / 256);
                re[1] = (byte)(va / 256 / 256);
                re[2] = (byte)(va / 256);
                re[3] = (byte)(va);
                pp.write(s, re);
            }
            else
                pp.write(s, va);
        }
        public int readvw(string add)
        {
            byte[] re = pp.read(add);
            Array.Reverse(re);
            if (re.Length == 2)
                return BitConverter.ToInt16(re, 0);
            else if (re.Length == 4)
                return BitConverter.ToInt32(re, 0);
            throw new InvalidOperationException(BitConverter.ToString(re, 0));
        }
        public void closePort()
        {
            pp.close();
        }
    }
    internal class ComPPI200
    {
        SerialPort ppiPort;
        byte PlcAddr;//PLC地址
        PortStatus Status = PortStatus.Idle;
        enum PortStatus { Reading, Writing, Idle }
        internal bool portReady
        {
            get
            {
                if (Status == PortStatus.Idle)
                    return true;
                else
                    return false;
            }
        }
        object RWlock;
        internal string portName
        {
            set
            {
                if (ppiPort.PortName != value)
                {
                    if (ppiPort.IsOpen)
                        ppiPort.Close();
                    ppiPort.PortName = value;
                }
            }
            get { return ppiPort.PortName; }
        }
        internal int BaudRate
        {
            set
            {
                if (ppiPort.BaudRate != value)
                {
                    if (ppiPort.IsOpen)
                        ppiPort.Close();
                    ppiPort.BaudRate = value;
                }
            }
            get { return ppiPort.BaudRate; }
        }
        internal bool logPort = false;
        /// <summary>
        /// 只支持200PLC
        /// </summary>
        /// <param name="portName">串口端口号</param>
        /// <param name="_PlcNumber">200PLC站号</param>
        internal ComPPI200(string portName, int _PlcNumber)
            : this(portName, _PlcNumber, 9600)
        {
        }
        internal ComPPI200(string portName, int _PlcNumber, int baudRate)
        {
            ppiPort = new SerialPort(portName, baudRate, Parity.Even, 8, StopBits.One);
            ppiPort.ReadTimeout = 200;
            PlcAddr = (byte)_PlcNumber;
            RWlock = new object();
            //ppiPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(ppiPort_DataReceived);
        }

        void writedata(byte[] buf)
        {
            if (buf != null && buf.Length > 0)
            {
                if (logPort && LogEvents != null)
                    LogEvents(string.Format("[发送]{0}\r\n", BitConverter.ToString(buf)));
                ppiPort.Write(buf, 0, buf.Length);
            }
        }
        byte[] ReadData(int len)
        {
            byte[] re = new byte[len];
            //Thread.Sleep(len * 2 + 5);
            int lenre = 0;
            while (lenre < len)
            {
                Thread.Sleep(30);
                lenre += ppiPort.Read(re, lenre, len - lenre);
            }
            if (logPort && LogEvents != null)
                LogEvents(string.Format("[接收]{0}\r\n", BitConverter.ToString(re)));
            if (lenre != len)
                throw new ArgumentException("读取长度不匹配:" + lenre);
            return re;
        }
        void checkE5()
        {
            byte[] receiveBuf;
            try
            {
                receiveBuf = ReadData(1);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message + " " + ppiPort.BytesToRead + "_E5");
            }
            if (receiveBuf[0] == 0x10)
            {
                receiveBuf = ReadData(5);
            }
            else if (receiveBuf[0] != 0xE5)
                throw new ArgumentException("等待E5错误：" + receiveBuf[0].ToString("X2"));
            byte[] dataSend = new byte[6] { 0x10, PlcAddr, 0x00, 0x5C, (byte)(0x5C + PlcAddr), 0x16 };
            //dataSend[0] = 0x10;
            //dataSend[1] = (byte)PlcAddr;//站号
            //dataSend[2] = 0x00;
            //dataSend[3] = 0x5C;
            //dataSend[4] = (byte)(dataSend[3] + dataSend[1]);
            //dataSend[5] = 0x16;
            writedata(dataSend);
        }
        /// <summary>
        /// 单字节写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        internal void write(string address, int value)
        {
            byte[] v = new byte[] { (byte)value };
            write(address, v);
        }
        internal void write(string address, byte[] value)
        {
            bool flag = false;
            checkPort();
            lock (RWlock)
            {
                if (Status == PortStatus.Idle)
                {
                    Status = PortStatus.Writing;
                    flag = true;
                }
            }
            if (flag == false)
            {
                throw new TimeoutException("前一条PLC指令执行中");
            }

            ppiPort.DiscardInBuffer();
            ppiPort.DiscardOutBuffer();
            address = address.ToUpper();
            byte[] tosend = null;
            try
            {
                if (address[0] == 'Q')
                {
                    tosend = WriteQ(PlcAddr, getAddr(address.Substring(1)), value[0]);
                }
                else if (address[0] == 'V' && (address[1] == 'D' || address[1] == 'B' || address[1] == 'W'))
                {
                    if ((address[1] == 'D' && value.Length != 4) || (address[1] == 'B' && value.Length != 1) || (address[1] == 'W' && value.Length != 2))
                        throwException(new Exception("字节长度不匹配 " + address + " 字节长度: " + value.Length));

                    tosend = WriteVBs(PlcAddr, getAddr(address.Substring(2)), value);
                }
                else if (address[0] == 'V')
                {
                    if (address[address.Length - 2] == '.')
                        tosend = WriteVBit(PlcAddr, getAddr(address.Substring(1)), value[0]);
                    else
                        tosend = WriteVBs(PlcAddr, getAddr(address.Substring(1)), value);
                }
                else
                    throwException(new Exception("不支持的写入地址格式 " + address));
            }
            catch (System.Exception ex)
            {
                throwException(ex);
            }
            try
            {
                writedata(tosend);
                checkE5();
                byte[] receiveBuf;

                receiveBuf = ReadData(24);
                if (receiveBuf[0] == 0x68 && receiveBuf[3] == 0x68 &&
                    receiveBuf[1] == receiveBuf[2] && receiveBuf[receiveBuf.Length - 1] == 0x16)
                    if (receiveBuf[17] == 0 && receiveBuf[18] == 0)//倒数第6、7
                    {
                        Status = PortStatus.Idle;
                        //线程继续发送下一个数据
                        //lastCommandTime = DateTime.MinValue;
                    }
                    else
                        throw new Exception("ERR14670--参数写入出错，" + receiveBuf[17].ToString("X2") + receiveBuf[18].ToString("X2"));
                else
                    throw new Exception("ERR16670--数据传输中出错");
            }
            catch (System.Exception ex)
            {
                Status = PortStatus.Idle;
                throw ex;
            }
        }
        byte[] readinternal(byte[] buf, int DataLength)
        {
            writedata(buf);
            Thread.Sleep(30 + buf.Length);
            checkE5();
            byte[] result = null;
            if (DataLength > 0)
            {
                byte[] readinbuf = ReadData(DataLength + 27);
                if (readinbuf[0] == 0x68 && readinbuf[3] == 0x68 &&
                    readinbuf[1] == readinbuf[2] && readinbuf[readinbuf.Length - 1] == 0x16)
                {
                    result = new byte[DataLength];
                    Array.Copy(readinbuf, 25, result, 0, DataLength);
                }
            }
            if (result == null)
                throw new Exception("接收结果过程中出错");
            Status = PortStatus.Idle;
            return result;
        }
        internal byte[] read(string address)
        {
            bool flag = false;
            checkPort();
            lock (RWlock)
            {
                if (Status == PortStatus.Idle)
                {
                    Status = PortStatus.Reading;
                    flag = true;
                }
            }
            if (flag == false)
                throw new TimeoutException("前一条PLC指令执行中");

            ppiPort.DiscardInBuffer();
            ppiPort.DiscardOutBuffer();
            address = address.ToUpper();
            byte[] sendbuf = null;
            int gettingDataLength = 0;
            try
            {
                if (address[0] == 'V' && address[1] == 'D')
                {
                    gettingDataLength = 4;
                    sendbuf = ReadVB(PlcAddr, 4, getAddr(address.Substring(2)));
                }
                else if (address[0] == 'V' && address[1] == 'B')
                {
                    gettingDataLength = 1;
                    sendbuf = ReadVB(PlcAddr, 1, getAddr(address.Substring(2)));
                }
                else if (address[0] == 'V' && address[1] == 'W')
                {
                    gettingDataLength = 2;
                    sendbuf = ReadVB(PlcAddr, 2, getAddr(address.Substring(2)));
                }
                else if (address[0] == 'I')
                {
                    gettingDataLength = 1;
                    sendbuf = ReadI(PlcAddr, getAddr(address.Substring(1)));
                }
                else if (address[0] == 'M')
                {
                    gettingDataLength = 1;
                    sendbuf = ReadM(PlcAddr, getAddr(address.Substring(1)));
                }
                else
                    throw new Exception("ERR19970:不支持的读取地址格式 " + address);

                return readinternal(sendbuf, gettingDataLength);
            }
            catch (System.Exception ex)
            {
                Status = PortStatus.Idle;
                throw ex;
            }
        }
        internal byte[] readV(int address, int len)
        {
            bool flag = false;
            checkPort();
            lock (RWlock)
            {
                if (Status == PortStatus.Idle)
                {
                    Status = PortStatus.Reading;
                    flag = true;
                }
            }
            if (flag == false)
                throw new TimeoutException("前一条PLC指令执行中");
            try
            {
                if (ppiPort.BytesToRead > 0)
                {
                    ppiPort.DiscardInBuffer();
                    ppiPort.DiscardOutBuffer();
                }
                byte[] sendbuf = null;
                sendbuf = ReadVB(PlcAddr, len, address * 8);
                return readinternal(sendbuf, len);
            }
            catch (System.Exception ex)
            {
                Status = PortStatus.Idle;
                throw ex;
            }
        }
        internal void close()
        {
            if (ppiPort != null && ppiPort.IsOpen)
                ppiPort.Close();
        }
        void checkPort()
        {
            if (!ppiPort.IsOpen)
                ppiPort.Open();
            if (!ppiPort.IsOpen)
                throwException(new Exception("串口打开失败"));
        }

        internal delegate void Logdelegate(string str);
        internal Logdelegate LogEvents = null;

        static byte[] WriteQ(int stationNO, int address, int value)
        {
            byte[] data = new byte[38];

            //开始标记符
            data[0] = 0x68;
            data[1] = 0x20;
            data[2] = 0x20;
            data[3] = 0x68;

            //站号
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //功能码，写入
            data[6] = 0x7C;

            //
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x0E;//13、14：命令帧中参数块的长度，上例中从17到30共14个。设为0EH
            data[15] = 0x00;
            data[16] = 0x05;//15、16：命令帧中数据块的长度，
            data[17] = 0x05;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;//变量地址长度，序号21到30的个数，此处设为0AH。
            data[21] = 0x10;//变量地址定义的识别码，固定为10H

            //长度
            //01：1 Bit	
            //02：1 Byte 
            //04：1 Word 
            //06：Double Word
            data[22] = 0x01;

            data[23] = 0x00;

            //个数
            data[24] = 0x01;

            data[25] = 0x00;


            //存储器类型，01：V存储器  00：其它
            data[26] = 0x00;

            //存储器类型
            //04：S	05：SM 		06：AI		07：AQ		1E: C
            //81：I	82：Q		83：M		84：V		1F: T
            data[27] = 0x82;

            //地址，偏移量
            data[28] = (byte)(address / 256 / 256);
            data[29] = (byte)(address / 256);
            data[30] = (byte)(address);


            data[31] = 0x00;
            //如果写入的是位数据这一字节为03，其它则为04
            data[32] = 0x03;
            data[33] = 0x00;

            //位数
            //01: 1 Bit	08: 1 Byte	10H: 1 Word  20H: 1 Double Word   
            data[34] = 0x01;

            //值
            data[35] = (byte)value;

            //效验和
            int j = 0;
            for (int i = 4; i <= 35; i++)
                j = j + data[i];
            data[36] = (byte)(j);


            data[37] = 0x16;
            return data;
        }
        static byte[] ReadVB(int stationNO, int length, int address)
        {
            byte[] data = new byte[33];
            //68 1B 1B 68 02 00 7C 32 01 00 00 00 08 00 0E 00 00 04 01 12 0A 10 02 00 06 00 01 84 00 1F 40 E4 16 
            data[0] = 0x68;
            data[1] = 0x1B;
            data[2] = 0x1B;
            data[3] = 0x68;
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //以前是6C ，现在改7C
            data[6] = 0x7C;
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;

            //这里是08
            data[12] = 0x08;//0x08

            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x00;
            data[17] = 0x04;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;
            data[22] = 0x02;
            data[23] = 0x00;
            data[24] = Convert.ToByte(length);
            data[25] = 0x00;
            data[26] = 0x01;
            data[27] = 0x84;
            data[28] = (byte)(address / 256 / 256);
            data[29] = (byte)(address / 256);
            data[30] = (byte)(address);
            int j = 0;
            for (int i = 4; i <= 30; i++)
            {
                j = j + data[i];
            }
            data[31] = (byte)(j);
            data[32] = 0x16;
            return data;
        }
        static byte[] ReadI(int stationNO, int address)
        {
            byte[] data = new byte[33];
            //68 1B 1B 68 02 00 7C 32 01 00 00 00 08 00 0E 00 00 04 01 12 0A 10 02 00 06 00 01 84 00 1F 40 E4 16 
            data[0] = 0x68;
            data[1] = 0x1B;
            data[2] = 0x1B;
            data[3] = 0x68;
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //以前是6C ，现在改7C
            data[6] = 0x7C;
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;

            //这里是08
            data[12] = 0x00;//0x08

            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x00;
            data[17] = 0x04;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;
            data[22] = 0x01;//
            data[23] = 0x00;
            data[24] = 1;
            data[25] = 0x00;
            data[26] = 0x00;
            data[27] = 0x81;
            data[28] = 0x00;
            data[29] = Convert.ToByte(address / 256);
            data[30] = (byte)(address);
            int j = 0;
            for (int i = 4; i <= 30; i++)
            {
                j = j + data[i];
            }
            data[31] = (byte)(j);
            data[32] = 0x16;
            return data;
        }
        static byte[] ReadM(int stationNO, int address)
        {
            byte[] data = new byte[33];
            //68 1B 1B 68 02 00 7C 32 01 00 00 00 08 00 0E 00 00 04 01 12 0A 10 02 00 06 00 01 84 00 1F 40 E4 16 
            data[0] = 0x68;
            data[1] = 0x1B;
            data[2] = 0x1B;
            data[3] = 0x68;
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //以前是6C ，现在改7C
            data[6] = 0x7C;
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;

            //这里是08
            data[12] = 0x00;//0x08

            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x00;
            data[17] = 0x04;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;
            data[22] = 0x01;//
            data[23] = 0x00;
            data[24] = 1;
            data[25] = 0x00;
            data[26] = 0x00;
            data[27] = 0x83;
            data[28] = Convert.ToByte(address / 256 / 256);
            data[29] = (byte)(address / 256);
            data[30] = (byte)(address);
            int j = 0;
            for (int i = 4; i <= 30; i++)
            {
                j = j + data[i];
            }
            data[31] = (byte)(j);
            data[32] = 0x16;
            return data;
        }
        static byte[] WriteVBs(int stationNO, int address, byte[] value)
        {
            byte[] data = new byte[37 + value.Length];

            //开始标记符
            data[0] = 0x68;
            data[1] = (byte)(0x1F + value.Length);
            data[2] = data[1];
            data[3] = 0x68;

            //站号
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //功能码，写入
            data[6] = 0x7C;

            //
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x0E;//13、14：命令帧中参数块的长度，上例中从17到30共14个。设为0EH
            data[15] = 0x00;
            data[16] = (byte)(0x04 + value.Length);//15、16：命令帧中数据块的长度，
            data[17] = 0x05;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;//变量地址长度，序号21到30的个数，此处设为0AH。
            data[21] = 0x10;//变量地址定义的识别码，固定为10H

            //长度
            //01：1 Bit	
            //02：1 Byte 
            //04：1 Word 
            //06：Double Word
            data[22] = 0x02;

            data[23] = 0x00;

            //个数
            data[24] = (byte)value.Length;

            data[25] = 0x00;


            //存储器类型，01：V存储器  00：其它
            data[26] = 0x01;

            //存储器类型
            //04：S	05：SM 		06：AI		07：AQ		1E: C
            //81：I	82：Q		83：M		84：V		1F: T
            data[27] = 0x84;

            //地址，偏移量
            data[28] = (byte)(address / 256 / 256);
            data[29] = (byte)(address / 256);
            data[30] = (byte)(address);


            data[31] = 0x00;
            //如果写入的是位数据这一字节为03，其它则为04
            data[32] = 0x04;

            //位数
            //01: 1 Bit	08: 1 Byte	10H: 1 Word  20H: 1 Double Word  
            data[33] = (byte)(value.Length / 32);
            data[34] = (byte)(0x08 * value.Length);

            //值
            //data[35] = (byte)value;
            value.CopyTo(data, 35);

            //效验和
            int j = 0;
            for (int i = 4; i < data.Length - 2; i++)
                j = j + data[i];
            data[data.Length - 2] = (byte)(j);

            data[data.Length - 1] = 0x16;
            return data;
        }
        static byte[] WriteVBit(int stationNO, int address, byte value)
        {
            byte[] data = new byte[37 + 1];

            //开始标记符
            data[0] = 0x68;
            data[1] = (byte)(0x1F + 1);
            data[2] = data[1];
            data[3] = 0x68;

            //站号
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //功能码，写入
            data[6] = 0x7C;

            //
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x0E;//13、14：命令帧中参数块的长度，上例中从17到30共14个。设为0EH
            data[15] = 0x00;
            data[16] = (byte)(0x04 + 1);//15、16：命令帧中数据块的长度，
            data[17] = 0x05;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;//变量地址长度，序号21到30的个数，此处设为0AH。
            data[21] = 0x10;//变量地址定义的识别码，固定为10H

            //长度
            //01：1 Bit	
            //02：1 Byte 
            //04：1 Word 
            //06：Double Word
            data[22] = 0x01;

            data[23] = 0x00;

            //个数
            data[24] = 1;

            data[25] = 0x00;


            //存储器类型，01：V存储器  00：其它
            data[26] = 0x01;

            //存储器类型
            //04：S	05：SM 		06：AI		07：AQ		1E: C
            //81：I	82：Q		83：M		84：V		1F: T
            data[27] = 0x84;

            //地址，偏移量
            data[28] = (byte)(address / 256 / 256);
            data[29] = (byte)(address / 256);
            data[30] = (byte)(address);


            data[31] = 0x00;
            //如果写入的是位数据这一字节为03，其它则为04
            data[32] = 0x03;

            //位数
            //01: 1 Bit	08: 1 Byte	10H: 1 Word  20H: 1 Double Word  
            data[33] = 0;
            data[34] = 1;

            //值
            data[35] = value;

            //效验和
            int j = 0;
            for (int i = 4; i < data.Length - 2; i++)
                j = j + data[i];
            data[data.Length - 2] = (byte)(j);

            data[data.Length - 1] = 0x16;
            return data;
        }

        static int getAddr(string addstr)
        {
            int i = 0, j;
            string[] s = addstr.Split(new char[] { '.' });
            i = Convert.ToInt32(s[0]) * 8;
            if (s.Length == 2)
            {
                j = Convert.ToInt32(s[1]);
                if (j > 7 || j < 0)
                    throw new Exception("ERR26248:地址错误 " + addstr);
                i += j;
            }
            return i;
        }
        void throwException(Exception ex)
        {
            //lastCommandTime = DateTime.MinValue;
            Status = PortStatus.Idle;
            throw ex;
        }
        void LogRecord(string str)
        {
            try
            {
                if (LogEvents != null)
                    LogEvents(str);
            }
            catch
            {

            }
        }
    }
}
