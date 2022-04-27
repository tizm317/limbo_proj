using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    // 기능성 함수 목록

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        // 컴포넌트를 있으면 추가, 없으면 생성하는 기능

        T component = go.GetComponent<T>();

        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }


    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        // FindChild - 게임오브젝트 전용 버전
        // 모든 게임오브젝트는 transform 컴포넌트 지님.

        // 1. transform 컴포넌트로 찾기
        Transform transform = FindChild<Transform>(go, name, recursive);

        if (transform == null)
            return null;

        // 2. transform의 게임오브젝트 반환
        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        // FindChild - (컴포넌트로 찾는 버전)

        // 하는 일 : 최상위부모 밑 오브젝트들 중에서 T 가진 오브젝트 찾는 기능
        // 입력 : 최상위부모 , 이름 , 재귀적으로 찾을지(자식들까지)
        // T : 찾고 싶은 컴포넌트 / 제한사항 : 유니티 오브젝트만
        // 이름 입력 안하면 T 타입 찾으면 반환

        if (go == null)
            return null;

        if (recursive == false)
        {
            // 1) recursive하지 않은 경우
            // 직속 자식만 찾기

            for (int i = 0; i < go.transform.childCount; i++)
            {
                // 1. 자식 순회
                Transform transform = go.transform.GetChild(i);

                // 2. 이름 같은지 또는 이름 입력 안했는지 확인
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    // 3. 컴포넌트 들고 있는 지 확인
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            // 2) recursive한 경우

            // GetComponetInChildren
            // 1. T 타입의 컴포넌트 가지고 있는 게임오브젝트 순회
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                // 2. 이름 입력 안하면 이름 상관없이 T 타입 찾으면 반환 , 이름 입력 시 이름 같으면 반환
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}
