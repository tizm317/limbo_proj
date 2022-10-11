using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Text _amountText;

    public int Index { get; private set; }
    public bool HasItem => (_iconImage.sprite != null);
    public bool IsAccessible => (_isAccessibleSlot && _isAccessibleItem);

    public RectTransform SlotRect => _slotRect;
    public RectTransform IconRect => _iconRect;


    private UI_Inventory _ui_inventory;

    private RectTransform _slotRect;
    private RectTransform _iconRect;

    [SerializeField]
    private GameObject _iconGo;
    [SerializeField]
    private GameObject _textGo;

    private Image _slotImage;

    private bool _isAccessibleSlot = true;
    private bool _isAccessibleItem = true;

    // 비활성화 슬롯, 아이콘 색상
    private Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    private Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);


    /* Methods */
    private void ShowIcon() => _iconGo.SetActive(true);
    private void HideIcon() => _iconGo.SetActive(false);
    private void ShowText() => _textGo.SetActive(true);
    private void HideText() => _textGo.SetActive(false);

    public void SetSlotIndex(int idx) => Index = idx;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _ui_inventory = GetComponentInParent<UI_Inventory>();

        _iconImage = transform.GetChild(0).GetComponent<Image>();
        _amountText = GetComponentInChildren<Text>(true);

        _slotRect = GetComponent<RectTransform>();
        _iconRect = _iconImage.rectTransform;

        _iconGo = _iconRect.gameObject;
        //_textGo = _amountText.gameObject;
        _textGo = transform.GetChild(1).gameObject;

        _slotImage = GetComponent<Image>();
    }

    // 슬롯 활성화 여부 설정(?)
    public void SetSlotAccessibleState(bool value)
    {
        // 중복 처리 방지
        if (_isAccessibleSlot == value) return;

        // 활성화
        if (value)
            _slotImage.color = Color.black;
        else // 비 활성화
        {
            _slotImage.color = InaccessibleSlotColor;
            HideIcon();
            HideText();
        }

        _isAccessibleSlot = value;
    }

    // 아이템 활성화 여부 설정
    public void SetItemAccessibleState(bool value)
    {
        // 중복 처리 방지
        if (_isAccessibleItem == value) return;
        if (value) // 활성화
        {
            _iconImage.color = Color.white;
            _amountText.color = Color.white;
        }
        else // 비활성화
        {
            _iconImage.color = InaccessibleIconColor;
            _amountText.color = InaccessibleIconColor;
        }

        _isAccessibleItem = value;
    }

    // 다른 슬롯과 아이템 아이콘 교환
    public void SwapIcon(UI_ItemSlot otherSlot)
    {
        if (otherSlot == null) return;          // 없는 경우
        if (this == otherSlot) return;          // 나 자신
        if (!this.IsAccessible) return;         // 접근 불가
        if (!otherSlot.IsAccessible) return;    // 접근 불가

        Sprite tempSprite = _iconImage.sprite;

        // 대상에 아이템이 있으면 교환/ 없으면 이동
        if (otherSlot.HasItem) SetItem(otherSlot._iconImage.sprite); // 교환
        else RemoveItem(); // 이동

        otherSlot.SetItem(tempSprite);
    }

    // 슬롯에 아이템 아이콘 등록
    public void SetItem(Sprite tempSprite)
    {
        if (tempSprite == null) RemoveItem();
        else
        {
            _iconImage.sprite = tempSprite;
            ShowIcon();
        }
    }

    // 슬롯에서 아이템 아이콘 제거
    public void RemoveItem()
    {
        _iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    // 아이템 개수 텍스트 설정(amount 1 이하면 텍스트 미표시)
    public void SetItemAmount(int amount)
    {
        if (HasItem && amount > 1) ShowText();
        else HideText();

        _amountText.text = amount.ToString();
    }
}
