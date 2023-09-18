using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingPrototype
{
    Invalid,
    Converter,
    InventoryInterface,
    MinionHouse,
    NeedsStation,
    LogisticRoad,
}

public enum BuildingCategory
{
    Invalid,
    Food,
    Metal,
    Energy,
    Minion,
}

public enum ResourceType
{
    Invalid,
    D_Select,
    O_IronOre,
    O_CopperOre,
    O_Stone,
    O_Limestone,
    O_Coal,
    R_IronIngot,
    R_CopperIngot,
    R_IronPlate,
    R_CopperPlate,
    R_IronRod,
    R_CopperWire,
    R_Sand,
    R_Soil,
    R_Lime,
    R_Wood,
    R_Energy,
    R_Metal,
    R_Water,
    R_Carbohydrate,
    R_Protein,
    R_Fat,
    R_Egg,
    R_Wheat,
    R_ChickenMeat,
    B_InfiniteConverter,
    B_WaterWell,
    B_Farm,
    B_Henhouse,
    B_Hut,
    B_Miner,
    B_Smelter,
    B_Foundry,
    B_Assembler,
    B_Extractor,
    B_Refinery,
    B_Container,
    B_StorageTank,
    B_StoneRoad,
    B_Core,
    B_Sender,
    B_Receiver,
    B_Cross,
    B_SolarPanel_1,
    B_Battery_1,
    B_ArmMotor_1,
    B_FootMotor_1,
}