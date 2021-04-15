﻿using System;
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
        private Semaphore semaphore;

        public SemaphoreQueue():this(1000)
        {
        }

        public SemaphoreQueue(int maxCount)
        {
            m_queue = new Queue<T>();
            semaphore =new Semaphore(0, maxCount);
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
                semaphore.Release();
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
                if (semaphore.WaitOne(maxWaitTime))
                {
                    msg = m_queue.Dequeue();
                    return true;
                }
                msg = default(T);
                return false;
            }
        }

        /// <summary>
        /// 无限等待 取出一个消息 
        /// </summary>
        public T Dequeue()
        {
            lock (m_queue)
            {
                semaphore.WaitOne(Timeout.Infinite);
                return m_queue.Dequeue();
            }
        }

    }
}
