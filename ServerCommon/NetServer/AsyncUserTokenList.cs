using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.NetServer
{
    /// <summary>
    /// 表示AsyncUserToken的列表集合
    /// </summary>
    public class AsyncUserTokenList
    {
        List<AsyncUserToken> m_list;


        //public AsyncUserToken this [int index]
        //{
        //    get
        //    {
        //        return m_list[index];
        //    }
        //}

        public AsyncUserTokenList()
        {
            m_list = new List<AsyncUserToken>();
        }

        public void Add(AsyncUserToken userToken)
        {
            if (null == userToken)
            {
                throw new ArgumentException("userToken added to a AsyncUserToken cannot be null");
            }
            lock (m_list)
            {
                m_list.Add(userToken);
            }
        }
        public void Remove(AsyncUserToken userToken)
        {
            lock (m_list)
            {
                m_list.Remove(userToken);
            }
        }

        public void CopyList(ref AsyncUserToken[] array)
        {
            lock (m_list)
            {
                array = new AsyncUserToken[m_list.Count];
                m_list.CopyTo(array);
            }
        }

        public void Clear()
        {
            lock (m_list)
            {
                m_list.Clear();
            }
        }

        public int Count { get { return m_list.Count; } }
    }
}
