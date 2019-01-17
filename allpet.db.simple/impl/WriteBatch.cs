using AllpetDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace allpet.db.simple.impl
{
    class WriteBatch : IWriteBatch, IDisposable
    {
        public WriteBatch(IntPtr dbptr, SnapShot snapshot)
        {
            this.dbPtr = dbptr;
            this.batchptr = RocksDbSharp.Native.Instance.rocksdb_writebatch_create();
            //this.batch = new RocksDbSharp.WriteBatch();
            this._snapshot = snapshot;
            this.cache = new Dictionary<string, byte[]>();
        }
        //RocksDbSharp.RocksDb db;
        public IntPtr dbPtr;
        public SnapShot _snapshot;
        public ISnapShot snapshot
        {
            get
            {
                return _snapshot;
            }
        }
        //public RocksDbSharp.WriteBatch batch;
        public IntPtr batchptr;
        Dictionary<string, byte[]> cache;

        public void Dispose()
        {
            if (batchptr != IntPtr.Zero)
            {
                RocksDbSharp.Native.Instance.rocksdb_writebatch_destroy(batchptr);
                batchptr = IntPtr.Zero;
                //batch.Dispose();
                //batch = null;
            }
            _snapshot.Dispose();
        }
        public byte[] GetData(byte[] finalkey)
        {
            var hexkey = Helper.ToString_Hex(finalkey);
            if (cache.ContainsKey(hexkey))
            {
                return cache[hexkey];
            }
            else
            {
                var data = RocksDbSharp.Native.Instance.rocksdb_get(dbPtr, _snapshot.readopHandle, finalkey);
                if (data == null || data.Length == 0)
                    return null;
                //db.Get(finalkey, null, snapshot.readop);
                cache[hexkey] = data;
                return data;
            }
        }
        private void PutDataFinal(byte[] finalkey, byte[] value)
        {
            var hexkey = Helper.ToString_Hex(finalkey);
            cache[hexkey] = value;
            RocksDbSharp.Native.Instance.rocksdb_writebatch_put(batchptr, finalkey, (ulong)finalkey.Length, value, (ulong)value.Length);
            //batch.Put(finalkey, value);
        }
        private void DeleteFinal(byte[] finalkey)
        {
            var hexkey = Helper.ToString_Hex(finalkey);
            cache.Remove(hexkey);
            RocksDbSharp.Native.Instance.rocksdb_writebatch_delete(batchptr, finalkey, (ulong)finalkey.Length);
            //batch.Delete(finalkey);
        }
        public void CreateTable(byte[] tableid, byte[] tableinfo)
        {
            var finalkey = Helper.CalcKey(tableid, null, SplitWord.TableInfo);
            var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);
            var data = GetData(finalkey);
            if (data != null && data.Length != 0)
            {
                throw new Exception("alread have that.");
            }
            PutDataFinal(finalkey, tableinfo);

            var byteCount = GetData(countkey);
            if (byteCount == null || byteCount.Length == 0)
            {
                byteCount = BitConverter.GetBytes((UInt32)0);
            }
            PutDataFinal(countkey, byteCount);
        }

        public void DeleteTable(byte[] tableid)
        {
            var finalkey = Helper.CalcKey(tableid, null, SplitWord.TableInfo);
            //var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);
            var vdata = GetData(finalkey);
            if (vdata != null && vdata.Length != 0)
            {
                DeleteFinal(finalkey);
            }
        }
        public void Put(byte[] tableid, byte[] key, byte[] finaldata)
        {
            var finalkey = Helper.CalcKey(tableid, key);
            var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);



            var countdata = GetData(countkey);
            UInt32 count = 0;
            if (countdata != null)
            {
                count = BitConverter.ToUInt32(countdata, 0);
            }
            var vdata = GetData(finalkey);
            if (vdata == null || vdata.Length == 0)
            {
                count++;
            }
            else
            {
                if (Helper.BytesEquals(vdata, finaldata) == false)
                    count++;
            }
            PutDataFinal(finalkey, finaldata);

            var countvalue = BitConverter.GetBytes(count);
            PutDataFinal(countkey, countvalue);
        }

        public void Delete(byte[] tableid, byte[] key)
        {
            var finalkey = Helper.CalcKey(tableid, key);

            var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);
            var countdata = GetData(countkey);
            UInt32 count = 0;
            if (countdata != null)
            {
                count = BitConverter.ToUInt32(countdata, 0);
            }

            var vdata = GetData(finalkey);
            if (vdata != null && vdata.Length != 0)
            {
                DeleteFinal(finalkey);
                count--;
                var countvalue = BitConverter.GetBytes(count);
                PutDataFinal(countkey, countvalue);

            }
        }
    }
}
