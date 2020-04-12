using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextController : MonoBehaviour
{
    private static DamageText damageText;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("UICanvas");
        damageText = Resources.Load<DamageText>("Prefabs/DmgPopParent");
    }

    public static void CreateFloatingText(string text, Transform location)
    {
        DamageText instance = Instantiate(damageText);
        instance.SetText(text);
        instance.transform.SetParent(canvas.transform, false);

    }
}
