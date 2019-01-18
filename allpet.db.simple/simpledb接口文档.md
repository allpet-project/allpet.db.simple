## simpledb接口文档##

#### 接口说明 **1、**打开数据库

- **接口**
> [void Open(string path, bool createIfMissing)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| path|  <mark>string,**不可为空**</mark>|  数据库的路径|
| createIfMissing|   bool,默认为false|  如果不存在是否创建该数据库|

- **返回参数**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| 无|

----------

#### 接口说明 **2、**使用数据库的快照

- **接口**
> [ISnapShot UseSnapShot()](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
|无|

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| snapShot|  ISnapShot|  数据库的快照|

----------

#### 接口说明 **3、**创建一个批量写入对象

- **接口**
> [IWriteBatch CreateWriteBatch()](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
|无|

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| writeBatch|  IWriteBatch|  批量写入对象|

----------

#### 接口说明 **4、**从DB中查询key对应的value

- **接口**
> [byte[] GetDirect(byte[] tableid, byte[] key)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| tableid|  <mark>byte[],**不能大于255字节**</mark>| tableid |
| key|  <mark>byte[],</mark>| 查询的key |

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| value|  byte[]| key对应的value |

----------

#### 接口说明 **5、**根据指定的key从DB中查询出UInt64类型的value

- **接口**
> [UInt64 GetUInt64Direct(byte[] tableid, byte[] key)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| tableid|  <mark>byte[],**不能大于255字节**</mark>| tableid |
| key|  <mark>byte[],</mark>| 查询的key |

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| value|  UInt64| key对应的UInt64类型的value |

----------

#### 接口说明 **6、**向DB中存储一个key/value

- **接口**
> [void PutDirect(byte[] tableid, byte[] key, byte[] data)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| tableid|  <mark>byte[],**不能大于255字节**</mark>| tableid |
| key|  <mark>byte[],</mark>| 存储的key |
| data|  <mark>byte[],</mark>| 存储的value |

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
|无|

----------

#### 接口说明 **7、**向DB中存储一个UInt64类型的key/value

- **接口**
> [void PutUInt64Direct(byte[] tableid, byte[] key,UInt64 v)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| tableid|  <mark>byte[],**不能大于255字节**</mark>| tableid |
| key|  <mark>byte[],</mark>| 存储的key |
| data|  <mark>UInt64,</mark>| UInt64类型的value |

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
|无|

----------

#### 接口说明 **8、**从DB中删除一个key/value

- **接口**
> [void DeleteDirect(byte[] tableid, byte[] key)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| tableid|  <mark>byte[],**不能大于255字节**</mark>| tableid |
| key|  <mark>byte[],</mark>| 需要删除的key |

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
|无|

----------

#### 接口说明 **9、**创建一个Table

- **接口**
> [void CreateTableDirect(byte[] tableid, byte[] info)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| tableid|  <mark>byte[],**不能大于255字节**</mark>| tableid |
| info|  <mark>byte[],</mark>| 需要删除的key |

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
|无|

----------

#### 接口说明 **10、**删除一个Table

- **接口**
> [void DeleteTableDirect(byte[] tableid)](#)

- **参数**
>
 | 请求参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
| tableid|  <mark>byte[],**不能大于255字节**</mark>| tableid |

- **返回值**
> | 返回参数      |     参数类型 |   参数说明   |
| :-------- | :--------| :------ |
|无|