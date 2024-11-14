using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToLevel1 : MonoBehaviour
{
    private void OnTriggerEnter(Collision other)
    {
        SceneManager.LoadScene("Level_1");
    }
}
