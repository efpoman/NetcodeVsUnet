using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Unity.NetCode;
using UnityEngine.SceneManagement;

[Serializable]
public class ButtonConnectToServer : MonoBehaviour
{
    
    public void changeScene(string mode)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();
        
        entityManager.AddComponentData(entity,new Switch {switchName="OnlineScene", switchClientorServer=mode } );
        
        
    }
}
