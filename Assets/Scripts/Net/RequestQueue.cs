using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class RequestQueue : MonoBehaviour
{
    private Queue<Func<Task>> _requestQueue = new Queue<Func<Task>>();
    private bool _isProcessing = false;

    public void EnqueueRequest(Func<Task> request)
    {
        _requestQueue.Enqueue(request);
        if (!_isProcessing)
        {
            _isProcessing = true;
            ProcessNextRequest();
        }
    }

    private async void ProcessNextRequest()
    {
        while (_requestQueue.Count > 0)
        {
            var currentRequest = _requestQueue.Dequeue();
            try
            {
                await currentRequest();
            }
            catch (Exception e)
            {
                Debug.LogError($"Request failed: {e.Message}");
            }
        }
        _isProcessing = false;
    }

    public void ClearQueue()
    {
        _requestQueue.Clear();
    }
}

