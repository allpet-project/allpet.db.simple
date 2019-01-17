using System;
using System.Collections.Generic;
using System.Text;

namespace allpet.db.simple
{
    public interface IWriteBatch
    {
        ISnapShot snapshot
        {
            get;
        }
        byte[] GetData(byte[] finalkey);
        void CreateTable(byte[] tableid, byte[] finaldata);
        void DeleteTable(byte[] tableid);
        void Put(byte[] tableid, byte[] key, byte[] finaldata);
        void Delete(byte[] tableid, byte[] key);
    }
}
