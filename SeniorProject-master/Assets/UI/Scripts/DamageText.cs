using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Animator animator;
    private Text damageAmountText;

    void Start()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
        damageAmountText = animator.GetComponent<Text>();
        
    }

    public void SetText(string text)
    {
        damageAmountText.text = text;

    }
}
