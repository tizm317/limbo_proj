using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CaptchaGenerator : ScriptableObject
{
    public CaptchaAlphabets[] alphabets;

    [SerializeField]
    private const int CodeLength = 6;

    public CaptchaAlphabets[] Generate()
    {
        CaptchaAlphabets[] Captcha = new CaptchaAlphabets[CodeLength];
        for (int i = 0; i < CodeLength; i++)
        {
            int idx = Random.Range(0, 26);
            Captcha[i] = alphabets[idx];
        }
        return Captcha;
    }

    public bool IsCodeValid(string input, CaptchaAlphabets[] captcha)
    {
        int i = 0;

        if (input.Length == 0)
            return false;
        
        // 대소문자 구분X
        input = input.ToUpper();

        foreach (char c in input)
        {
            if (captcha[i] == null || c != captcha[i++].Value )
                return false;
        }

        Debug.Log($"Input : {input}");
        return true;
    }
}
