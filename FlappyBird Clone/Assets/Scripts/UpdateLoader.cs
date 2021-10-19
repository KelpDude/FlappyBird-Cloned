using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLoader : MonoBehaviour
{
    private void Update()
    {
        Loader.LoadTargetScene();
    }
}
