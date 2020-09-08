using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


public class CreateBootstrap : ClientServerBootstrap
{
 
    
    public override bool Initialize(string defaultWorldName)
    {
         if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "OnlineScene")
            return base.Initialize(defaultWorldName);

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        GenerateSystemLists(systems);

        var world = new World(defaultWorldName);
        World.DefaultGameObjectInjectionWorld = world;

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, ExplicitDefaultWorldSystems);
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        return true;
    




        /* 
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "OnlineScene")
            return base.Initialize(defaultWorldName);

     

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
            GenerateSystemLists(systems);

            var world = new World(defaultWorldName);
            World.DefaultGameObjectInjectionWorld = world;

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, DefaultWorldSystems);
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
            return true; 
            */
        
    }

  
}


