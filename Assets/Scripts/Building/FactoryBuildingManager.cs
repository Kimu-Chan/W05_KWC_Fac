using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryBuildingBuilder
{
    private FactoryBuildingClass cls;

    private FactoryBuildingBuilder()
    {
        cls = new FactoryBuildingClass();
    }

    public static FactoryBuildingBuilder StartBuild()
    {
        return new FactoryBuildingBuilder();
    }
    public FactoryBuildingBuilder SetBuildingName(string name)
    {
        cls.buildingName = name;
        return this;
    }
    public FactoryBuildingBuilder Size(int xSize, int ySize, int xCenter, int yCenter)
    {
        cls.xSize = xSize;
        cls.ySize = ySize;
        cls.xCenter = xCenter;
        cls.yCenter = yCenter;
        return this;
    }

    public FactoryBuildingBuilder BuildDragAble()
    {
        cls.isBuildDragAble = true;
        return this;
    }

    public FactoryBuildingBuilder SetCategory(BuildingCategory category)
    {
        cls.buildingCategory = category;
        return this;
    }

    public FactoryBuildingBuilder AppendCost(ResourceType resourceType, int value)
    {
        if (cls.costs.ContainsKey(resourceType))
        {
            cls.costs[resourceType] += value;
        }
        else
        {
            cls.costs.Add(resourceType, value);
        }
        return this;
    }

    public FactoryBuildingBuilder SetLogistics()
    {
        cls.isLogisticsAvailable = true;
        return this;
    }

    public FactoryBuildingBuilder DisableOverlay()
    {
        cls.hasOverlay = false;
        return this;
    }

    public FactoryBuildingBuilder AppendAddon(ResourceType resourceType)
    {
        cls.addons.Add(resourceType);
        return this;
    }

    public FactoryBuildingBuilder CanBuildCount(int count)
    {
        cls.canBuildCount = count;
        return this;
    }

    public FactoryBuildingBuilder AssemblyRequired()
    {
        cls.isAssemblyRequired = true;
        return this;
    }

    public FactoryBuildingBuilder CanHaveKeyBindingAction()
    {
        cls.canHaveKeyBindingAction = true;
        return this;
    }

    public FactoryBuildingBuilder IsPipeSender()
    {
        cls.isPipeSender = true;
        return this;
    }

    public FactoryBuildingBuilder IsPipeReciver()
    {
        cls.isPipeReciver = true;
        return this;
    }

    public FactoryBuildingBuilder SetPrototype(BuildingPrototype prototype)
    {
        cls.prototype = prototype;
        return this;
    }

    public FactoryBuildingBuilder SetPrefab(string prefabName)
    {
        cls.prefabName = prefabName;
        return this;
    }

    public FactoryBuildingClass Build()
    {
        // prefab name 처리
        // if (cls.prefabName == null)
        // {
        //     cls.prefabName = $"{cls.xSize}x{cls.ySize}_Cube";
        // }
        return cls;
    }
}

public class FactoryBuildingManager
{
    private Dictionary<ResourceType, FactoryBuildingClass> buildings;
    private static FactoryBuildingManager obj;

    public List<ResourceType> GetBuildingListByBuildingCategory(BuildingCategory buildingCategory)
    {
        return buildings.Where((k) => k.Value.buildingCategory == buildingCategory).Select((k) => k.Key).ToList();
    }

    public List<ResourceType> BuildingResourceTypes
    {
        get
        {
            return buildings.Keys.ToList();
        }
    }

    public FactoryBuildingClass GetBuildingClass(ResourceType type)
    {
        return buildings[type];
    }

    public static FactoryBuildingManager Instance
    {
        get
        {
            if (obj != null)
            {
                return obj;
            }
            return obj = new FactoryBuildingManager();
        }
    }

    public bool CheckFactoryBuildingCost(ResourceType buildingType)
    {
        FactoryBuildingClass result;
        if (!buildings.TryGetValue(buildingType, out result))
        {
            return false;
        }

        // TODO: 수정

        // 조립기 필요 없음 == 블루프린트 방식이므로 비용 확인하지 않음

        if (!result.isAssemblyRequired)
        {
            return true;
        }

        // 조립기 필요함 = 해당 건물이 인벤토리에 있는지 확인

        ResourceManager rm = ResourceManager.Instance();

        foreach (var cost in result.costs)
        {
            int value = rm.GetResourceValue(cost.Key);
            if (value < cost.Value)
            {
                return false;
            }
        }


        return true;
    }

    private void RegistBuilding(ResourceType resourceType, FactoryBuildingClass cls)
    {
        cls.resourceType = resourceType;
        buildings.Add(resourceType, cls);
    }

    private void InitFoodCategoryFactoryBuildingBuilder()
    {
        {
            // Water Well
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(3, 3, 1, 1)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .SetPrototype(BuildingPrototype.Converter)
                .SetBuildingName("Water Well")
                .SetCategory(BuildingCategory.Food)
                .SetLogistics()
                .AppendCost(ResourceType.R_IronIngot, 10)
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                ConverterPrototype prototype = (ConverterPrototype)rawPrototype;
                // 1초에 Water 10개 생산
                prototype.SetOutput(ResourceType.R_Water, 10);
                prototype.IntervalTick = GameManager.MillisecondToTick(1000);
            };

            RegistBuilding(ResourceType.B_WaterWell, cls);
        }

        {
            // Farm

            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(5, 5, 2, 2)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .SetPrototype(BuildingPrototype.Converter)
                .SetCategory(BuildingCategory.Food)
                .SetLogistics()
                .SetBuildingName("Farm")
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                ConverterPrototype prototype = (ConverterPrototype)rawPrototype;
                // 1초에 Wheat 5개 생산
                prototype.SetOutput(ResourceType.R_Wheat, 4);
                prototype.IntervalTick = GameManager.MillisecondToTick(1000);
            };

            RegistBuilding(ResourceType.B_Farm, cls);
        }

        {
            // Henhouse

            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(3, 3, 1, 1)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .SetPrototype(BuildingPrototype.Converter)
                .SetCategory(BuildingCategory.Food)
                .SetLogistics()
                .SetBuildingName("Henhouse")
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                ConverterPrototype prototype = (ConverterPrototype)rawPrototype;
                // 10초에 Wheat 4개 소모, Egg 1 생산
                prototype.SetInput(ResourceType.R_Wheat, 5);
                prototype.SetOutput(ResourceType.R_Egg, 1);
                // prototype.SetOutput(ResourceType.R_Fat, 1);
                prototype.IntervalTick = GameManager.MillisecondToTick(10000);
            };

            RegistBuilding(ResourceType.B_Henhouse, cls);
        }

    }

    private void InitMetalCategoryFactoryBuildingBuilder()
    {
        {
            // Miner

            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(3, 3, 1, 1)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .SetPrototype(BuildingPrototype.Converter)
                .SetCategory(BuildingCategory.Metal)
                .SetLogistics()
                .SetBuildingName("Miner")
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                ConverterPrototype prototype = (ConverterPrototype)rawPrototype;
                // 4초에 IronOre 5 생산
                prototype.SetOutput(ResourceType.O_IronOre, 5);
                prototype.IntervalTick = GameManager.MillisecondToTick(4000);
            };

            RegistBuilding(ResourceType.B_Miner, cls);
        }

        {
            // Smelter

            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(3, 3, 1, 1)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .SetPrototype(BuildingPrototype.Converter)
                .SetCategory(BuildingCategory.Metal)
                .SetLogistics()
                .SetBuildingName("Smelter")
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                ConverterPrototype prototype = (ConverterPrototype)rawPrototype;
                // 2초에 IronOre 2 소모, IronIngot 1 생산
                prototype.SetInput(ResourceType.O_IronOre, 2);
                prototype.SetOutput(ResourceType.R_IronIngot, 1);
                prototype.IntervalTick = GameManager.MillisecondToTick(2000);
            };

            RegistBuilding(ResourceType.B_Smelter, cls);
        }

        {
            // Assembler

            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(5, 5, 1, 1)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .SetPrototype(BuildingPrototype.Converter)
                .SetCategory(BuildingCategory.Metal)
                .SetLogistics()
                .SetBuildingName("Assembler")
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                ConverterPrototype prototype = (ConverterPrototype)rawPrototype;
                // 2초에 IronIngot 1 소모, IronPlate 2 생산
                prototype.SetInput(ResourceType.R_IronIngot, 1);
                prototype.SetOutput(ResourceType.R_IronPlate, 2);
                prototype.IntervalTick = GameManager.MillisecondToTick(2000);
            };

            RegistBuilding(ResourceType.B_Assembler, cls);
        }
    }

    private void InitEnergyCategoryFactoryBuildingBuilder()
    {

    }

    private void InitMinionCategoryFactoryBuildingBuilder()
    {

        {
            // Hut

            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(3, 3, 1, 1)
                .SetPrototype(BuildingPrototype.MinionHouse)
                .SetBuildingName("Hut")
                .SetCategory(BuildingCategory.Minion)
                .SetLogistics()
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                MinionHousePrototype prototype = (MinionHousePrototype)rawPrototype;
            };

            RegistBuilding(ResourceType.B_Hut, cls);
        }

        {
            // Stone Road

            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(1, 1, 0, 0)
                .SetPrototype(BuildingPrototype.LogisticRoad)
                .SetBuildingName("Stone Road")
                .SetPrefab("StoneRoad")
                .SetCategory(BuildingCategory.Minion)
                .SetLogistics()
                .DisableOverlay()
                .BuildDragAble()
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                LogisticRoadPrototype prototype = (LogisticRoadPrototype)rawPrototype;
            };

            RegistBuilding(ResourceType.B_StoneRoad, cls);
        }
    }

    private void InitFactoryBuildingBuilder()
    {
        buildings = new Dictionary<ResourceType, FactoryBuildingClass>();
        InitFoodCategoryFactoryBuildingBuilder();
        InitMetalCategoryFactoryBuildingBuilder();
        InitEnergyCategoryFactoryBuildingBuilder();
        InitMinionCategoryFactoryBuildingBuilder();
        return;


        {
            // Core
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(5, 5, 2, 2)
                .Build();
            buildings.Add(ResourceType.B_Core, cls);
        }

        {
            // Infinite Input/Output Converter
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(1, 1, 0, 0)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .SetPrototype(BuildingPrototype.Converter)
                .SetBuildingName("Infinite Input/Output Converter")
                .Build();

            cls.initPrototype = (rawPrototype) =>
            {
                ConverterPrototype prototype = (ConverterPrototype)rawPrototype;
                prototype.SetInput(ResourceType.D_Select, 1);
                prototype.SetOutput(ResourceType.D_Select, 1);
            };

            buildings.Add(ResourceType.B_InfiniteConverter, cls);
        }


        // {
        //     // Solar Panel (Organic)
        //     var cls = FactoryBuildingBuilder.StartBuild()
        //         .Size(3, 3, 1, 1)
        //         .AppendCost(ResourceType.R_Energy, 1)
        //         .AppendCost(ResourceType.R_Metal, 1)
        //         .AppendCost(ResourceType.R_Organic, 1)
        //         .AppendAddon(ResourceType.B_Sender)
        //         .AppendAddon(ResourceType.B_Receiver)
        //         .Build();
        //     buildings.Add(ResourceType.B_SolarPanel_1, cls);
        // }

        {
            // Sender
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(1, 1, 0, 0)
                .AppendCost(ResourceType.R_Energy, 1)
                .AppendCost(ResourceType.R_Metal, 1)
                .IsPipeSender()
                .Build();
            buildings.Add(ResourceType.B_Sender, cls);
        }

        {
            // Receiver
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(1, 1, 0, 0)
                .AppendCost(ResourceType.R_Energy, 1)
                .AppendCost(ResourceType.R_Metal, 1)
                .IsPipeReciver()
                .Build();
            buildings.Add(ResourceType.B_Receiver, cls);
        }

        {
            // Cross
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(1, 1, 0, 0)
                .AppendCost(ResourceType.R_Energy, 1)
                .AppendCost(ResourceType.R_Metal, 1)
                .IsPipeReciver()
                .IsPipeSender()
                .Build();
            buildings.Add(ResourceType.B_Cross, cls);
        }


        // {
        //     // Battery (Organic)
        //     var cls = FactoryBuildingBuilder.StartBuild()
        //         .Size(3, 3, 1, 1)
        //         .AppendCost(ResourceType.R_Energy, 1)
        //         .AppendCost(ResourceType.R_Metal, 1)
        //         .AppendCost(ResourceType.R_Organic, 1)
        //         .AppendAddon(ResourceType.B_Sender)
        //         .AppendAddon(ResourceType.B_Receiver)
        //         .Build();
        //     buildings.Add(ResourceType.B_Battery_1, cls);
        // }

        {
            // Arm Motor
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(3, 3, 1, 1)
                .AppendCost(ResourceType.R_Energy, 1)
                .AppendCost(ResourceType.R_Metal, 50)
                .AppendAddon(ResourceType.B_Sender)
                .AppendAddon(ResourceType.B_Receiver)
                .CanBuildCount(2)
                .AssemblyRequired()
                .CanHaveKeyBindingAction()
                .Build();
            buildings.Add(ResourceType.B_ArmMotor_1, cls);
        }

        {
            // Foot Motor
            var cls = FactoryBuildingBuilder.StartBuild()
                .Size(3, 3, 1, 1)
                .AppendCost(ResourceType.R_Energy, 1)
                .AppendCost(ResourceType.R_Metal, 50)
                .CanBuildCount(4)
                .AssemblyRequired()
                .CanHaveKeyBindingAction()
                .Build();
            buildings.Add(ResourceType.B_FootMotor_1, cls);
        }
    }

    private FactoryBuildingManager()
    {
        InitFactoryBuildingBuilder();
    }
}
