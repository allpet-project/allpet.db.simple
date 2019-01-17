using AllpetDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace allpet.db.simple.impl
{
    class SnapShot : ISnapShot
    {
        public SnapShot(IntPtr dbPtr)
        {
            this.dbPtr = dbPtr;
        }
        public void Init()
        {
            //this.readop = new RocksDbSharp.ReadOptions();
            this.readopHandle = RocksDbSharp.Native.Instance.rocksdb_readoptions_create();

            snapshotHandle = RocksDbSharp.Native.Instance.rocksdb_create_snapshot(this.dbPtr);
            RocksDbSharp.Native.Instance.rocksdb_readoptions_set_snapshot(readopHandle, snapshotHandle);
        }
        int refCount = 0;
        public IntPtr dbPtr;
        //public RocksDbSharp.RocksDb db;
        public IntPtr readopHandle;
        //public RocksDbSharp.ReadOptions readop;
        public IntPtr snapshotHandle = IntPtr.Zero;
        //public RocksDbSharp.Snapshot snapshot;

        public void Dispose()
        {
            lock (this)
            {
                refCount--;
                if (refCount == 0 && snapshotHandle != IntPtr.Zero)
                {
                    RocksDbSharp.Native.Instance.rocksdb_release_snapshot(this.dbPtr, snapshotHandle);
                    //snapshot.Dispose();
                    snapshotHandle = IntPtr.Zero;

                    RocksDbSharp.Native.Instance.rocksdb_readoptions_destroy(readopHandle);
                    readopHandle = IntPtr.Zero;
                }
            }
        }
        /// <summary>
        /// 对snapshot的引用计数加锁，保证处理是线程安全的
        /// </summary>
        public void AddRef()
        {
            lock (this)
            {
                refCount++;
            }
        }
        public byte[] GetValueData(byte[] tableid, byte[] key)
        {
            byte[] finialkey = Helper.CalcKey(tableid, key);
            return RocksDbSharp.Native.Instance.rocksdb_get(this.dbPtr, this.readopHandle, finialkey);
            //(readOptions ?? DefaultReadOptions).Handle, key, keyLength, cf);

            //return this.db.Get(finialkey, null, readop);
        }
        public IKeyFinder CreateKeyFinder(byte[] tableid, byte[] beginkey = null, byte[] endkey = null)
        {
            TableKeyFinder find = new TableKeyFinder(this, tableid, beginkey, endkey);
            return find;
        }
        public IKeyIterator CreateKeyIterator(byte[] tableid, byte[] _beginkey = null, byte[] _endkey = null)
        {
            var beginkey = Helper.CalcKey(tableid, _beginkey);
            var endkey = Helper.CalcKey(tableid, _endkey);
            return new TableIterator(this, tableid, beginkey, endkey);
        }
        public byte[] GetTableInfoData(byte[] tableid)
        {
            var tablekey = Helper.CalcKey(tableid, null, SplitWord.TableInfo);
            var data = RocksDbSharp.Native.Instance.rocksdb_get(this.dbPtr, this.readopHandle, tablekey);
            if (data == null)
                return null;
            return data;
        }
        public uint GetTableCount(byte[] tableid)
        {
            var tablekey = Helper.CalcKey(tableid, null, SplitWord.TableCount);
            var data = RocksDbSharp.Native.Instance.rocksdb_get(this.dbPtr, this.readopHandle, tablekey);
            return BitConverter.ToUInt32(data, 0);
        }
    }
}
