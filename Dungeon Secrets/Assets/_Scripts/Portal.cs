using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Portal : Collide
{
    public string[] sceneNames;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            GameManager.instance.SaveState();
            SceneManager.LoadScene(sceneNames[0]);
        }
    }
}
