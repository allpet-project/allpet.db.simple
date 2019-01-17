using allpet.db.simple;
using allpet.db.simple.impl;
using AllpetDB;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllPet.db.simple
{
    public class DB : IDisposable
    {
        IntPtr dbPtr;
        IntPtr defaultWriteOpPtr;
        public void Open(string path, bool createIfMissing = false)
        {
            if (dbPtr != IntPtr.Zero)
                throw new Exception("already open a db.");
            this.defaultWriteOpPtr = RocksDbSharp.Native.Instance.rocksdb_writeoptions_create();

            var HandleOption = RocksDbSharp.Native.Instance.rocksdb_options_create();
            if (createIfMissing)
            {
                RocksDbSharp.Native.Instance.rocksdb_options_set_create_if_missing(HandleOption, true);
            }
            RocksDbSharp.Native.Instance.rocksdb_options_set_compression(HandleOption, RocksDbSharp.CompressionTypeEnum.rocksdb_snappy_compression);
            //RocksDbSharp.DbOptions option = new RocksDbSharp.DbOptions();
            //option.SetCreateIfMissing(true);
            //option.SetCompression(RocksDbSharp.CompressionTypeEnum.rocksdb_snappy_compression);
            IntPtr handleDB = RocksDbSharp.Native.Instance.rocksdb_open(HandleOption, path);
            this.dbPtr = handleDB;

            snapshotLast = CreateSnapInfo();
            snapshotLast.AddRef();
        }
        public void Dispose()
        {
            snapshotLast.Dispose();
            snapshotLast = null;

            RocksDbSharp.Native.Instance.rocksdb_writeoptions_destroy(this.defaultWriteOpPtr);
            this.defaultWriteOpPtr = IntPtr.Zero;
            RocksDbSharp.Native.Instance.rocksdb_close(this.dbPtr);
            this.dbPtr = IntPtr.Zero;
        }
        //创建快照
        private SnapShot CreateSnapInfo()
        {
            //看最新高度的快照是否已经产生
            var snapshot = new SnapShot(this.dbPtr);
            snapshot.Init();
            return snapshot;
        }
        private SnapShot snapshotLast;

        //如果 height=0，取最新的快照
        public ISnapShot UseSnapShot()
        {
            var snap = snapshotLast;

            snap.AddRef();
            return snap;
        }
        public IWriteBatch CreateWriteBatch()
        {
            return new WriteBatch(this.dbPtr, UseSnapShot() as SnapShot);
        }
        public void WriteBatch(IWriteBatch wb)
        {
            RocksDbSharp.Native.Instance.rocksdb_write(this.dbPtr, this.defaultWriteOpPtr, (wb as WriteBatch).batchptr);
            snapshotLast.Dispose();
            snapshotLast = CreateSnapInfo();
            snapshotLast.AddRef();
        }
        private byte[] GetDirectFinal(byte[] finalkey)
        {
            var data = RocksDbSharp.Native.Instance.rocksdb_get(dbPtr, snapshotLast.readopHandle, finalkey);
            if (data == null || data.Length == 0)
                return null;
            return data;
        }
        public byte[] GetDirect(byte[] tableid, byte[] key)
        {
            var finalkey = Helper.CalcKey(tableid, key);
            return GetDirectFinal(finalkey);
        }
        public UInt64 GetUInt64Direct(byte[] tableid, byte[] key)
        {
            var data = GetDirect(tableid, key);
            if (data == null || data.Length == 0)
                return 0;
            else return BitConverter.ToUInt64(data,0);
        }
        public void PutUInt64Direct(byte[] tableid, byte[] key,UInt64 v)
        {
            this.PutDirect(tableid, key, BitConverter.GetBytes(v));
        }
        private void DeleteDirectFinal(byte[] finalkey)
        {
            RocksDbSharp.Native.Instance.rocksdb_delete(this.dbPtr, this.defaultWriteOpPtr, finalkey, finalkey.LongLength);

        }
        private void PutDirectFinal(byte[] finalkey, byte[] data)
        {
            RocksDbSharp.Native.Instance.rocksdb_put(this.dbPtr, this.defaultWriteOpPtr, finalkey, (UIntPtr)finalkey.Length, data, (UIntPtr)data.Length, out IntPtr err);
            if (err != IntPtr.Zero)
            {
                return;
            }
        }
        public void PutDirect(byte[] tableid, byte[] key, byte[] data)
        {
            var finalkey = Helper.CalcKey(tableid, key);
            var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);



            var countdata = GetDirectFinal(countkey);
            UInt32 count = 0;
            if (countdata != null)
            {
                count = BitConverter.ToUInt32(countdata, 0);
            }
            var vdata = GetDirectFinal(finalkey);
            if (vdata == null || vdata.Length == 0)
            {
                count++;
            }
            else
            {
                if (Helper.BytesEquals(vdata, data) == false)
                    count++;
            }
            PutDirectFinal(finalkey, data);

            var countvalue = BitConverter.GetBytes(count);
            PutDirectFinal(countkey, countvalue);

        }
        public void DeleteDirect(byte[] tableid, byte[] key)
        {
            var finalkey = Helper.CalcKey(tableid, key);

            var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);
            var countdata = GetDirectFinal(countkey);
            UInt32 count = 0;
            if (countdata != null)
            {
                count = BitConverter.ToUInt32(countdata, 0);
            }

            var vdata = GetDirectFinal(finalkey);
            if (vdata != null && vdata.Length != 0)
            {
                DeleteDirectFinal(finalkey);
                count--;
                var countvalue = BitConverter.GetBytes(count);
                PutDirectFinal(countkey, countvalue);

            }
        }
        public void CreateTableDirect(byte[] tableid, byte[] info)
        {
            var finalkey = Helper.CalcKey(tableid, null, SplitWord.TableInfo);
            var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);
            var data = GetDirectFinal(finalkey);
            if (data != null && data.Length != 0)
            {
                throw new Exception("alread have that.");
            }
            PutDirectFinal(finalkey, info);

            var byteCount = GetDirectFinal(countkey);
            if (byteCount == null || byteCount.Length == 0)
            {
                byteCount = BitConverter.GetBytes((UInt32)0);
            }
            PutDirectFinal(countkey, byteCount);

        }
        public void DeleteTableDirect(byte[] tableid)
        {
            var finalkey = Helper.CalcKey(tableid, null, SplitWord.TableInfo);
            //var countkey = Helper.CalcKey(tableid, null, SplitWord.TableCount);
            var vdata = GetDirectFinal(finalkey);
            if (vdata != null && vdata.Length != 0)
            {
                DeleteDirectFinal(finalkey);
            }
        }
    }
}