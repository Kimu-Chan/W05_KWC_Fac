using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Resource
{
    private ResourceType type;
    public int value;

    public ResourceType Type
    {
        get
        {
            return type;
        }
    }

    public Resource(ResourceType type)
    {
        this.type = type;
        value = 0;
    }
}

public class ResourceManager
{
    public object latch = new object();

    public int inventoryCapacity;
    public int inventoryUsage;
    public HashSet<ResourceType> inventoryResources = new HashSet<ResourceType>();

    private Dictionary<ResourceType, Resource> resources = new Dictionary<ResourceType, Resource>();

    private void InitResources()
    {
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resources.Add(resourceType, new Resource(resourceType));
        }

        // 초기값
        inventoryCapacity = 20;
        resources[ResourceType.R_Metal].value = 100;
        // resources[ResourceType.R_Organic].value = 100;
        resources[ResourceType.R_Energy].value = 100;
    }

    public ResourceManager()
    {
        InitResources();
    }

    // private Resource GetResource(ResourceType resourceType)
    // {
    //     return resources[resourceType];
    // }

    public int GetResourceValue(ResourceType resourceType)
    {
        return resources[resourceType].value;
    }

    public int AddResourceValue(ResourceType resourceType, int value)
    {
        return Interlocked.Add(ref resources[resourceType].value, value);
    }

    public bool TrySubManyResourceValue(List<ResourceType> types, List<int> values)
    {
        lock (latch)
        {
            bool buildAble = true;
            for (int i = 0; i < types.Count; ++i)
            {
                buildAble &= resources[types[i]].value >= values[i];
            }
            if (!buildAble)
            {
                return false;
            }

            for (int i = 0; i < types.Count; ++i)
            {
                resources[types[i]].value -= values[i];
            }
            return true;
        }
    }

    // public bool TrySubResourceValue(ResourceType resourceType, int value)
    // {
    //     while (true)
    //     {
    //         int rv = resources[resourceType].value;
    //         int nv = rv - value;
    //         if (nv >= 0)
    //         {
    //             if (Interlocked.CompareExchange(ref resources[resourceType].value, nv, rv) == nv)
    //             {
    //                 return true;
    //             }
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }
    // }

    private static ResourceManager obj;

    public static ResourceManager Instance()
    {
        if (obj != null)
        {
            return obj;
        }
        return obj = new ResourceManager();
    }
}
