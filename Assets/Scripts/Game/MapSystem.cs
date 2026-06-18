using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    public static MapSystem Instance { get; private set; }

   

    private void Awake()
    {
        Instance = this;
    }

   
}
