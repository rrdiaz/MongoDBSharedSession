using IOL.SharedSessionServer.COMWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
           SharedSession session = new SharedSession();
           // var lineaID=Console.ReadLine();
           //var req= session.Start(lineaID);
           //req.Set("nuevo1", "diego");
           
            var ss = session.Start("cookie_12345");
            ss["testKey"] = "testValue";
            Console.WriteLine(ss["testKey"]);

            //int idx=0;
            //foreach (var item in "A,B,C,D,E,F,G,H".Split(','))
            //{
            //    idx++;
            //    var key=(item + "_" + idx.ToString());
            //    var value= "Valor " + item;
            //    session[key] = value;
            //    //ss.Set(key,value);
              
            //}



            //idx = 0;
            //foreach (var item in "A,B,C,D,E,F,G,H".Split(','))
            //{
            //    idx++;
            //    var key = (item + "_" + idx.ToString());
            //    //var value = ss.Get(key);
            //    var value = session[key];

            //    Console.WriteLine("{0} = {1}", key,value.ToString());
            //}


            Console.ReadLine();
        }
    }
}
