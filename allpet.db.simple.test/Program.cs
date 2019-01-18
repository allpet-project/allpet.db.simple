
using AllPet.db.simple;
using System;

namespace allpet.db.simple.test
{
    class Program
    {
        static void Main(string[] args)
        {
            DB db = new DB();
            db.Open("db001", true);

            var tableid = new byte[] { 0x01, 0x02, 0x03 };
            var ss = db.UseSnapShot();
            var tableinfo = ss.GetTableInfoData(tableid);
            if (tableinfo == null || tableinfo.Length == 0)
            {
                var wb = db.CreateWriteBatch();
                wb.CreateTable(tableid, new byte[] { 0x01, 0x02 });
                db.WriteBatch(wb);
            }
            var t0 = DateTime.Now;
            Random r = new Random();
            for (var i = 0; i < 10000; i++)
            {
                var wb = db.CreateWriteBatch();
                var key = BitConverter.GetBytes(i);
                for (var j = 0; j < 1; j++)
                {
                    byte[] tdata = new byte[8000];
                    r.NextBytes(tdata);
                    //db.PutDirect(tableid, key, tdata);
                    wb.Put(tableid, key, tdata);
                }

                db.WriteBatch(wb);

            }
            var t1 = DateTime.Now;
            Console.WriteLine("time=" + (t1 - t0).TotalSeconds);
            Console.ReadLine();
        }
    }
}
