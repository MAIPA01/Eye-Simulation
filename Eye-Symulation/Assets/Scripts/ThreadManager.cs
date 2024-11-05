using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ThreadManager<T>
{
    private struct FinishedThreadData
    {
        public T Value;
        public Action<T> OnFinishCallback;
    };

    private readonly Queue<FinishedThreadData> FinishedThreads = new();

    public void Update()
    {
        lock (FinishedThreads)
        {
            while (FinishedThreads.Count > 0)
            {
                var data = FinishedThreads.Dequeue();
                if (data.OnFinishCallback != null)
                {
                    data.OnFinishCallback.Invoke(data.Value);
                }
            }
        }
    }

    public void RunNewTask(Func<T> func, Action<T> onFinishCallback = null)
    {
        Task.Run(() =>
        {
            FinishedThreadData data = new()
            {
                Value = func(),
                OnFinishCallback = onFinishCallback
            };

            lock (FinishedThreads)
            {
                FinishedThreads.Enqueue(data);
            }
        });
    }
}
