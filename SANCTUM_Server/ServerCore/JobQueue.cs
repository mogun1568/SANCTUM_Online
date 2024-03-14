using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }

    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false;

        public void Push(Action job)
        {
            bool flush = false;

            lock (_lock)
            {
                _jobQueue.Enqueue(job);
                if (_flush == false)
                {
                    flush = _flush = true;
                }
            }

            // lock으로 인해 딱 하나의 쓰레드만 실행할 수 있음 (한 명만 순차적으로 실행하는 개념)
            if (flush)
            {
                Flush();
            }
        }

        void Flush()
        {
            while (true)
            {
                Action action = Pop();
                if (action == null)
                {
                    return;
                }

                action.Invoke();
            }
        }

        Action Pop()
        {
            // 꺼내는 중에도 Push를 할 수 있기 때문에 lock을 걸어줌
            lock ( _lock )
            {
                if (_jobQueue.Count == 0)
                {
                    _flush = false;
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }
}
