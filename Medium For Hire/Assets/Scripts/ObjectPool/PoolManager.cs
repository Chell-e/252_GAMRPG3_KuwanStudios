using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> pooledObjects = new List<PooledObjectInfo>();

    private GameObject _objectPoolEmptyHolder;

    public static GameObject _enemyPoolEmpty;
    public static GameObject _expOrbPoolEmpty;
    private static GameObject _gameObjectsEmpty;

    public enum PoolType
    {
        Enemy,
        ExpOrb,
        None
    }
    public static PoolType poolingType;

    private void Awake()
    {
        SetupEmpties();
    }

    private void SetupEmpties()
    {
        _objectPoolEmptyHolder = new GameObject("Pooled Objects");

        _enemyPoolEmpty = new GameObject("Enemy Pooled Objects");
        _enemyPoolEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _expOrbPoolEmpty = new GameObject("Exp Orb Pooled Objects");
        _expOrbPoolEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        PooledObjectInfo pool = pooledObjects.Find(p => p.lookupString == objectToSpawn.name);

        // if pool doesnt exist, create it
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookupString = objectToSpawn.name };
            pooledObjects.Add(pool);
        }

        // check for any inactive objects in pool
        GameObject spawnableObj = pool.inactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            // find parent of object to spawn based on pool type
            GameObject parentObj = SetParentsObject(poolType);

            // if no inactive objects, create new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            if (parentObj != null)
            {
                spawnableObj.transform.SetParent(parentObj.transform);
            }    
        }
        else
        {
            // if there is an inactive object, reactivate it
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        // removes (Clone) from the passed in obj name to match the lookup string in pool
        string goName = obj.name.Substring(0, obj.name.Length - 7);

        PooledObjectInfo pool = pooledObjects.Find(p => p.lookupString == goName);

        if (pool == null)
        {
            Debug.LogError($"No pool found for object: {goName}.");
            return;
        }
        else
        {
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentsObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Enemy:
                return _enemyPoolEmpty;
            case PoolType.ExpOrb:
                return _expOrbPoolEmpty;
            case PoolType.None:
                return null;
            default:
                return null;
        }
    }
}

public class PooledObjectInfo
{
    public string lookupString;
    public List<GameObject> inactiveObjects = new List<GameObject>();
}
