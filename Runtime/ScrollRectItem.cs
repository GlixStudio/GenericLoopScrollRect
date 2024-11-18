using System;
using UnityEngine;

namespace GlixStudio.GenericScrollRect
{
    public abstract class ScrollRectItem<T> : MonoBehaviour
    {
        [HideInInspector]
        public int ItemIndex;
        public Action<int> onClickAction;

        public abstract void UpdateData(T data);

        public void UpdateIndex(int index)
        {
            ItemIndex = index;
        }

        public virtual void RefreshUI(int index)
        {

        }

        protected void OnItemClick()
        {
            onClickAction?.Invoke(ItemIndex);
        }

        private void OnDestroy()
        {
            onClickAction = null;
        }
    }
}