﻿using System;
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

    // 착용한 아이템 리스트
    // 여기서 직접 관리?
    List<Item> _EquipItems = new List<Item>();

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
            if(GetObject((int)GameObjects.Weapon).transform.GetChild(0).GetComponent<Image>().sprite != null)
            {
                // 이미 착용중
                // 교체
                foreach(EquipmentItem e in _EquipItems)
                {
                    if(e is WeaponItem)
                    {
                        exchangedItem = e;

                        _EquipItems.Remove(e);
                        break;
                    }
                }
            }

            // 리스트에 추가
            _EquipItems.Add(weaponItem);

            // 아이콘 변경
            GetObject((int)GameObjects.Weapon).transform.GetChild(0).GetComponent<Image>().sprite = weaponItem.Data.IconSprite;

        }
        // 방어구
        else if(equipmentItem is ArmorItem armorItem)
        {
            ArmorItem a =  (ArmorItem)armorItem;
            int idx = 0;
            switch(a.Part)
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
            if(GetObject(idx).transform.GetChild(0).GetComponent<Image>().sprite != null)
            {
                // 이미 착용중
                // 교체
                foreach (EquipmentItem e in _EquipItems)
                {
                    if (e is ArmorItem)
                    {
                        ArmorItem a2 = (ArmorItem)e;

                        // 방어구 부위가 다르면 continue
                        if (a.Part != a2.Part) continue;

                        exchangedItem = e;

                        _EquipItems.Remove(e);
                        break;
                    }
                }
            }

            // 리스트에 추가
            _EquipItems.Add(a);
            // 아이콘 변경
            GetObject(idx).transform.GetChild(0).GetComponent<Image>().sprite = armorItem.Data.IconSprite;

        }
        return true;
    }
}
