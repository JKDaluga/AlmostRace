using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public bool attachToManager;
        public int size;
    }

    public static ObjectPooler instance;
    

    private void Awake()
    {
        instance = this;
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
                GameObject currentObject = Instantiate(pool.prefab);
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
        if(poolDictionary == null)
        {
            Debug.LogWarning("Pool Dictionary doesn't exist. tag: " + tag);
            return null;
        }

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        if(poolDictionary[tag].Count == 0)
        {
            print(tag);
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

    public void Deactivate(string tag, GameObject objectToDeactivate)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
        }
        IPooledObject pooledObj = objectToDeactivate.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectDeactivate();
        }
        objectToDeactivate.SetActive(false);
        objectToDeactivate.transform.position = transform.position;
        objectToDeactivate.transform.rotation = transform.rotation;
        poolDictionary[tag].Enqueue(objectToDeactivate);
    }

    public IEnumerator DeactivateAfterTime(string tag, GameObject objectToDeactivate, float time)
    {
        if (poolDictionary == null)
        {
            ///Debug.LogWarning("Pool Dictionary doesn't exist");
            yield break;
        }

        yield return new WaitForSeconds(time);
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
        }

        IPooledObject pooledObj = objectToDeactivate.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectDeactivate();
        }

        objectToDeactivate.SetActive(false);
        objectToDeactivate.transform.position = transform.position;
        objectToDeactivate.transform.rotation = transform.rotation;
        poolDictionary[tag].Enqueue(objectToDeactivate);

    }


}
