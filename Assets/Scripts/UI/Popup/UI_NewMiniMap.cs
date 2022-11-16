using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_NewMiniMap : UI_Popup
{
    // MiniMap New Version
    
    enum Texts
    {
        MapNameText,
    }

    void Start()
    {
        Init();
    }

    Image[] images;
    RawImage mapImg;
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;
    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();
        _gr.Raycast(_ped, _rrList);
        if (_rrList.Count == 0) return null;
        return _rrList[0].gameObject.GetComponent<T>();
    }


    private void Update()
    {
        _ped.position = Input.mousePosition;
        OnPointerEnter(_ped.position);
        OnPointerExit(_ped.position);
    }

    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));

        // 맵 이름 초기화
        Text _MapNameText = GetText((int)Texts.MapNameText);
        string SceneName = SceneManager.GetActiveScene().name;
        if (SceneName.Contains("InGame"))
            SceneName = SceneName.Substring(6);

        string mapName = "";
        switch (SceneName)
        {
            case "Village":
                mapName = "Miðgarðr";
                break;
            case "Nature":
                mapName = "Járnviðr";
                break;
            case "Desert":
                mapName = "Múspellsheimr";
                break;
            case "Cemetery":
                mapName = "Helheim";
                break;
        }
        _MapNameText.text = mapName;
 


        images = GetComponentsInChildren<Image>();
        mapImg = GetComponentInChildren<RawImage>();

        _gr = Util.GetOrAddComponent<GraphicRaycaster>(this.gameObject);
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);
    }

    UI_NewMiniMap UI_miniMap;
    private void OnPointerEnter(Vector2 pointer)
    {
        //if (UI_miniMap == RaycastAndGetFirstComponent<UI_NewMiniMap>())
        //    return;
        UI_miniMap = RaycastAndGetFirstComponent<UI_NewMiniMap>();
        RawImage rawImage = RaycastAndGetFirstComponent<RawImage>();
        Mask mask = RaycastAndGetFirstComponent<Mask>();
        if (UI_miniMap == null && rawImage == null && mask == null) return;

        //if (mask == RaycastAndGetFirstComponent<Mask>()) return;
        //mask = RaycastAndGetFirstComponent<Mask>();
        //if (mask == null) return;

        // 불투명
        foreach (Image img in images)
        {
            img.color = new Color(img.color.r , img.color.g, img.color.b, 1.0f);
        }
        mapImg.color = new Color(mapImg.color.r, mapImg.color.g, mapImg.color.b, 1.0f);
    }

    private void OnPointerExit(Vector2 pointer)
    {
        UI_NewMiniMap miniMap = RaycastAndGetFirstComponent<UI_NewMiniMap>();
        RawImage rawImage = RaycastAndGetFirstComponent<RawImage>();
        Mask mask = RaycastAndGetFirstComponent<Mask>();
        if (miniMap != null || rawImage != null || mask != null) return;

        //Mask mask = RaycastAndGetFirstComponent<Mask>();
        //if (mask != null) return;

        // 반투명
        foreach (Image img in images)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.2f);
        }
        mapImg.color = new Color(mapImg.color.r, mapImg.color.g, mapImg.color.b, 0.2f);
    }
}
