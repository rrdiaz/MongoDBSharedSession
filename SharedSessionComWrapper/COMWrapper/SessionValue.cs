using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IOL.SharedSessionServer.COMWrapper
{
    [ComVisible(true)]
    [Guid("B261672B-A6D0-4FDB-BBDB-210253BB6D82")]
    [ProgId("IOL.SharedSessionServer.COMWrapper.MongoSessionValue")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class SessionValue
    {
        private string key;
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        private string value;
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
       
    }
}
