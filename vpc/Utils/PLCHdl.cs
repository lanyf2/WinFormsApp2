using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Modbus;
using System.ComponentModel;

namespace vpc
{
    public class PlcModbus : PlcInterface, ICustomTypeDescriptor
    {
        Modbus.Device.ModbusSerialMaster modbus;
        int ReadDCounts, ReadMCounts;
        double TimeSpend;
        byte slaveAddress = 1;
        internal override ushort PackingLength { get { return 32; } }

        public PlcModbus(string port, int? baudrate = null)
        {
            //var ports = System.IO.Ports.SerialPort.GetPortNames();
            //if (Array.Find(ports, new Predicate<string>((string str) => str == port)) == null)
            //    if (ports.Length > 0)
            //        port = ports[0];
            int Baudrate = 19200;
            if (baudrate > 1000)
                Baudrate = baudrate.Value;
            System.IO.Ports.SerialPort sport = new System.IO.Ports.SerialPort(port, Baudrate);
            portname = port;
            if (port.EndsWith("M0"))
            {
                var pts = System.IO.Ports.SerialPort.GetPortNames();
                Exception mex = null;
                foreach (var item in pts)
                {
                    try
                    {
                        sport.Close();
                        sport.ReadTimeout = 400;
                        sport.WriteTimeout = 400;
                        sport.PortName = item;
                        sport.Open();
                        modbus = Modbus.Device.ModbusSerialMaster.CreateRtu(sport);

                        var re = ReadPlcInternalSync(10, 1);
                        if (re != null && re.Length == 1)
                        {
                            Program.MsgBox(sport.PortName);
                            mex = null;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        mex = ex;
                    }
                }
                if (mex != null)
                    Program.MsgBox("Err:" + mex.Message);
            }

            try
            {
                sport.Open();
            }
            catch
            {
            }
            modbus = Modbus.Device.ModbusSerialMaster.CreateRtu(sport);
            modbus.Transport.ReadTimeout = 500;
            modbus.Transport.WriteTimeout = 500;
            err = new Exception("初始化中");
        }
        internal override void InitAddrs(List<int> addrs)
        {
            AddAddrInfo();
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

        void Update(object state)
        {
            int runct = 0;
            Thread.Sleep(1000);
            WaitLoading();
            while (true)
            {
                try
                {
                    bool updateflag = false;
                    ReadDCounts = ReadMCounts = 0;
                    DateTime tm1 = DateTime.Now;
                    if (DataAddrToAdd != null)
                    {
                        DataAddrStore.AddRange(DataAddrToAdd);
                        DataAddrToAdd = null;
                    }
                    //if (portname == "COM3")
                    //{
                    //    Program.MsgBox("com3");
                    //}
                    if (ReInitAddrsFlag)
                        ReInitAddrsExec();
                    if (UpdateDataStore())
                        updateflag = true;
                    LastUpdateTime = DateTime.Now;
                    if (DataStore.Count == 0)
                        return;
                    UpdateRunHdl();
                    

                    //if ((runct & 0x07) == 1)
                    //    AssertInitBuffer();

                    if (err != null)
                    {
                        updateflag = true;
                    }
                    if (TestInfoReadHdl())
                        updateflag = true;
                    err = null;
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
                Thread.Sleep(5);
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
                        var re = modbus.ReadHoldingRegisters(slaveAddress, addr, (ushort)len);
                        ReadDCounts++;
                        var nre = ToGB2312(re);
                        if (preTestValue is string == false || (string)preTestValue != nre)
                            infochanged = true;
                        testValue = nre;
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
                                    var re = modbus.ReadHoldingRegisters(slaveAddress, addr, le);
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

        [DisplayName("状态")]
        public string 状态
        {
            get
            {
                int len = -1;
                if (DataAddrStore != null)
                    len = DataAddrStore.Count;
                if (err == null)
                    return string.Format("{6}更新于{0:F1}秒前({1:HH:mm:ss},耗时{2:F0} ms,{3}D,{4}M,{5})", (DateTime.Now - LastUpdateTime).TotalSeconds, DateTime.Now, TimeSpend, ReadDCounts, ReadMCounts, len, portname);
                else
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
                WriteCmds.TryDequeue(out wcmd);
                Thread.Sleep(100);
            }
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
    }
    public abstract class PlcInterface
    {
        internal const int AddrBase = 1000;
        internal string portname;
        internal Dictionary<int, string> AddrInfo = new Dictionary<int, string>();
        internal Dictionary<int, ushort> DataStore = new Dictionary<int, ushort>();
        internal List<KeyValuePair<int, ushort>> DataAddrStore = new List<KeyValuePair<int, ushort>>();
        internal List<KeyValuePair<int, ushort>> DataAddrToAdd;
        internal Exception err = null;
        internal object testValue;
        internal string testAddr;
        internal bool InitBufferFlag = true;
        internal System.Collections.Concurrent.ConcurrentQueue<writeCmd> WriteCmds = new System.Collections.Concurrent.ConcurrentQueue<writeCmd>();
        protected DateTime LastUpdateTime = DateTime.MinValue;
        internal event EventHandler StatusUpdated;
        internal event Action<Exception> OnErr;
        internal event Action<Dictionary<int, ushort>> UpdateRun;
        protected void StatusUpdatedHdl()
        {
            StatusUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal virtual void AssertInitBuffer()
        {
            if (Settings.Default.初始化写入 != null && InitBufferFlag)
            {
                for (int i = 0; i < Settings.Default.初始化写入.Length / 2; i++)
                {
                    if (DataStore != null && DataStore.TryGetValue(Settings.Default.初始化写入[i * 2], out ushort val) &&
                        val == Settings.Default.初始化写入[i * 2 + 1])
                        continue;
                    WritePlcInternalAsync(Settings.Default.初始化写入[i * 2], Settings.Default.初始化写入[i * 2 + 1]);
                }
            }
            if (Settings.Default.初始化写入2 != null && Settings.Default.初始化写入2地址 != null)
            {
                int len = Math.Min(Settings.Default.初始化写入2.Length, Settings.Default.初始化写入2地址.Length);
                for (int i = 0; i < len; i++)
                {
                    if (string.IsNullOrEmpty(Settings.Default.初始化写入2[i]) == false)
                        WritePlcInternalAsync(Settings.Default.初始化写入2地址[i], string.Format(Settings.Default.初始化写入2[i], DateTime.Now));
                }
            }
        }
        internal static void WaitLoading()
        {
            for (int i = 0; i < 100; i++)
            {
                if (Cognex.VisionPro.JobManager.Inited)
                    return;
                else
                    Thread.Sleep(100);
            }
        }

        internal void AddAddrInfo()
        {
            AddrInfo.Add(AddrBase + 0, "工位1角度");
            AddrInfo.Add(AddrBase + 2, "工位2角度");
            AddrInfo.Add(AddrBase + 4, "工位3角度");
            AddrInfo.Add(AddrBase + 6, "工位4角度");
            AddrInfo.Add(AddrBase + 8, "工位5角度");
            AddrInfo.Add(AddrBase + 10, "NG角度");
            AddrInfo.Add(AddrBase + 12, "OK角度");
            AddrInfo.Add(AddrBase + 14, "物料大小角度");

            AddrInfo.Add(AddrBase + 16, "吹气时间");
            AddrInfo.Add(AddrBase + 17, "缺料时间");
            AddrInfo.Add(AddrBase + 18, "加料时间");
            AddrInfo.Add(AddrBase + 25, "物料大小脉冲");

            AddrInfo.Add(AddrBase - 50, "角度实时值");
        }

        internal virtual ushort PackingLength { get { return 88; } }
        internal abstract void InitAddrs(List<int> addrs);
        protected bool ReInitAddrsFlag = false;
        internal void ReInitAddrs()
        {
            ReInitAddrsFlag = true;
        }
        protected void ReInitAddrsExec()
        {
            if (DataAddrStore != null && DataAddrStore.Count > 1)
            {
                SortedSet<int> tmpss = new SortedSet<int>();
                var dataAddrStoreTmp = new List<KeyValuePair<int, ushort>>();
                if (DataStore == null)
                    DataStore = new Dictionary<int, ushort>();
                foreach (var item in DataAddrStore)
                {
                    for (int j = 0; j < item.Value; j++)
                    {
                        int addr;
                        if (item.Key >= 0)
                            addr = item.Key + j;
                        else
                            addr = item.Key - j;
                        if (false == tmpss.Contains(addr))
                        {
                            tmpss.Add(addr);
                            if (DataStore.ContainsKey(addr) == false)
                                DataStore.Add(addr, 0);
                        }
                    }
                }

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
                        Program.ErrHdl(new Exception("内部错误：71495_" + startAddr + "_" + tmpss.Count));
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
                        dataAddrStoreTmp.Add(new KeyValuePair<int, ushort>(startAddr + len - 1, (ushort)len));
                    else
                    {
                        while (len > PackingLength)
                        {
                            dataAddrStoreTmp.Add(new KeyValuePair<int, ushort>(startAddr, PackingLength));
                            len -= PackingLength;
                            startAddr += PackingLength;
                        }
                        dataAddrStoreTmp.Add(new KeyValuePair<int, ushort>(startAddr, (ushort)len));
                    }
                }
                DataAddrStore = dataAddrStoreTmp;
            }
            ReInitAddrsFlag = false;
        }
        internal virtual void WritePlcInternalAsync(int Internaladdr, string val)
        {
            WritePlcArrayInternalAsync(Internaladdr, StringToushort(val));
        }
        internal virtual void WritePlcInternalAsync(int Internaladdr, ushort[] val)
        {
            WritePlcArrayInternalAsync(Internaladdr, val);
        }
        internal abstract void WritePlcInternalAsync(int Internaladdr, ushort val);
        internal virtual void WritePlcInternalAsync(int Internaladdr, float val)
        {
            WriteCmds.Enqueue(new writeCmd((ushort)Internaladdr, val));
        }
        internal abstract void WritePlcArrayInternalAsync(int Internaladdr, ushort[] val);
        internal abstract ushort[] ReadPlcInternalSync(int Internaladdr, ushort length);
        internal virtual ushort[] ReadPlcInternalSync(int Internaladdr, ushort length, int retry)
        {
            if (retry <= 0)
                retry = 1;
            Exception err = new Exception("无数据");
            for (int i = 0; i < retry; i++)
            {
                try
                {
                    return ReadPlcInternalSync(Internaladdr, length);
                }
                catch (Exception e)
                {
                    err = e;
                }
            }
            throw err;

        }

        public static string ToGB2312(ushort[] t)
        {
            if (t == null || t.Length == 0)
                return null;
            byte[] ary = new byte[t.Length * 2];
            for (int i = 0; i < t.Length; i++)
            {
                ary[i * 2] = (byte)(t[i] & 0xFF);
                ary[i * 2 + 1] = (byte)(t[i] >> 8);
            }
            int len = -1;
            for (int i = 0; i < ary.Length; i++)
            {
                if (ary[i] != 0)
                {
                    len = i;
                }
                else
                    break;
            }
            len++;
            if (len == 0)
                return "";
            var cd = Encoding.GetEncoding("gb2312");
            return cd.GetString(ary, 0, len);
        }
        public static string ToGB2312(ushort[] t, int startAddr, int MaxLen)
        {
            if (t == null || t.Length == 0)
                return null;
            if (startAddr >= t.Length - 1)
                return null;
            MaxLen = Math.Min(t.Length - startAddr, MaxLen);
            while (MaxLen > 0)
            {
                if (t[startAddr + MaxLen - 1] == 0x2020 || t[startAddr + MaxLen - 1] == 0)
                {
                    MaxLen--;
                }
                else
                    break;
            }
            if (MaxLen <= 0)
                return null;
            byte[] ary = new byte[MaxLen * 2];
            for (int i = 0; i < MaxLen; i++)
            {
                ary[i * 2] = (byte)(t[i + startAddr] & 0xFF);
                ary[i * 2 + 1] = (byte)(t[i + startAddr] >> 8);
            }
            int len = -1;
            for (int i = 0; i < ary.Length; i++)
            {
                if (ary[i] != 0)
                {
                    len = i;
                }
                else
                    break;
            }
            len++;
            if (len == 0)
                return "";
            var cd = Encoding.GetEncoding("gb2312");
            return cd.GetString(ary, 0, len);
        }
        internal static string GetString2(Dictionary<int, ushort> d, int addr, int len)
        {
            ushort[] ary = new ushort[len];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len / 2; i++)
            {
                var val = d[addr + i];
                sb.Append((char)(val & 0xFF));
                sb.Append((char)(val >> 8));
            }
            if (len % 2 == 1)
            {
                var val = d[addr + len / 2];
                sb.Append((char)(val & 0xFF));
            }
            return sb.ToString();
        }
        internal static string GetStringGB2312(Dictionary<int, ushort> d, int addr, int len)
        {
            try
            {
                ushort[] ary = new ushort[len];
                for (int i = 0; i < len; i++)
                {
                    ary[i] = d[addr + i];
                }
                return ToGB2312(ary);
            }
            catch (Exception ex)
            {
                return string.Format("{0}[{1},{2}]", ex.Message, addr, len);
            }
        }
        static internal string GetStringUnicode(Dictionary<int, ushort> d, int addr, int len)
        {
            try
            {
                if (len <= 0)
                    return null;
                byte[] ary = new byte[len * 2];
                int len2 = -1;
                for (int i = 0; i < len; i++)
                {
                    d.TryGetValue(addr + i, out ushort val);
                    ary[i * 2] = (byte)(val & 0xFF);
                    ary[i * 2 + 1] = (byte)(val >> 8);
                    if (val != 0)
                        len2 = i;
                    else
                        break;
                }

                len2++;
                if (len2 == 0)
                    return "";
                else
                    return Encoding.Unicode.GetString(ary, 0, len2 * 2);
            }
            catch (Exception ex)
            {
                return string.Format("{0}[{1},{2}]", ex.Message, addr, len);
            }
        }
        internal static string GetHex(Dictionary<int, ushort> d, int addr, int len)
        {
            try
            {
                ushort[] ary = new ushort[len];
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < len; i++)
                {
                    sb.Append(d[addr + i].ToString("X2")).Append('-');
                }
                if (sb.Length > 0)
                    return sb.ToString(0, sb.Length - 1);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return string.Format("{0}[{1},{2}]", ex.Message, addr, len);
            }
        }
        internal static double GetFloat(Dictionary<int, ushort> d, int addr)
        {
            try
            {
                if (TryGetValue(d, (ushort)addr, NumType.Float, out double val))
                    return val;
            }
            catch
            {
            }
            return double.NaN;
        }
        public enum NumType
        {
            UnSignedWord, SignedWord, UnSignedDWord, SignedDWord, Float, Ascii, AsciiCompact, Hex, GB2312
        }
        public static bool TryGetValue(Dictionary<int, ushort> DataStore, ushort add, NumType wdTp, out double val)
        {
            ushort val0, val1;
            val = 0;
            if (DataStore.TryGetValue(add, out val0) == false)
                return false;
            if (wdTp == NumType.SignedWord)
            {
                if (val0 > short.MaxValue)
                    val = val0 - ushort.MaxValue - 1;
                else
                    val = val0;
                return true;
            }
            else if (wdTp == NumType.UnSignedWord)
            {
                val = val0;
                return true;
            }
            else if (DataStore.TryGetValue(add + 1, out val1) == false)
                return false;
            else
            {
                uint v = ((uint)val1 << 16) + val0;
                if (wdTp == NumType.SignedDWord)
                {
                    if (v > int.MaxValue)
                        val = v - int.MaxValue - 1;
                    else
                        val = v;
                    return true;
                }
                else if (wdTp == NumType.UnSignedDWord)
                {
                    val = v;
                    return true;
                }
                else
                {
                    byte[] bt = new byte[4] { (byte)val0, (byte)(val0 >> 8), (byte)val1, (byte)(val1 >> 8) };
                    val = BitConverter.ToSingle(bt, 0);
                    return true;
                }
            }
        }
        internal static string GetString(Dictionary<int, ushort> d, int addr, int len)
        {
            try
            {
                for (int i = len - 1; i >= 0; i--)
                    if (d[addr + i] == 0) len--;
                if (len == 0)
                    return null;
                ushort[] ary = new ushort[len];
                for (int i = 0; i < len; i++)
                {
                    ary[i] = d[addr + i];
                }
                return ushortToString(ary);
            }
            catch (Exception ex)
            {
                return string.Format("{0}[{1},{2}]", ex.Message, addr, len);
            }
        }
        internal static string GetString(ushort[] array, int addr, int len)
        {
            if (array == null || array.Length == 0 || len == 0)
                return null;
            if (addr >= array.Length)
                return null;
            if (addr + len > array.Length)
                len = array.Length - addr;
            StringBuilder sb = new StringBuilder(array.Length + 3);
            if (array[0] > 255)
                for (int i = 0; i < len; i++)
                {
                    sb.Append((char)(array[addr + i] & 0xFF));
                    sb.Append((char)(array[addr + i] >> 8));
                }
            else
                for (int i = 0; i < len; i++)
                    sb.Append((char)array[addr + i]);
            while (sb.Length > 0)
            {
                if (sb[sb.Length - 1] == '\0')
                    sb.Remove(sb.Length - 1, 1);
                else
                    break;
            }
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] < 0x20)
                    sb[i] = '?';
            }
            return sb.ToString();
        }
        internal static string ushortToHexString(ushort[] array)
        {
            if (array == null || array.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder(array.Length + 3);
            for (int i = 0; i < array.Length; i++)
                sb.Append(array[i].ToString("X")).Append('-');
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        internal static string ushortToString(ushort[] array)
        {
            if (array == null || array.Length == 0)
                return null;
            return GetString(array, 0, array.Length);
        }
        internal static ushort[] StringToushort(string array)
        {
            if (array == null || array.Length == 0)
                return null;
            ushort[] re = new ushort[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                re[i] = array[i];
            }
            return re;
        }
        internal static bool IsEqual(Dictionary<int, ushort> DataStore1, Dictionary<int, ushort> DataStore2)
        {
            if (DataStore1 == null || DataStore2 == null)
                return false;
            if (DataStore1.Count != DataStore2.Count)
                return false;
            var enu = DataStore1.GetEnumerator();
            ushort val;
            while (enu.MoveNext())
            {
                if (DataStore2.TryGetValue(enu.Current.Key, out val) == false || val != enu.Current.Value)
                    return false;
            }
            return true;
        }
        internal static bool IsEqual(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            if (b2.Length != b1.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i]) return false;
            return true;
        }
        internal static bool IsEqual(double[] b1, double[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            if (b2.Length != b1.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i]) return false;
            return true;
        }
        internal static bool IsEqual(float[] b1, float[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            if (b2.Length != b1.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i]) return false;
            return true;
        }
        internal static bool IsEqual(bool[] b1, bool[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            if (b2.Length != b1.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i]) return false;
            return true;
        }
        internal static bool IsEqual(ushort[] b1, ushort[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            if (b2.Length != b1.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i]) return false;
            return true;
        }
        internal static bool IsEqual(string[] b1, string[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            if (b2.Length != b1.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i]) return false;
            return true;
        }
        internal static int? ParseModbusAddr(string addr)
        {
            if (addr != null && addr.Length > 1)
            {
                ushort add;
                if (addr[0] == 'D')
                {
                    if (ushort.TryParse(addr.Substring(1), out add))
                        return add;
                }
                else if (addr[0] == 'M')
                {
                    if (ushort.TryParse(addr.Substring(1), out add))
                        return -2000 - add;
                }
                else if (ushort.TryParse(addr, out add))
                    return add;
            }
            return null;
        }
        internal void UpdateRunHdl()
        {
            try
            {
                UpdateRun?.Invoke(DataStore);
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        static internal string ConvertToStringCmpact(ushort[] buffer, int len)
        {
            if (buffer == null || len <= 0 || buffer.Length == 0)
                return null;
            int size = Math.Min(len, buffer.Length * 2);
            byte[] buf = new byte[size];
            for (int i = 0; i < size; i += 2)
            {
                buf[i] = (byte)buffer[i / 2];
                if (i + 1 < size)
                    buf[i + 1] = (byte)(buffer[i / 2] >> 8);
            }
            return Encoding.GetEncoding("gb2312").GetString(buf);
        }

        internal void ErrHdl(Exception ex)
        {
            try
            {
                OnErr?.Invoke(new Exception(portname + ex?.Message));
            }
            catch
            { }
        }
        internal abstract void writeHdl();
        internal virtual bool UpdateDataStore()
        {
            bool updateflag = false;
            foreach (var item in DataAddrStore)
            {
                writeHdl();
                if (item.Key >= 0)
                {
                    var re = ReadPlcInternalSync(item.Key, item.Value);
                    ushort vpre;
                    for (int i = 0; i < re?.Length; i++)
                    {
                        if (DataStore.TryGetValue(item.Key + i, out vpre))
                        {
                            if (vpre != re[i])
                            {
                                updateflag = true;
                                DataStore[item.Key + i] = re[i];
                            }
                        }
                        else
                            DataStore.Add(item.Key + i, re[i]);
                    }
                }
                else if (item.Key < 0)
                {
                    var re = ReadPlcInternalSync(item.Key, item.Value);
                    for (int i = 0; i < re.Length; i++)
                    {
                        if (updateflag == false && DataStore[item.Key - i] != re[i])
                            updateflag = true;
                        DataStore[item.Key - i] = re[i];
                    }
                }
                Thread.Sleep(1);
            }
            if (Cognex.VisionPro.JobManager.initflag)
            {
                if (DateTime.Now.Second != dtmem.Second)
                {
                    dtmem = DateTime.Now;
                    WriteSync(AddrBase + 42, 0);
                    if (FormPLC.位置参数 != null)
                        WriteSync(AddrBase, FormPLC.位置参数);
                }
            }
            return updateflag;
        }
        DateTime dtmem = DateTime.MinValue;

        internal abstract void WriteSync(int Addr, ushort[] val);
        internal virtual void WriteSync(int Addr, float val)
        {
            var b = BitConverter.GetBytes(val);
            ushort[] w = new ushort[2] { (ushort)(b[0] + b[1] * 256), (ushort)(b[2] + b[3] * 256) };
            WriteSync(Addr, w);
        }
        internal virtual void WriteSync(int Addr, float[] val)
        {
            if (val == null || val.Length == 0)
                return;
            ushort[] w = new ushort[2 * val.Length];
            for (int i = 0; i < val.Length; i++)
            {
                var b = BitConverter.GetBytes(val[i]);
                w[i * 2] = (ushort)(b[0] + b[1] * 256);
                w[i * 2 + 1] = (ushort)(b[2] + b[3] * 256);
            }
            WriteSync(Addr, w);
        }
        internal void WriteSync(int Addr, ushort val)
        {
            WriteSync(Addr, new ushort[] { val });
        }
        internal bool AssertWriteSync(int Addr, ushort val)
        {
            return AssertWriteSync(Addr, new ushort[] { val });
        }
        internal bool AssertWriteSync(int Addr, ushort[] val)
        {
            if (val != null && val.Length > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        WriteSync(Addr, val);
                        var read = ReadPlcInternalSync(Addr, (ushort)val.Length);
                        if (IsEqual(read, val))
                            return true;
                        else
                            Thread.Sleep(100);
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            return false;
        }

        internal class MyStringBoolConverter : StringConverter
        {
            protected Dictionary<string, object> dic;
            public MyStringBoolConverter()
            {
                dic = BoolDic;
            }
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(dic.Values);
                return vals;
            }
            static readonly Dictionary<string, object> BoolDic = new Dictionary<string, object>(2) { { "True", "True" }, { "False", "False" } };
        }
    }

    internal class DataStoreComparer : IComparer<KeyValuePair<int, ushort>>
    {
        public int Compare(KeyValuePair<int, ushort> x, KeyValuePair<int, ushort> y)
        {
            if (x.Key < 0 && y.Key < 0)
                return y.Key - x.Key;
            else
                return x.Key - y.Key;
        }
    }
}
internal class writeCmd
{
    internal ushort addr;
    internal object val;
    internal writeCmd(ushort _addr, float _val)
    {
        addr = _addr;
        val = _val;
    }
    internal writeCmd(ushort _addr, bool _val)
    {
        addr = _addr;
        val = _val;
    }
    internal writeCmd(ushort _addr, bool[] _val)
    {
        addr = _addr;
        val = _val;
    }
    internal writeCmd(ushort _addr, ushort _val)
    {
        addr = _addr;
        val = _val;
    }
    internal writeCmd(ushort _addr, ushort[] _val)
    {
        addr = _addr;
        val = _val;
    }
    internal writeCmd(ushort _addr, string _val)
    {
        addr = _addr;
        val = _val;
    }
}
