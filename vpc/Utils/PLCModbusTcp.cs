using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Modbus;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace vpc
{
    internal class PlcModbusTcp : PlcInterface, ICustomTypeDescriptor
    {
        Modbus.Device.ModbusIpMaster modbus;
        byte slaveAddress = 1;
        internal override ushort PackingLength { get { return 128; } }
        IPEndPoint IpDevice;
        TcpClient tcp;

        internal PlcModbusTcp(string address, int? baudrate = null)
        {

            IpDevice = address.TryParseIPEndPoint();
            if (IpDevice == null)
                err = new Exception("地址非法");
            else
                err = new Exception("初始化中");
        }
        internal void WritePlcAsync(ushort addr, ushort val)
        {
            WriteCmds.Enqueue(new writeCmd(addr, val));
        }
        internal void WritePlcAsync(ushort addr, string val)
        {
            WriteCmds.Enqueue(new writeCmd(addr, val));
        }
        internal void WritePlcAsync(ushort addr, bool val)
        {
            WriteCmds.Enqueue(new writeCmd(addr, val));
        }
        internal void WritePlcAsync(ushort addr, bool[] val)
        {
            WriteCmds.Enqueue(new writeCmd(addr, val));
        }
        internal override void WritePlcArrayInternalAsync(int Internaladdr, ushort[] val)
        {
            if (Internaladdr >= 0)
                WriteCmds.Enqueue(new writeCmd((ushort)Internaladdr, val));
            else
                throw new NotImplementedException();
        }
        internal override void WritePlcInternalAsync(int Internaladdr, ushort val)
        {
            if (Internaladdr >= 0)
                WriteCmds.Enqueue(new writeCmd((ushort)Internaladdr, val));
            else if (Internaladdr < -2000)
                WriteCmds.Enqueue(new writeCmd((ushort)(-2000 - Internaladdr), val == 0 ? false : true));
        }
        internal override void InitAddrs(List<int> addrs)
        {
            DataStore = new Dictionary<int, ushort>();
            DataAddrStore = new List<KeyValuePair<int, ushort>>();

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
            if (IpDevice != null)
                ThreadPool.QueueUserWorkItem(Update);
        }
        internal override void writeHdl()
        {
            while (WriteCmds.TryPeek(out writeCmd wcmd))
            {
                if (wcmd.val is bool)
                {
                    modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + wcmd.addr), (bool)wcmd.val);
                    ReadMCounts++;
                }
                else if (wcmd.val is ushort)
                {
                    modbus.WriteSingleRegister(slaveAddress, wcmd.addr, (ushort)wcmd.val);
                    ReadDCounts++;
                }
                else if (wcmd.val is bool[])
                {
                    bool[] data = wcmd.val as bool[];
                    if (data != null && data.Length > 0)
                    {
                        modbus.WriteMultipleCoils(slaveAddress, (ushort)(2000 + wcmd.addr), data);
                        ReadMCounts += data.Length;
                    }
                }
                else if (wcmd.val is ushort[])
                {
                    ushort[] data = wcmd.val as ushort[];
                    if (data != null && data.Length > 0)
                    {
                        modbus.WriteMultipleRegisters(slaveAddress, wcmd.addr, data);
                        ReadDCounts += data.Length;
                    }
                }
                else if (wcmd.val is string)
                {
                    ushort[] data = StringToushort(wcmd.val as string);
                    if (data != null && data.Length > 0)
                    {
                        modbus.WriteMultipleRegisters(slaveAddress, wcmd.addr, data);
                        ReadDCounts += data.Length;
                    }
                }
                else if(wcmd.val is float f)
                {
                    WriteSync(wcmd.addr, f);
                }
                WriteCmds.TryDequeue(out wcmd);
                Thread.Sleep(10);
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
                        var re = modbus.ReadCoils(slaveAddress, (ushort)(2000 + addr), 1);
                        ReadMCounts++;
                        if (preTestValue is bool == false || (bool)preTestValue != re[0])
                            infochanged = true;
                        testValue = re[0];
                    }
                }
                else if (TestAddr[0] == 'D' || TestAddr[0] == 'd')
                {
                    if (ushort.TryParse(TestAddr.Substring(1, TestAddr.Length - 1), out addr))
                    {
                        var re = modbus.ReadHoldingRegisters(slaveAddress, addr, 1);
                        ReadDCounts++;
                        if (preTestValue is ushort == false || (ushort)preTestValue != re[0])
                            infochanged = true;
                        testValue = re[0];
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
                                    var re = ReadPlcInternalSync(addr, le);
                                    if (preTestValue is ushort[] == false || !IsEqual((ushort[])preTestValue, re))
                                    {
                                        infochanged = true;
                                        testValue = ushortToHexString(re);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (string.IsNullOrEmpty(TestAddr) == false)
            {
                var re = modbus.ReadHoldingRegisters(slaveAddress, 900, 1);
                if (re[0] > 0)
                {
                    var re2 = modbus.ReadHoldingRegisters(slaveAddress, 901, (ushort)((re[0] + 1) / 2));
                    testValue = ConvertToStringCmpact(re2, re[0]);
                }
                else
                    testValue = null;
            }
            else
                testValue = null;

            return infochanged;
        }
        void WaitHdl(int wait)
        {

        }
        void Update(object state)
        {
            int runct = 0;
            err = new Exception("等待初始化");
            WaitLoading();
            while (true)
            {
                try
                {
                    if (tcp == null)
                    {
                        err = new Exception("连接中：" + IpDevice);
                        tcp = new TcpClient();
                        tcp.ReceiveTimeout = 300;
                        tcp.SendTimeout = 300;
                        tcp.Connect(IpDevice);
                        err = new Exception("已连接：" + IpDevice);
                        modbus = Modbus.Device.ModbusIpMaster.CreateIp(tcp);
                    }

                    bool updateflag = false;
                    ReadDCounts = ReadMCounts = 0;
                    DateTime tm1 = DateTime.Now;
                    if (DataAddrToAdd != null)
                    {
                        DataAddrStore.AddRange(DataAddrToAdd);
                        DataAddrToAdd = null;
                    }
                    if (ReInitAddrsFlag)
                        ReInitAddrsExec();
                    if (UpdateDataStore())
                        updateflag = true;
                    LastUpdateTime = DateTime.Now;

                    UpdateRunHdl();

                    if (err != null)
                    {
                        updateflag = true;
                    }
                    if (TestInfoReadHdl())
                        updateflag = true;
                    err = null;
                    ReadDCountsDisp = ReadDCounts;
                    ReadMCountsDisp = ReadMCounts;
                    TimeSpend = (DateTime.Now - tm1).TotalMilliseconds;
                    runct++;
                    if (updateflag)
                        StatusUpdatedHdl();
                }
                catch (Exception ex)
                {
                    runct = 0;
                    try
                    {
                        err = new Exception("PLC连接异常：" + ex.Message);
                        if (err == null)
                            Program.Loginfo(ex.Message);
                        if (tcp != null)
                        {
                            tcp.Close();
                            tcp = null;
                        }
                        StatusUpdatedHdl();
                        ErrHdl(ex);
                    }
                    catch { }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(10);
            }
        }

        [DisplayName("状态")]
        public string 状态
        {
            get
            {
                if (err == null)
                    return string.Format("更新于{0:F1}秒前({1:HH:mm:ss},耗时{2:F0} ms,{3}D,{4}M)", (DateTime.Now - LastUpdateTime).TotalSeconds, DateTime.Now, TimeSpend, ReadDCountsDisp, ReadMCountsDisp);
                else
                    return string.Format("连接异常：{0}({1:HH:mm:ss})", err.Message, DateTime.Now);
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
                            if (bool.TryParse(value, out val))
                                modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + addr), val);
                            else if (value is string && ushort.TryParse(value, out valus))
                                modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + addr), valus > 0);
                    }
                    else if (TestAddr[0] == 'D' || TestAddr[0] == 'd')
                    {
                        ushort val = 0;
                        if (ushort.TryParse(TestAddr.Substring(1, TestAddr.Length - 1), out addr))
                        {
                            if (value is string && ushort.TryParse(value, out val))
                                modbus.WriteSingleRegister(slaveAddress, addr, val);
                        }
                    }
                }
            }
        }
        #region ICustomTypeDescriptor 成员
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }
        public object GetEditor(System.Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        public EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
        public virtual PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(this);
            if (DataStore == null || DataStore.Count == 0)
                return pdc;
            List<PropertyDescriptor> props = new List<PropertyDescriptor>(DataStore.Count + pdc.Count);
            for (int i = 0; i < pdc.Count; i++)
                props.Add(pdc[i]);
            var re = DataStore.ToArray();
            Array.Sort(re, new DataStoreComparer());
            for (int i = 0; i < re.Length; i++)
            {
                string name = AddrInfo.TryGetValue(re[i].Key);
                if (re[i].Key < 0)
                {
                    props.Add(new XPropDescriptor(new XProp(string.Format("{1}M{0}", -2000 - re[i].Key, name), re[i].Value == 0 ? false : true, valueChangedHdl), attributes));
                }
                else
                {
                    props.Add(new XPropDescriptor(new XProp(string.Format("{1}D{0}", re[i].Key, name), re[i].Value, valueChangedHdl), attributes));
                }
            }
            //props[i + pdc.Count] = new XPropDescriptor(new XProp(string.Format("工具{0}", i + 1), string.Empty, null, null, new ExpandableObjectConverter()), attributes);
            return new PropertyDescriptorCollection(props.ToArray());
        }
        int ReadDCounts, ReadMCounts;
        int ReadDCountsDisp, ReadMCountsDisp;
        double TimeSpend;

        void valueChangedHdl(XProp xp)
        {
            if (modbus != null && xp != null && xp.Name != null)
            {
                ushort addr;
                ushort valus;
                int index = xp.Name.LastIndexOf('D');
                string value = xp.Value as string;
                if (index >= 0)
                {
                    ushort val;
                    if (ushort.TryParse(xp.Name.Substring(index + 1, xp.Name.Length - index - 1), out addr))
                    {
                        if (xp.Value is ushort)
                            modbus.WriteSingleRegister(slaveAddress, addr, (ushort)xp.Value);
                        else if (ushort.TryParse(value, out val))
                            modbus.WriteSingleRegister(slaveAddress, addr, val);
                    }
                }
                else
                {
                    index = xp.Name.LastIndexOf('M');
                    if (index >= 0)
                    {
                        bool val = false;
                        if (ushort.TryParse(xp.Name.Substring(index + 1, xp.Name.Length - index - 1), out addr))
                            if (value == null)
                            {
                                bool? v = xp.Value as bool?;
                                if (v != null)
                                    modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + addr), v.Value);
                            }
                            else if (bool.TryParse(value, out val))
                                modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + addr), val);
                            else if (ushort.TryParse(value, out valus))
                                modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + addr), valus > 0);
                    }
                }
            }
        }
        internal override ushort[] ReadPlcInternalSync(int Internaladdr, ushort length)
        {
            if (modbus != null && length > 0)
            {
                if (Internaladdr >= 0)
                {
                    if (length < PackingLength)
                    {
                        var re = modbus.ReadHoldingRegisters(slaveAddress, (ushort)Internaladdr, length);
                        ReadDCounts += length;
                        return re;
                    }
                    else
                    {
                        ushort[] re = new ushort[length];
                        for (int i = 0; i < length; i += PackingLength)
                        {
                            if (length - i > PackingLength)
                            {
                                var re2 = modbus.ReadHoldingRegisters(slaveAddress, (ushort)(Internaladdr + i), PackingLength);
                                ReadDCounts += PackingLength;
                                Array.Copy(re2, 0, re, i, re2.Length);
                            }
                            else
                            {
                                var re2 = modbus.ReadHoldingRegisters(slaveAddress, (ushort)(Internaladdr + i), (ushort)(length - i));
                                ReadDCounts += re2.Length;
                                Array.Copy(re2, 0, re, i, re2.Length);
                            }
                        }
                        return re;
                    }
                }
                else
                {
                    var re = modbus.ReadCoils(slaveAddress, (ushort)-Internaladdr, length);
                    if (re == null)
                        return null;
                    ReadMCounts += re.Length;
                    ushort[] re2 = new ushort[re.Length];
                    for (int i = 0; i < re.Length; i++)
                    {
                        re2[i] = (ushort)(re[i] ? 1 : 0);
                    }
                    return re2;
                }
            }
            return null;
        }
        internal override void WriteSync(int Addr, ushort[] val)
        {
            if (modbus != null && val != null && val.Length > 0)
            {
                if (Addr >= 0)
                {
                    if (val.Length < PackingLength)
                    {
                        modbus.WriteMultipleRegisters(slaveAddress, (ushort)Addr, val);
                    }
                    else
                    {
                        for (int i = 0; i < val.Length; i += PackingLength)
                        {
                            if (val.Length - i > PackingLength)
                            {
                                ushort[] re = new ushort[PackingLength];
                                Array.Copy(val, i, re, 0, PackingLength);
                                modbus.WriteMultipleRegisters(slaveAddress, (ushort)(Addr + i), re);
                            }
                            else
                            {
                                ushort[] re = new ushort[val.Length - i];
                                Array.Copy(val, i, re, 0, re.Length);
                                modbus.WriteMultipleRegisters(slaveAddress, (ushort)(Addr + i), re);
                            }
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
