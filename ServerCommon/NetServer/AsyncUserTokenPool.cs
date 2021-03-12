using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.NetServer
{
    /// <summary>
    /// 表示AsyncUserToken的栈结构
    /// </summary>
    class AsyncUserTokenPool
    {
        Stack<AsyncUserToken> m_pool;

        public AsyncUserTokenPool(int capacity)
        {
            m_pool = new Stack<AsyncUserToken>(capacity);
        }

        public void Push(AsyncUserToken userToken)
        {
            if (null == userToken)
            {
                throw new ArgumentException("userToken added to a AsyncUserToken cannot be null");
            }

            lock (m_pool)
            {
                m_pool.Push(userToken);
            }
        }
        public AsyncUserToken Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        public void Clear()
        {
            lock (m_pool)
            {
                m_pool.Clear();
            }
        }

        public int Count
        {
            get { return m_pool.Count; }
        }
    }
}
