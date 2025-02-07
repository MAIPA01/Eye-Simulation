using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
                data.OnFinishCallback?.Invoke(data.Value);
            }
        }
    }

    public void RunNewTask(Func<T> func, Action<T> onFinishCallback = null)
    {
        Task t = Task.Run(() =>
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
