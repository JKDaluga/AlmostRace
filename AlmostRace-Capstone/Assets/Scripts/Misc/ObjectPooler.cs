using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject preFab;
        public bool attachToManager;
        public int size;
    }

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject currentObject = Instantiate(pool.preFab);
                if (pool.attachToManager)
                {
                    currentObject.transform.SetParent(gameObject.transform);
                }
                currentObject.SetActive(false);
                objectPool.Enqueue(currentObject);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        return Activate(tag, position, rotation, false, null);
    }

    public GameObject SpawnFromPoolAndParent(string tag, Vector3 position, Quaternion rotation, Transform givenParent)
    {
        return Activate(tag, position, rotation, true, givenParent);
    }

    private GameObject Activate(string tag, Vector3 position, Quaternion rotation, bool setParent, Transform givenParent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        GameObject objectToActivate = poolDictionary[tag].Dequeue();
        if (setParent)
        {
            objectToActivate.transform.SetParent(givenParent);
        }
        objectToActivate.SetActive(true);
        objectToActivate.transform.position = position;
        objectToActivate.transform.rotation = rotation;

        IPooledObject pooledObj = objectToActivate.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectActivate();
        }

        return objectToActivate;
    }

    public void Deactivate(string tag, GameObject objectToActivate)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
        }
        objectToActivate.SetActive(false);
        objectToActivate.transform.position = transform.position;
        objectToActivate.transform.rotation = transform.rotation;
        poolDictionary[tag].Enqueue(objectToActivate);
    }


}
