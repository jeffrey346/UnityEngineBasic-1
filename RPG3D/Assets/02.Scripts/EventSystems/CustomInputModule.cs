using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.EventSystems
{ 
    public class CustomInputModule : StandaloneInputModule
    {
        public static CustomInputModule main;

        protected override void Awake()
        {
            base.Awake();

            if (main != null)
               main = this;
            
        }
        // � Raycaster �� ���� Casting �� ��� GameObject �� ��ȯ�ϴ� �Լ�
        public bool TryGetHovered<T>(out IEnumerable<GameObject> hovered, int mouseID = kMouseLeftId)
            where T : BaseRaycaster
        {
            // Ư�� mouseID �� ���� ���콺 �̺�Ʈ �����͸� ����
            if (m_PointerData.TryGetValue(mouseID, out PointerEventData pointerEventData))
            {
                BaseRaycaster module = pointerEventData.pointerCurrentRaycast.module;
                if (module != null && 
                    module is T)
                {
                    hovered = pointerEventData.hovered;
                    return true;
                }
            }
            hovered = null;
            return false;
        }


        // � Raycaster �� ���� Casting �� GameObject �� �߿�
        // Ư�� Component �� ã�� �Լ�
        public bool TryGetHovered<T, K>(out IEnumerable<GameObject> hovered, int mouseID = kMouseLeftId)
            where T : BaseRaycaster
        {
            // Ư�� mouseID �� ���� ���콺 �̺�Ʈ �����͸� ����
            if (m_PointerData.TryGetValue(mouseID, out PointerEventData pointerEventData))
            {
                BaseRaycaster module = pointerEventData.pointerCurrentRaycast.module;
                if (module != null &&
                    module is T)
                {
                    foreach (var item in pointerEventData.hovered)
                    {
                        if (item.TryGetComponent(out hovered))
                            return true;
                    }
                }
            }
            hovered = default;
            return false;
        }
    }
}