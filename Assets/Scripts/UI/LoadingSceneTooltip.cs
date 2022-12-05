using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadingScene", menuName = "Tooltip", order = 1)]
public class LoadingSceneTooltip : ScriptableObject
{
    public string[] Tooltips {get {return  _tooltips;} set {_tooltips = value;}}
    [SerializeField]
    private string[] _tooltips;
}
