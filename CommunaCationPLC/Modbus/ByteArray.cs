using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunaCationPLC
{
    public class ByteArray
    {
        //创建一个byte集合
        List<byte> list = new List<byte>();

        //添加单个字节
        public void Add(byte item)
        {
            list.Add(item);
        }

        //添加一个字节数组
        public void Add(byte[] item)
        {
            list.AddRange(item);
        }

        //清空集合
        public void Clear()
        {
            // list.Clear();
            list = new List<byte>();
        }

        //获取数组
        public byte[] array
        {
            get { return list.ToArray(); }
        }

    }
}
