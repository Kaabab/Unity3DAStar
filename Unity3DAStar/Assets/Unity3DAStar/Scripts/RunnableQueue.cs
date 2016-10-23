using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
 
/// 
/// Allows for safely switching between a Thread pool and Unity thread
/// 
/// Typical Usage include excuting some heavy operation on a thread pool and returning a callback to Unity thread as shown below :
/// 
//		runnableQueue.ThreadPooledQueue.Enqueue(() =>{
//			// Do some heavy operation ie I/O, parsing, computations, etc
//			// ...								
//			// When done make callback to the Unity Loop
//			runnableQueue.UpdateQueue.Enqueue(() =>
//			{
//				// Make a safe callback to the unity loop eg (Debug.Log("Safe print");)
//				});
//		});
public class RunnableQueue : MonoBehaviour
{
    public int MaxThreads
    {
        get { return _maxThreads; }
        private set
        {
            _maxThreads = value;
        }
    }
    public IActionQueue UpdateQueue { get; private set; }// executes at next update
    public IActionQueue ThreadPooledQueue { get; private set; }// is queued in the worker pool and will execute as soon as a thread is available

    private static ThreadSafeQueue _debugQueue = null;
    private int _maxThreads;

    void Awake()
    {
        _debugQueue = _debugQueue ?? new ThreadSafeQueue();
        MaxThreads = SystemInfo.processorCount > 1 ? SystemInfo.processorCount * 2 : 1;
        UpdateQueue = UpdateQueue ?? new ThreadSafeQueue();
        ThreadPooledQueue = ThreadPooledQueue ?? new ThreadPoolQueue();
    }

    public interface IActionQueue
    {
        void Enqueue(Action action);
        Action Dequeue();
    }

    static void RunAsync(Action action)
    {
        ThreadPool.QueueUserWorkItem((object state) =>
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                _debugQueue.Enqueue(delegate ()
                {
                    Debug.LogError("Thread Pool Exception");
                    Debug.LogException(e);
                });
            }
        });
    }

    void Update()
    {
        Action action;
        while ((action = _debugQueue.Dequeue()) != null)
        {
            action();
        }
        while ((action = UpdateQueue.Dequeue()) != null)
        {
            action();
        }
    }

    private class ThreadSafeQueue : IActionQueue
    {
        private System.Object _lock;
        private Queue<Action> _queuedActions;

        public ThreadSafeQueue()
        {
            _queuedActions = new Queue<Action>();
            _lock = new System.Object();
        }

        public Action Dequeue()
        {
            lock (_lock)
            {
                if (_queuedActions.Count > 0)
                {
                    return _queuedActions.Dequeue();
                }
                return null;
            }
        }

        public void Enqueue(Action action)
        {
            lock (_lock)
            {
                _queuedActions.Enqueue(action);
            }
        }
    }

    private class ThreadPoolQueue : IActionQueue
    {
        public void Enqueue(Action action)
        {
            RunAsync(action);
        }

        public Action Dequeue()
        {
            return null;
        }
    }
}

