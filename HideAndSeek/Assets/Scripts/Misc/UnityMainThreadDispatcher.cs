using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class UnityMainThreadDispatcher : SingletonGameObject<UnityMainThreadDispatcher> {

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public new void Awake() {
        base.Awake();

        Debugger.OnLog += (message) => { Debug.Log(message); };
        Debugger.OnErrorLog += (message) => { Debug.LogError(message); };
    }

    public void Update() {
        lock (_executionQueue) {
            while (_executionQueue.Count > 0) {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    // Locks the queue and adds the IEnumerator to the queue
    // IEnumerator function that will be executed from the main thread
    public void Enqueue(IEnumerator action) {
        lock (_executionQueue) {
            _executionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }

    // Locks the queue and adds the Action to the queue
    // action is function that will be executed from the main thread
    public void Enqueue(Action action) {
        Enqueue(ActionWrapper(action));
    }

    // Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
    // action is function that will be executed from the main thread
    //A Task that can be awaited until the action completes
    public Task EnqueueAsync(Action action) {
        var tcs = new TaskCompletionSource<bool>();

        void WrappedAction() {
            try {
                action();
                tcs.TrySetResult(true);
            } catch (Exception ex) {
                tcs.TrySetException(ex);
            }
        }

        Enqueue(ActionWrapper(WrappedAction));
        return tcs.Task;
    }


    IEnumerator ActionWrapper(Action action) {
        action();
        yield return null;
    }
}