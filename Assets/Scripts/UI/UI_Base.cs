using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UI_Base : MonoBehaviour
{
    // ��� UI �� �ֻ��� ���̽� Ŭ����
    // Bind �Լ���, Get �Լ���

    // ��ųʸ��� ����
    // �� Type ���� list�� ��� ����
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    private void Start()
    {
        //Init(); 
    }
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        // UI �ڵ�ȭ ��� : ����Ƽ ���� ���������ִ� ��� �ڵ�ȭ 
        // ���� ���
        
        // enum ���� ��� �Ѱܹ���?
        // C# �� reflection ��� ���
        // reflection : ������Ʈ �������� �� ���� ����
        // Type �� �̿��ؼ� �Է¹���, typeof() �̿��ؼ� ����.

        // �ϴ� ��
        // enum ���Ͽ��� �ش�Ǵ� �̸��� ã�Ƽ� ������Ʈ ã�Ƽ� ����
        // ��Ȯ���� ������Ʈ�� ��� �ִ� ������Ʈ
        // generic ���� ������Ʈ �־ ...

        // 1. C# ������� �̸� ��ȯ enum -> string ��ȯ
        string[] names = Enum.GetNames(type); 

        // 2. ������ŭ ������Ʈ ����Ʈ ����
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        
        // 3. Ÿ�Ժ��� ����Ʈ�� ��ųʸ��� �ֱ�
        _objects.Add(typeof(T), objects);

        // 4. �ڵ����� �ֱ� (�̸��ϰ� ���� ������Ʈ ã�Ƽ�)
        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject)) // ���� ������Ʈ��
                objects[i] = Util.FindChild(gameObject, names[i], true); // GameObject ���� ���� FindChild
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true); // ������Ʈ ���� FindChild

            // ���ε� ����
            if (objects[i] == null)
                Debug.Log($"Failed to Bind :  {names[i]}");
        }


    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        // Get �Լ� : ��ųʸ����� ������ ���� �Լ�

        // 1. ������ ����Ʈ ����
        UnityEngine.Object[] objects = null;

        // 2. ��ųʸ����� ã�Ƽ� ����Ʈ ����
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        // 3. T�� casting �ؼ� ��ȯ
        return objects[idx] as T;
    }

    // Get �ٸ� ������ : GetObject, GetText, GetButton, GetImage
    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        // ���� ������Ʈ �޾Ƽ�, UI_EventHandler ������Ʈ �����ϰ�, ���� �߰��ϴ� ���
        // �Է� : ���� ������Ʈ , �ݹ����� ���� ������ �Լ� action���� ���� , UI_Event ����

        // 1. UI_EventHandler �������� (������ �߰�)
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        // 2. type�� ���� ����
        switch(type)
        {
            // Click type�̸�, �ش� action �� OnClickHandler ������û
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;

                // Drag type�̸�, �ش� action �� OnDragHandler ������û
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
            case Define.UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case Define.UIEvent.PointerEnter:
                evt.OnPointerEnterHandler -= action;
                evt.OnPointerEnterHandler += action;
                break;
            case Define.UIEvent.PointerExit:
                evt.OnPointerExitHandler -= action;
                evt.OnPointerExitHandler += action;
                break;
        }
    }

}
