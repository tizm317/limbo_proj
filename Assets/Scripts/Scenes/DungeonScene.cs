using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonScene : GameScene
{
    UI_Dungeon _sceneUI;

    protected override void Init()
    {
        SceneType = Define.Scene.Dungeon;

        // �� UI
        if(_sceneUI == null)
            _sceneUI = Managers.UI.ShowSceneUI<UI_Dungeon>("UI_Dungeon");


        // InGame Scene BGM ����

        if (SceneManager.GetActiveScene().name == "DungeonCemetery")
        {
            Managers.Sound.Play("Sound/BGM/BGM_DungeonCemetery", Define.Sound.Bgm);

        }
        else if (SceneManager.GetActiveScene().name == "DungeonNature")
        {
            Managers.Sound.Play("Sound/BGM/BGM_DungeonNature", Define.Sound.Bgm);
        }
        else if (SceneManager.GetActiveScene().name == "DungeonDesert")
        {
            Managers.Sound.Play("Sound/BGM/BGM_DungeonDesert", Define.Sound.Bgm);
        }

        // DataManager test
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;

        // Ŀ����Ʈ�ѷ� @Scene
        gameObject.GetOrAddComponent<CursorController>();
    }

    public override void Clear()
    {
        Debug.Log("Dungeon Scene Clear");
    }

}
