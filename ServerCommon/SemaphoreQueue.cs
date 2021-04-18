using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCommon
{
    public class SemaphoreQueue<T> 
    {
        private Queue<T> m_queue = new Queue<T>();
        public Semaphore Semaphore { get; }

        public SemaphoreQueue():this(1000)
        {
        }

        public SemaphoreQueue(int maxCount)
        {
            m_queue = new Queue<T>();
            Semaphore = new Semaphore(0, maxCount);
        }

        /// <summary>
        /// 消息个数
        /// </summary>
        public int Count => m_queue.Count;

        public void Enqueue(T msg)
        {
            lock (m_queue)
            {
                m_queue.Enqueue(msg);
                Semaphore.Release();
            }
        }

        /// <summary>
        /// 取出一个消息 如果等待超时则返回false
        /// </summary>
        /// <param name="maxWaitTime">最大等待时间 毫秒</param>
        /// <param name="msg">返回的消息</param>
        public bool Dequeue(int maxWaitTime,out T msg)
        {
            lock (m_queue)
            {
                if (Semaphore.WaitOne(maxWaitTime))
                {
                    msg = m_queue.Dequeue();
                    return true;
                }
                msg = default(T);
                return false;
            }
        }

        /// <summary>
        /// 取出对象 并且不等待
        /// </summary>
        public T DequeueNoWait()
        {
            lock (m_queue)
            {
                return m_queue.Dequeue();
            }
        }
        /// <summary>
        /// 无限等待 取出一个消息 
        /// </summary>
        public T Dequeue()
        {
            lock (m_queue)
            {
                Semaphore.WaitOne(Timeout.Infinite);
                return m_queue.Dequeue();
            }
        }

    }
}
