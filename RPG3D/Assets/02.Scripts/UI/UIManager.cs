using RPG.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public LinkedList<IUI> showns = new LinkedList<IUI>();
        public Dictionary<Type, IUI> _dictionary = new Dictionary<Type, IUI>();

        public void Register(IUI ui)
        {
            Type type = ui.GetType();

            if (_dictionary.ContainsKey(type) == false)
            {
                _dictionary.Add(type, ui);
                Debug.Log($"[UIManager] : Registered {ui.GetType()}");
            }
        }

        /// <summary>
        /// UI ���� ��������
        /// </summary>
        /// <typeparam name="T"> �ʿ��� UI Ÿ��</typeparam>
        /// <returns> UI </returns>

        public T Get<T>()
             where T : IUI
        {
           return (T)_dictionary[typeof(T)];
        }

        public void Push(IUI ui)
        {
            //�̹� �� UI�� ���� �ڿ� �ö������
            if (showns.Count > 0 &&
                showns.Last.Value == ui)
                return;

            // ���� �ڿ� �ִ� UI ���� �ڷ�
            int sortOrder = 0;
            if (showns.Last != null)
            {
                sortOrder = showns.Last.Value.sortOrder + 1;
                showns.Last.Value.inputActionEnabled = false;
            }
            ui.sortOrder = sortOrder;
            ui.inputActionEnabled = true;
            showns.Remove(ui); // �߰��� UI ������ ��
            showns.AddLast(ui); // UI �� �ڷ� ����
                                
            if (showns.Count == 1)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        public void Pop(IUI ui)
        {
            // ������ UI �� ���� �ڿ� �־���,
            // �� �տ� ���� UI�� �ִٸ�, �ش� UI �� �����Է�ó�� ���
            if (showns.Count > 1 &&
                showns.Last.Value == ui)
            {
                showns.Last.Previous.Value.inputActionEnabled = true;
            } 

            showns.Remove(ui);
                        
            if(showns.Count == 0)
            {
                Cursor.visible = false;
                Cursor.lockState= CursorLockMode.Locked;
            }
        }

        public void HideLast()
        {
            if (showns.Count <= 0)
                return;

            showns.Last.Value.Hide();
        }

    }
}