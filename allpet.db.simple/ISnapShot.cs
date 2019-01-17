using System;
using System.Collections.Generic;
using System.Text;

namespace allpet.db.simple
{
    public interface ISnapShot : IDisposable
    {
        byte[] GetValueData(byte[] tableid, byte[] key);
        IKeyFinder CreateKeyFinder(byte[] tableid, byte[] beginkey = null, byte[] endkey = null);
        IKeyIterator CreateKeyIterator(byte[] tableid, byte[] _beginkey = null, byte[] _endkey = null);
        byte[] GetTableInfoData(byte[] tableid);
        uint GetTableCount(byte[] tableid);
    }

    public interface IKeyIterator : IEnumerator<byte[]>
    {

    }
    public interface IKeyFinder : IEnumerable<byte[]>
    {

    }

}
