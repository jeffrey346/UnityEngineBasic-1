using System;
using System.Collections;

namespace RPG.DataModel
{
    /// <summary>
    /// ������ �� �������̽�
    /// </summary>
    /// <typeparam name="T">������ Ÿ��</typeparam>
    public interface IDataModel
    {
        IEnumerable itemIDs { get; }
        IEnumerable items { get; }
        void RequestRead(int itemID, Action<int, object> onSuccess);
        void RequestWrite(int itemID, object item, Action<int, object> onSuccess);

        void SetDefaultItems();
    }
}