using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Equipment : UI_Popup
{
    Transform player;
    Camera selfCam;
    float x_cam;
    float z_cam;
    float r;

    Player player_State;
    private GameObject Scene;

    // 착용한 아이템 딕셔너리
    // 여기서 직접 관리?
    Dictionary<string, Item> _EquipItemDict = new Dictionary<string, Item>();

    enum GameObjects
    {
        Slider,
        Head,
        Body,
        Pants,
        Shoes,
        Weapon,
        EnchantStone1,
        EnchantStone2,
        EnchantStone3,
    }

    private void Start()
    {
        Init();
    }

    // 원래 스프라이트
    Sprite[] spArr;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.Slider).BindEvent(onSliderDrag, Define.UIEvent.Drag);

        Scene = GameObject.Find("@Scene");
        player_State = Scene.GetComponent<Player>();
        player = player_State.GetPlayer().transform;
        //player = GameObject.Find("Player").transform;
        selfCam = player.Find("SelfCam").GetComponent<Camera>();
        selfCam.gameObject.SetActive(true);

        // 캠 위치,방향 초기화
        selfCam.transform.localPosition = new Vector3(0f, 1.2f, 3f);
        selfCam.transform.LookAt(player.GetChild(0)); // Hips 기준

        // r 계산
        x_cam = selfCam.transform.position.x;
        z_cam = selfCam.transform.position.z;
        r = Vector2.Distance(new Vector2(x_cam, z_cam), new Vector2(player.position.x, player.position.z));

        //
        _gr = Util.GetOrAddComponent<GraphicRaycaster>(this.gameObject);
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

        // 원래 스프라이트 저장
        spArr = new Sprite[8];
        for(int i = 1; i < 8; i++)
        {
            spArr[i-1] = GetObject(i).transform.GetChild(0).GetComponent<Image>().sprite;
        }
    }

    private void Update()
    {
        _ped.position = Input.mousePosition;
        OnPointerUp();
    }

    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(1))
        {
            UI_EquipSlot slot = RaycastAndGetFirstComponent<UI_EquipSlot>();
            if (slot != null)
                TakeOff(slot.gameObject);
        }
    }

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

    private void TakeOff(GameObject slot)
    {
        if (_EquipItemDict.ContainsKey(slot.name) == false) return;

        Item takeOff_Item = _EquipItemDict[slot.name];
        
        // 딕셔너리에서 제거
        _EquipItemDict.Remove(slot.name);

        // 인벤토리에 추가
        Inventory inventory = GameObject.Find("@Scene").GetComponent<Inventory>();
        int tempIdx;
        inventory.Add(takeOff_Item.Data, out tempIdx);

        // 아이콘 제거
        int idx = 0;
        switch (slot.name)
        {
            case "Head":
                idx = (int)GameObjects.Head;
                break;
            case "Body":
                idx = (int)GameObjects.Body;
                break;
            case "Pants":
                idx = (int)GameObjects.Pants;
                break;
            case "Shoes":
                idx = (int)GameObjects.Shoes;
                break;
            case "Weapon":
                idx = (int)GameObjects.Weapon;
                break;
            default:
                idx = (int)GameObjects.EnchantStone1;
                break;
        }
        slot.transform.GetChild(0).GetComponent<Image>().sprite = spArr[idx - 1];
    }

    public void onSliderDrag(PointerEventData data)
    {
        // 카메라 회전
        x_cam = selfCam.transform.position.x;
        z_cam = selfCam.transform.position.z;
        float sValue = GetObject((int)GameObjects.Slider).GetComponent<Slider>().value; // 0 ~ 1

        x_cam = player.position.x + r * Mathf.Sin(sValue * 2 * Mathf.PI);
        if (sValue > 0.25f && sValue <= 0.75f)
            z_cam = player.position.z + Mathf.Sqrt(r * r - (x_cam - player.position.x) * (x_cam - player.position.x));
        else
            z_cam = player.position.z - Mathf.Sqrt(r * r - (x_cam - player.position.x) * (x_cam - player.position.x));

        selfCam.transform.position = new Vector3(x_cam, selfCam.transform.position.y, z_cam);
        selfCam.transform.LookAt(player.GetChild(0)); // Hips 기준
    }
    private void OnDisable()
    {
        if(selfCam)
            selfCam.gameObject.SetActive(false);
    }

    internal bool Equip(EquipmentItem equipmentItem, out EquipmentItem exchangedItem)
    {
        // 교체된 아이템
        exchangedItem = null;

        // 무기
        if(equipmentItem is WeaponItem weaponItem)
        {
            if (player_State.Job != weaponItem.Class)
            {
                // 다른 클래스 무기 착용 불가
                Debug.Log($"This Weapon Is For {weaponItem.Class}.");
                return false;
            }

            // 이미 착용중이면 교체
            if (_EquipItemDict.ContainsKey("Weapon"))
            {
                exchangedItem = (EquipmentItem)_EquipItemDict["Weapon"];
                _EquipItemDict["Weapon"] = weaponItem;
            }
            else // 없으면 추가
            {
                _EquipItemDict["Weapon"] = weaponItem;
            }

            // 아이콘 변경
            GetObject((int)GameObjects.Weapon).transform.GetChild(0).GetComponent<Image>().sprite = weaponItem.Data.IconSprite;

        }
        // 방어구
        else if(equipmentItem is ArmorItem armorItem)
        {
            ArmorItem a =  (ArmorItem)armorItem;
            // 이미 있으면 교체
            if (_EquipItemDict.ContainsKey(a.Part))
            {
                exchangedItem = (EquipmentItem)_EquipItemDict[a.Part];
                _EquipItemDict[a.Part] = a;
            }
            else // 없으면 착용
            {
                _EquipItemDict[a.Part] = a;
            }

            // 아이콘 변경
            int idx = 0;
            switch (a.Part)
            {
                case "Head":
                    idx = (int)GameObjects.Head;
                    break;
                case "Body":
                    idx = (int)GameObjects.Body;
                    break;
                case "Pants":
                    idx = (int)GameObjects.Pants;
                    break;
                case "Shoes":
                    idx = (int)GameObjects.Shoes;
                    break;
            }
            GetObject(idx).transform.GetChild(0).GetComponent<Image>().sprite = armorItem.Data.IconSprite;
        }
        return true;
    }

}
