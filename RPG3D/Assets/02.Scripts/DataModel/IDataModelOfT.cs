using System;
using System.Collections.Generic;

namespace RPG.DataModel
{
    /// <summary>
    /// ������ �� �������̽�
    /// </summary>
    /// <typeparam name="T">������ Ÿ��</typeparam>
    public interface IDataModel<T> : IDataModel
    {
        IEnumerable<int> itemIDs { get; }
        IEnumerable<T> items { get; }
        void RequestRead(int itemID, Action<int, T> onSuccess);
        void RequestWrite(int itemID, T item, Action<int, T> onSuccess);
    }
}