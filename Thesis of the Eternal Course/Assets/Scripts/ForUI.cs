using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForUI : MonoBehaviour
{
    public GameObject E_Button;

    void Update()
    {
        if (Player.IsDialog == true)
            E_Button.SetActive(true);
        else
            E_Button.SetActive(false);
    }

    public static void ActivatedDialog(int index)
    {

    }
}
