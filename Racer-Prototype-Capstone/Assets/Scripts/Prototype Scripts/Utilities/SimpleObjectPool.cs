using UnityEngine;
using System.Collections.Generic;

class SimpleObjectPool : MonoBehaviour
{
	///Summary
	///The base size of the pool.
	///Summary
	const int DEFAULT_POOL_SIZE = 5;

	///Summary
	///Singleton value
	///Summary
	private static SimpleObjectPool instance;

	///Summary
	///Singleton function, only instantiate the object when it needs to
	///Summary
	public SimpleObjectPool Instance()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
		}

		return instance;
	}

	void Awake()
	{
		if(instance != null)
		{
			Destroy(this);
		}

		Instance();
	}

	///Summary
	///The class that holds the individual prefabs and inactive items.
	///Summary
	class Pool
	{
		///Summary
		///A cosmetic ID number for the prefabs.
		///Summary
		int nextID = 1;

		Stack<GameObject> inactiveObjects;

		GameObject prefab;

		///Summary
		///Constructor for the Pool class.
		///Summary
		public Pool(GameObject prefab, int initialQuantity)
		{
			this.prefab = prefab;

			inactiveObjects = new Stack<GameObject>(initialQuantity);
		}

		///Summary
		///Checks for inactive objects, then spawns a new one if there aren't any.
		///Summary
		public GameObject Spawn(Vector3 pos, Quaternion rot)
		{
			GameObject spawnedObject;

			if(inactiveObjects.Count == 0)
			{
				spawnedObject = (GameObject)GameObject.Instantiate(prefab, pos, rot);
				spawnedObject.name = prefab.name + "("+(nextID++)+")";

				spawnedObject.AddComponent<PoolMember>().pool = this;
			}
			else
			{
				spawnedObject = inactiveObjects.Pop();

				if(spawnedObject == null)
				{
					return Spawn(pos, rot);
				}
			}

			spawnedObject.transform.position = pos;
			spawnedObject.transform.rotation = rot;
			spawnedObject.SetActive(true);
			return spawnedObject;
		}

		///Summary
		///Deactivates the object and adds it back into the stack.
		///Summary
		public void Despawn(GameObject objToDespawn)
		{
			objToDespawn.SetActive(false);

			inactiveObjects.Push(objToDespawn);
		}
	}

	///Summary
	///Class that is added to spawned objects. Holds a reference to the Pool they were spawned from.
	///Summary
	class PoolMember : MonoBehaviour
	{
		public Pool pool;
	}

	///Summary
	///A dictionary to hold all of the different pools.
	///Summary
	static Dictionary< GameObject, Pool > pools;

	///Summary
	///Function that initializes the Dictionary with new Pools if one does not already exist.
	///Summary
	static void Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
	{
		if(pools == null)
		{
			pools = new Dictionary<GameObject, Pool>();
		}
		
		if(prefab != null && pools.ContainsKey(prefab) == false)
		{
			pools[prefab] = new Pool(prefab, qty);
		}
	}

	///Summary
	///Function that allows for preloading of objects. This can be helful with objects that you know you will need lots of in quick succession. Not necessary for most items/objects.
	///Summary
	static public void Preload(GameObject prefab, int qty = 1)
	{
		Init(prefab, qty);

		GameObject[] objects = new GameObject[qty];
		for(int i = 0; i < qty; i++)
		{
			objects[i] = Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		for(int i = 0; i < qty; i++)
		{
			Despawn(objects[i]);
		}
	}

	///Summary
	///Function that initializes a new Pool (if not already there) and then spawns a prefab from it.
	///Summary
	static public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		Init(prefab);

		return pools[prefab].Spawn(pos, rot);
	}

	///Summary
	///Checks that the object is a poolmember and then Despawns it, if it is not a poolmember it destroys it.
	///Summary
	static public void Despawn(GameObject objToDespawn)
	{
		PoolMember poolMember = objToDespawn.GetComponent<PoolMember>();

		if(poolMember == null)
		{
			Debug.Log("Object " +objToDespawn.name+ " was not spawned from a pool. Destroying it.");
			GameObject.Destroy(objToDespawn);
		}
		else
		{
			poolMember.pool.Despawn(objToDespawn);
		}
	}
}