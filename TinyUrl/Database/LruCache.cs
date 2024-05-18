using System.Collections.Concurrent;

namespace TinyUrl.Database;

public class LruCache<TKey, TValue>
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _dictionary;
    private readonly LinkedList<KeyValuePair<TKey, TValue>> _linkedList;
    private readonly ConcurrentDictionary<TKey, SemaphoreSlim> _semaphores;

    public LruCache(int capacity)
    {
        _capacity = capacity;
        _dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        _linkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
        _semaphores = new ConcurrentDictionary<TKey, SemaphoreSlim>();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (_dictionary.TryGetValue(key, out var node))
        {
            _linkedList.Remove(node);
            _linkedList.AddLast(node);
            value = node.Value.Value;
            return true;
        }

        value = default;
        return false;
    }

    public void Set(TKey key, TValue value)
    {
        if (_dictionary.TryGetValue(key, out var node))
        {
            _linkedList.Remove(node);
        }
        else if (_dictionary.Count == _capacity)
        {
            var leastRecentlyUsed = _linkedList.First;
            _dictionary.Remove(leastRecentlyUsed.Value.Key);
            _linkedList.RemoveFirst();
        }

        var newNode = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
        _dictionary[key] = newNode;
        _linkedList.AddLast(newNode);
    }

    public SemaphoreSlim GetOrAddSemaphore(TKey key, SemaphoreSlim semaphore)
    {
        lock (_semaphores)
        {
            if (!_semaphores.TryGetValue(key, out var existingSemaphore))
            {
                _semaphores[key] = semaphore;
                return semaphore;
            }
            else
            {
                return existingSemaphore;
            }
        }
    }
}
