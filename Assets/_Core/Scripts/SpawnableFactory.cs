using System;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnableFactory
{
    private static Dictionary<Type, GameObject> _spawnablesMap;

    static SpawnableFactory()
    {
        BuildPrefabMap();
    }

    private static void BuildPrefabMap()
    {
        _spawnablesMap = new Dictionary<Type, GameObject>();

        foreach (var prefab in Resources.LoadAll<GameObject>("Spawnables"))
        {
            var component = prefab.GetComponent<ISpawnable>();
            if (component != null)
            {
                var type = component.GetType();
                Debug.Log($"Added prefab {prefab.name} as {type}");
                _spawnablesMap.TryAdd(type, prefab);
            }
        }
    }

    public static T Create<T>(Transform container, Vector3 position, Quaternion rotation) where T : Component
    {
        if (_spawnablesMap.TryGetValue(typeof(T), out var prefab))
        {
            GameObject obj = UnityEngine.Object.Instantiate(prefab, position, rotation, container);
            return obj.GetComponent<T>();
        }

        Debug.LogError($"Prefab di tipo {typeof(T)} non trovato!");
        return null;
    }
}