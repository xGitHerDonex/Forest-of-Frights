using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveUI : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
