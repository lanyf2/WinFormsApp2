using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using S7.Net;


namespace vpc
{
    class PLCSiemens : PlcInterface
    {
        Plc plc;

        public PLCSiemens(string ip)
        {
            if (ip.IsNullOrEmpty() || ip.StartsWith("COM"))
                ip = "192.168.0.11";
            plc = new Plc(CpuType.S71200, ip, 0, 0);
        }

        void Update(object state)
        {
            bool updateflag = false;
            Thread.Sleep(1000);
            WaitLoading();
            plc.Open();
            while (true)
            {
                try
                {
                    updateflag = false;
                    DateTime tm1 = DateTime.Now;
                    if (DataAddrToAdd != null)
                    {
                        DataAddrStore.AddRange(DataAddrToAdd);
                        DataAddrToAdd = null;
                    }
                    if (UpdateDataStore())
                        updateflag = true;
                    LastUpdateTime = DateTime.Now;
                    if (DataStore.Count == 0)
                    {
                        Thread.Sleep(1500);
                        continue;
                    }
                    UpdateRunHdl();
                    if (err != null)
                    {
                        updateflag = true;
                    }
                    if (TestInfoReadHdl())
                        updateflag = true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        bool loginfo = err == null;
                        err = new Exception($"{portname}连接异常：" + ex.Message);
                        if (loginfo)
                        {
                            Program.Loginfo(portname + " " + ex.Message);
                        }
                        StatusUpdatedHdl();
                        ErrHdl(ex);
                    }
                    catch { }
                    Thread.Sleep(500);
                }
            }
        }
        bool TestInfoReadHdl()
        {
            bool infochanged = false;
            if (string.IsNullOrEmpty(TestAddr) == false && TestAddr.Length >= 2)
            {
                object preTestValue = testValue;
                testValue = null;
                ushort addr;
                if (TestAddr[0] == 'M' || TestAddr[0] == 'm')
                {
                    if (ushort.TryParse(TestAddr.Substring(1, TestAddr.Length - 1), out addr))
                    {
                        //var re = modbus.ReadCoils(slaveAddress, (ushort)(2000 + addr), 1);
                        //if (preTestValue is bool == false || (bool)preTestValue != re[0])
                        //    infochanged = true;
                        //testValue = re[0];
                    }
                }
                else if (TestAddr[0] == 'S' || TestAddr[0] == 's')
                {
                    int len = 16;
                    var pos = TestAddr.IndexOf('-');
                    string addrs;
                    if (pos > 1)
                    {
                        addrs = TestAddr.Substring(1, pos - 1);
                        if (ushort.TryParse(TestAddr.Substring(pos + 1, TestAddr.Length - pos - 1), out ushort le))
                        {
                            if (le > 1)
                                len = le;
                        }
                    }
                    else
                        addrs = TestAddr.Substring(1);

                    if (ushort.TryParse(addrs, out addr))
                    {
                        //var re = modbus.ReadHoldingRegisters(slaveAddress, addr, (ushort)len);
                        //ReadDCounts++;
                        //var nre = HCLabel.ToGB2312(re);
                        //if (preTestValue is string == false || (string)preTestValue != nre)
                        //    infochanged = true;
                        //testValue = nre;
                    }

                }
                else if (TestAddr[0] == 'D' || TestAddr[0] == 'd')
                {
                    if (ushort.TryParse(TestAddr.Substring(1, TestAddr.Length - 1), out addr))
                    {
                        //var re = modbus.ReadHoldingRegisters(slaveAddress, addr, 1);
                        //ReadDCounts++;
                        //if (preTestValue is ushort == false || (ushort)preTestValue != re[0])
                        //    infochanged = true;
                        //testValue = re[0];
                    }
                    else
                    {
                        var pos = TestAddr.IndexOf('-');
                        if (pos > 1)
                        {
                            if (ushort.TryParse(TestAddr.Substring(1, pos - 1), out addr))
                            {
                                ushort le;
                                if (ushort.TryParse(TestAddr.Substring(pos + 1, TestAddr.Length - pos - 1), out le))
                                {
                                    //var re = modbus.ReadHoldingRegisters(slaveAddress, addr, le);
                                    //if (preTestValue is ushort[] == false || !IsEqual((ushort[])preTestValue, re))
                                    //{
                                    //    infochanged = true;
                                    //    testValue = ushortToHexString(re);
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            else if (string.IsNullOrEmpty(TestAddr) == false)
            {
                //var re = modbus.ReadHoldingRegisters(slaveAddress, 900, 1);
                //if (re[0] > 0)
                //{
                //    var re2 = modbus.ReadHoldingRegisters(slaveAddress, 901, (ushort)((re[0] + 1) / 2));
                //    testValue = ConvertToStringCmpact(re2, re[0]);
                //}
                //else
                    testValue = null;
            }
            else
                testValue = null;

            return infochanged;
        }

        internal override void InitAddrs(List<int> addrs)
        {
            SortedSet<int> tmpss = new SortedSet<int>();
            for (int j = 0; j < addrs.Count; j++)
                if (false == tmpss.Contains(addrs[j]))
                    tmpss.Add(addrs[j]);

            while (tmpss.Count > 0)
            {
                int len = 1;
                var enu = tmpss.GetEnumerator();
                int startAddr;
                if (enu.MoveNext())
                    startAddr = enu.Current;
                else
                    break;
                if (tmpss.Remove(startAddr) == false)
                {
                    Program.ErrHdl(new Exception("内部错误：20981_" + startAddr + "_" + tmpss.Count));
                    break;
                }
                while (tmpss.Remove(startAddr - 1))
                {
                    len++;
                    startAddr--;
                }
                while (tmpss.Remove(startAddr + len))
                {
                    len++;
                }
                if (startAddr < 0)
                    DataAddrStore.Add(new KeyValuePair<int, ushort>(startAddr + len - 1, (ushort)len));
                else
                {
                    while (len > PackingLength)
                    {
                        DataAddrStore.Add(new KeyValuePair<int, ushort>(startAddr, PackingLength));
                        len -= PackingLength;
                        startAddr += PackingLength;
                    }
                    DataAddrStore.Add(new KeyValuePair<int, ushort>(startAddr, (ushort)len));
                }
            }
            ThreadPool.QueueUserWorkItem(Update);
        }

        internal override void WritePlcInternalAsync(int Internaladdr, ushort val)
        {
            throw new NotImplementedException();
        }

        internal override void WritePlcArrayInternalAsync(int Internaladdr, ushort[] val)
        {
            throw new NotImplementedException();
        }

        internal override void writeHdl()
        {
            throw new NotImplementedException();
        }

        internal override ushort[] ReadPlcInternalSync(int Internaladdr, ushort length)
        {
            if (Internaladdr >= 0)
            {
                var re = plc.ReadBytes(DataType.DataBlock, 1, Internaladdr, length * 2);
                ushort[] bt = new ushort[length];
                Buffer.BlockCopy(re, 0, bt, 0, length);
                return bt;
            }
            return null;
        }

        internal override void WriteSync(int Addr, ushort[] val)
        {
            byte[] bt = new byte[val.Length * 2];
            Buffer.BlockCopy(val, 0, bt, 0, bt.Length);
            if (Addr >= 0)
                plc.WriteBytes(DataType.DataBlock, 0, Addr, bt);
        }

        [DisplayName("状态")]
        public string 状态
        {
            get
            {
                int len = -1;
                if (DataAddrStore != null)
                    len = DataAddrStore.Count;
                //if (err == null)
                //    return string.Format("{6}更新于{0:F1}秒前({1:HH:mm:ss},耗时{2:F0} ms,{3}D,{4}M,{5})", (DateTime.Now - LastUpdateTime).TotalSeconds, DateTime.Now, TimeSpend, ReadDCountsDisp, ReadMCountsDisp, len, portname);
                //else
                    return string.Format("{0}({1:HH:mm:ss})", err.Message, DateTime.Now, portname);
            }
        }
        [DisplayName("测试值地址")]
        public string TestAddr
        {
            get { return testAddr; }
            set { testAddr = value.ToUpper(); }
        }
        [DisplayName("测试值"), TypeConverter(typeof(MyStringBoolConverter))]
        public string TestValue
        {
            get
            {
                if (testValue == null)
                    return "";
                else
                    return testValue.ToString();
            }
            set
            {
                if (TestAddr != null && TestAddr.Length > 1)
                {
                    ushort addr;
                    if (TestAddr[0] == 'M' || TestAddr[0] == 'm')
                    {
                        bool val = false;
                        ushort valus;
                        if (ushort.TryParse(TestAddr.Substring(1, TestAddr.Length - 1), out addr))
                        {
                            //if (bool.TryParse(value, out val))
                            //    modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + addr), val);
                            //else if (value is string && ushort.TryParse(value, out valus))
                            //    modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + addr), valus > 0);

                        }
                    }
                    else if (TestAddr[0] == 'D' || TestAddr[0] == 'd')
                    {
                        ushort val = 0;
                        if (ushort.TryParse(TestAddr.Substring(1, TestAddr.Length - 1), out addr))
                        {
                            //if (value is string && ushort.TryParse(value, out val))
                            //    modbus.WriteSingleRegister(slaveAddress, addr, val);
                        }
                    }
                }
            }
        }
    }
}
