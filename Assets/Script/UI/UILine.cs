using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILine : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 딱 하나 UILIne 활성화인 경우에는 충돌처리할 필요 없다
        if (this.transform.parent.GetComponentsInChildren<UILine>().Length == 1)
            return;

        // 여러개인 경우(Curved Line인 경우)만 충돌 시 비활성화해주고, 한꺼번에 Destory한다
        if (other.gameObject.name == "PlayerImage")
            this.gameObject.SetActive(false);
            //Managers.Resource.Destroy(this.gameObject);
    }
}
