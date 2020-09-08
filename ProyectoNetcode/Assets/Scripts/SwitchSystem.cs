﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.NetCode.ClientServerBootstrap;

[UpdateInWorld(UpdateInWorld.TargetWorld.Default)]
[UpdateInGroup(typeof(InitializationSystemGroup))]

public class SwitchSystem : SystemBase
{

    internal struct State
    {
        public List<Type> DefaultWorldSystems;
        public List<Type> ExplicitDefaultWorldSystems;
        public List<Type> ClientInitializationSystems;
        public List<Type> ClientSimulationSystems;
        public List<Type> ClientPresentationSystems;
        public List<Tuple<Type, Type>> ClientChildSystems;
        public List<Type> ServerInitializationSystems;
        public List<Type> ServerSimulationSystems;
        public List<Tuple<Type, Type>> ServerChildSystems;
    }

    internal static State s_State;
    protected override void OnUpdate()
    {
        Entities.
            WithStructuralChanges().
            ForEach(
                    (Entity entity, in Switch sceneSwitch) =>
                    {
                            // Destroy the switch.
                            EntityManager.DestroyEntity(entity);
                            // Replace the scene.
                            LoadScene(sceneSwitch);
                    }
                   ).
            WithName("SwitchSystem").
            WithoutBurst().
            Run();
        
    }

    void LoadScene(Switch sceneSwitch)
    {
        PlayType playModeType = RequestedPlayType;
        if (sceneSwitch.switchClientorServer == "server")
        {
            playModeType =PlayType.ClientAndServer;

        }
        if (sceneSwitch.switchClientorServer == "client")
        {
            playModeType = PlayType.Client;

        }

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        GenerateSystemLists(systems);

        var world = new World("Default World");
        World.DefaultGameObjectInjectionWorld = world;

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, ExplicitDefaultWorldSystems);
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

        
        int numClientWorlds = RequestedNumClients;

        int totalNumClients = numClientWorlds;
        if (playModeType != PlayType.Server)
        {
#if UNITY_EDITOR
            int numThinClients = RequestedNumThinClients;
            totalNumClients += numThinClients;
#endif
            for (int i = 0; i < numClientWorlds; ++i)
            {
                CreateClientWorld(world, "ClientWorld" + i);
            }
#if UNITY_EDITOR
            for (int i = numClientWorlds; i < totalNumClients; ++i)
            {
                var clientWorld = CreateClientWorld(world, "ClientWorld" + i);
                clientWorld.EntityManager.CreateEntity(typeof(ThinClientComponent));
            }
#endif
        }

        if (playModeType != PlayType.Client)
        {
            CreateServerWorld(world, "ServerWorld");
        }


        var sceneName = sceneSwitch.switchName.ToString();
        var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        async.allowSceneActivation = true;
        

    }
    protected internal static void GenerateSystemLists(IReadOnlyList<Type> systems)
    {
        s_State.DefaultWorldSystems = new List<Type>();
        s_State.ExplicitDefaultWorldSystems = new List<Type>();
        s_State.ClientInitializationSystems = new List<Type>();
        s_State.ClientSimulationSystems = new List<Type>();
        s_State.ClientPresentationSystems = new List<Type>();
        s_State.ClientChildSystems = new List<Tuple<Type, Type>>();
        s_State.ServerInitializationSystems = new List<Type>();
        s_State.ServerSimulationSystems = new List<Type>();
        s_State.ServerChildSystems = new List<Tuple<Type, Type>>();

        foreach (var type in systems)
        {
            var targetWorld = GetSystemAttribute<UpdateInWorld>(type);
            if ((targetWorld != null && targetWorld.World == UpdateInWorld.TargetWorld.Default) ||
                type == typeof(InitializationSystemGroup) ||
                type == typeof(SimulationSystemGroup) ||
                type == typeof(PresentationSystemGroup) ||
                type == typeof(ConvertToEntitySystem))
            {
                DefaultWorldSystems.Add(type);
                ExplicitDefaultWorldSystems.Add(type);
                continue;
            }

            var groups = TypeManager.GetSystemAttributes(type, typeof(UpdateInGroupAttribute)); ;
            if (groups.Length == 0)
            {
                groups = new Attribute[] { new UpdateInGroupAttribute(typeof(SimulationSystemGroup)) };
            }

            foreach (var grp in groups)
            {
                var group = grp as UpdateInGroupAttribute;
                if (group.GroupType == typeof(ClientAndServerSimulationSystemGroup) ||
                    group.GroupType == typeof(SimulationSystemGroup))
                {
                    if (group.GroupType == typeof(ClientAndServerSimulationSystemGroup) && targetWorld != null && targetWorld.World == UpdateInWorld.TargetWorld.Default)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use [UpdateInWorld(UpdateInWorld.TargetWorld.Default)] when using [UpdateInGroup(typeof(ClientAndServerSimulationSystemGroup)] {0}",
                            type));
                    if (group.GroupType == typeof(SimulationSystemGroup) &&
                        (targetWorld == null || targetWorld.World == UpdateInWorld.TargetWorld.Default))
                    {
                        DefaultWorldSystems.Add(type);
                        if (targetWorld != null)
                            ExplicitDefaultWorldSystems.Add(type);
                    }
                    if (targetWorld == null || (targetWorld.World & UpdateInWorld.TargetWorld.Server) != 0)
                    {
                        s_State.ServerSimulationSystems.Add(type);
                    }
                    if (targetWorld == null || (targetWorld.World & UpdateInWorld.TargetWorld.Client) != 0)
                    {
                        s_State.ClientSimulationSystems.Add(type);
                    }
                }
                else if (group.GroupType == typeof(ClientAndServerInitializationSystemGroup) ||
                         group.GroupType == typeof(InitializationSystemGroup))
                {
                    if (group.GroupType == typeof(ClientAndServerInitializationSystemGroup) && targetWorld != null && targetWorld.World == UpdateInWorld.TargetWorld.Default)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use [UpdateInWorld(UpdateInWorld.TargetWorld.Default)] when using [UpdateInGroup(typeof(ClientAndServerInitializationSystemGroup)] {0}",
                            type));
                    if (group.GroupType == typeof(InitializationSystemGroup) &&
                        (targetWorld == null || targetWorld.World == UpdateInWorld.TargetWorld.Default))
                    {
                        DefaultWorldSystems.Add(type);
                        if (targetWorld != null)
                            ExplicitDefaultWorldSystems.Add(type);
                    }
                    if (targetWorld == null || (targetWorld.World & UpdateInWorld.TargetWorld.Server) != 0)
                    {
                        s_State.ServerInitializationSystems.Add(type);
                    }
                    if (targetWorld == null || (targetWorld.World & UpdateInWorld.TargetWorld.Client) != 0)
                    {
                        s_State.ClientInitializationSystems.Add(type);
                    }
                }
                else if (group.GroupType == typeof(ServerInitializationSystemGroup))
                {
                    if (targetWorld != null)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use [UpdateInWorld] when using [UpdateInGroup(typeof(ServerInitializationSystemGroup)] {0}",
                            type));
                    s_State.ServerInitializationSystems.Add(type);
                }
                else if (group.GroupType == typeof(ClientInitializationSystemGroup))
                {
                    if (targetWorld != null)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use [UpdateInWorld] when using [UpdateInGroup(typeof(ClientInitializationSystemGroup)] {0}",
                            type));
                    s_State.ClientInitializationSystems.Add(type);
                }
                else if (group.GroupType == typeof(ServerSimulationSystemGroup))
                {
                    if (targetWorld != null)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use [UpdateInWorld] when using [UpdateInGroup(typeof(ServerSimulationSystemGroup)] {0}",
                            type));
                    s_State.ServerSimulationSystems.Add(type);
                }
                else if (group.GroupType == typeof(ClientSimulationSystemGroup))
                {
                    if (targetWorld != null)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use [UpdateInWorld] when using [UpdateInGroup(typeof(ClientSimulationSystemGroup)] {0}",
                            type));
                    s_State.ClientSimulationSystems.Add(type);
                }
                else if (group.GroupType == typeof(ClientPresentationSystemGroup) ||
                         group.GroupType == typeof(PresentationSystemGroup))
                {
                    if (group.GroupType == typeof(ClientPresentationSystemGroup) && targetWorld != null)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use [UpdateInWorld] when using [UpdateInGroup(typeof(ClientPresentationSystemGroup)] {0}",
                            type));
                    if (targetWorld != null && (targetWorld.World & UpdateInWorld.TargetWorld.Server) != 0)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot use presentation systems on the server {0}",
                            type));
                    if (group.GroupType == typeof(PresentationSystemGroup) &&
                        (targetWorld == null || targetWorld.World == UpdateInWorld.TargetWorld.Default))
                    {
                        DefaultWorldSystems.Add(type);
                        if (targetWorld != null)
                            ExplicitDefaultWorldSystems.Add(type);
                    }
                    if (targetWorld == null || (targetWorld.World & UpdateInWorld.TargetWorld.Client) != 0)
                    {
                        s_State.ClientPresentationSystems.Add(type);
                    }
                }
                else
                {
                    var mask = GetTopLevelWorldMask(group.GroupType);
                    if (targetWorld != null && targetWorld.World == UpdateInWorld.TargetWorld.Default &&
                        (mask & WorldType.DefaultWorld) == 0)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot update in default world when parent is not in the default world {0}", type));
                    if ((targetWorld != null && (targetWorld.World & UpdateInWorld.TargetWorld.Client) != 0) &&
                        (mask & WorldType.ClientWorld) == 0)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot update in client world when parent is not in the client world {0}", type));
                    if ((targetWorld != null && (targetWorld.World & UpdateInWorld.TargetWorld.Server) != 0) &&
                        (mask & WorldType.ServerWorld) == 0)
                        UnityEngine.Debug.LogWarning(String.Format(
                            "Cannot update in server world when parent is not in the server world {0}", type));
                    if ((mask & WorldType.DefaultWorld) != 0 &&
                        (targetWorld == null || targetWorld.World == UpdateInWorld.TargetWorld.Default))
                    {
                        DefaultWorldSystems.Add(type);
                        if (targetWorld != null || (mask & WorldType.ExplicitWorld) != 0)
                            ExplicitDefaultWorldSystems.Add(type);
                    }
                    if ((mask & WorldType.ClientWorld) != 0 &&
                        (targetWorld == null || (targetWorld.World & UpdateInWorld.TargetWorld.Client) != 0))
                    {
                        s_State.ClientChildSystems.Add(new Tuple<Type, Type>(type, group.GroupType));
                    }
                    if ((mask & WorldType.ServerWorld) != 0 &&
                        (targetWorld == null || (targetWorld.World & UpdateInWorld.TargetWorld.Server) != 0))
                    {
                        s_State.ServerChildSystems.Add(new Tuple<Type, Type>(type, group.GroupType));
                    }
                }
            }
        }
    }

    protected static T GetSystemAttribute<T>(Type systemType)
            where T : Attribute
    {
        var attribs = TypeManager.GetSystemAttributes(systemType, typeof(T));
        if (attribs.Length != 1)
            return null;
        return attribs[0] as T;
    }

    public enum PlayType
    {
        ClientAndServer = 0,
        Client = 1,
        Server = 2
    }
#if UNITY_SERVER
        public static PlayType RequestedPlayType => PlayType.Server;
        public static int RequestedNumClients => 0;
#elif UNITY_EDITOR
    public const int k_MaxNumClients = 4;
    public const int k_MaxNumThinClients = 32;
    public static PlayType RequestedPlayType =>
        (PlayType)UnityEditor.EditorPrefs.GetInt("MultiplayerPlayMode_" + UnityEngine.Application.productName +
                                                  "_Type");

    public static int RequestedNumClients
    {
        get
        {
            int numClientWorlds =
                UnityEditor.EditorPrefs.GetInt("MultiplayerPlayMode_" + UnityEngine.Application.productName +
                                               "_NumClients");
            if (numClientWorlds < 1)
                numClientWorlds = 1;
            if (numClientWorlds > k_MaxNumClients)
                numClientWorlds = k_MaxNumClients;
            return numClientWorlds;
        }
    }

    public static int RequestedNumThinClients
    {
        get
        {
            int numClientWorlds =
                UnityEditor.EditorPrefs.GetInt("MultiplayerPlayMode_" + UnityEngine.Application.productName +
                                               "_NumThinClients");
            if (numClientWorlds < 0)
                numClientWorlds = 0;
            if (numClientWorlds > k_MaxNumThinClients)
                numClientWorlds = k_MaxNumThinClients;
            return numClientWorlds;
        }
    }

    public static string RequestedAutoConnect
    {
        get
        {
            switch (RequestedPlayType)
            {
                case PlayType.Client:
                    return UnityEditor.EditorPrefs.GetString(
                        "MultiplayerPlayMode_" + UnityEngine.Application.productName + "_AutoConnectAddress");
                case PlayType.ClientAndServer:
                    return "127.0.0.1";
                default:
                    return "";
            }
        }
    }
#elif UNITY_CLIENT
        public static PlayType RequestedPlayType => PlayType.Client;
        public static int RequestedNumClients => 1;
#else
        public static PlayType RequestedPlayType => PlayType.ClientAndServer;
        public static int RequestedNumClients => 1;
#endif

    [Flags]
    private enum WorldType
    {
        NoWorld = 0,
        DefaultWorld = 1,
        ClientWorld = 2,
        ServerWorld = 4,
        ExplicitWorld = 8
    }

    static WorldType GetTopLevelWorldMask(Type type)
    {
        var targetWorld = GetSystemAttribute<UpdateInWorld>(type);
        if (targetWorld != null)
        {
            if (targetWorld.World == UpdateInWorld.TargetWorld.Default)
                return WorldType.DefaultWorld | WorldType.ExplicitWorld;
            if (targetWorld.World == UpdateInWorld.TargetWorld.Client)
                return WorldType.ClientWorld;
            if (targetWorld.World == UpdateInWorld.TargetWorld.Server)
                return WorldType.ServerWorld;
            return WorldType.ClientWorld | WorldType.ServerWorld;
        }

        var groups = TypeManager.GetSystemAttributes(type, typeof(UpdateInGroupAttribute));
        if (groups.Length == 0)
        {
            if (type == typeof(ClientAndServerSimulationSystemGroup) ||
                type == typeof(ClientAndServerInitializationSystemGroup))
                return WorldType.ClientWorld | WorldType.ServerWorld;
            if (type == typeof(SimulationSystemGroup) || type == typeof(InitializationSystemGroup))
                return WorldType.DefaultWorld | WorldType.ClientWorld | WorldType.ServerWorld;
            if (type == typeof(ServerSimulationSystemGroup) || type == typeof(ServerInitializationSystemGroup))
                return WorldType.ServerWorld;
            if (type == typeof(ClientSimulationSystemGroup) ||
                type == typeof(ClientInitializationSystemGroup) ||
                type == typeof(ClientPresentationSystemGroup))
                return WorldType.ClientWorld;
            if (type == typeof(PresentationSystemGroup))
                return WorldType.DefaultWorld | WorldType.ClientWorld;
            // Empty means the same thing as SimulationSystemGroup
            return WorldType.DefaultWorld | WorldType.ClientWorld | WorldType.ServerWorld;
        }

        WorldType mask = WorldType.NoWorld;
        foreach (var grp in groups)
        {
            var group = grp as UpdateInGroupAttribute;
            mask |= GetTopLevelWorldMask(group.GroupType);
        }

        return mask;
    }

}