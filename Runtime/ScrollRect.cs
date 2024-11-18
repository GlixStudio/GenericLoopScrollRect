using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GlixStudio.GenericScrollRect
{
    public abstract class ScrollRect<T> : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
    {
        public abstract List<T> ItemDataList { get; }
        [SerializeField] protected LoopScrollRect loopScrollRect;
        [SerializeField] protected ScrollRectItem<T> scrollRectItem;

        private readonly float _scrollSensitivity = 25f;
        private readonly LoopScrollRectBase.MovementType _movementType = LoopScrollRectBase.MovementType.Clamped;

        protected readonly Stack<Transform> _itemPool = new Stack<Transform>();
        protected readonly Dictionary<Transform, ScrollRectItem<T>> _itemTransformToItemMap = new Dictionary<Transform, ScrollRectItem<T>>();

        protected virtual void Start()
        {
            if (loopScrollRect.viewport != null)
            {
                loopScrollRect.viewport.anchorMax = Vector2.one;
                loopScrollRect.viewport.anchorMin = Vector2.zero;
                loopScrollRect.viewport.pivot = new Vector2(0, 1);

                loopScrollRect.Rebuild(CanvasUpdate.Prelayout);
            }

            loopScrollRect.prefabSource = this;
            loopScrollRect.dataSource = this;
            loopScrollRect.totalCount = ItemDataList.Count;
            loopScrollRect.movementType = _movementType;
            loopScrollRect.scrollSensitivity = _scrollSensitivity;
            loopScrollRect.RefillCells();
        }

        protected virtual void OnItemCreated(ScrollRectItem<T> item)
        {
            
        }

        protected abstract void OnItemClick(int index);

        public virtual GameObject GetObject(int index)
        {
            if (_itemPool.Count == 0)
            {
                ScrollRectItem<T> newItem = Instantiate(scrollRectItem);
                newItem.onClickAction = OnItemClick;
                OnItemCreated(newItem);

                _itemTransformToItemMap.Add(newItem.transform, newItem);
                return newItem.gameObject;
            }

            Transform candidate = _itemPool.Pop();
            candidate.gameObject.SetActive(true);
            return candidate.gameObject;
        }

        public void ReturnObject(Transform itemTransform)
        {
            itemTransform.gameObject.SetActive(false);
            itemTransform.SetParent(transform, false);
            _itemPool.Push(itemTransform);
        }

        public virtual void ProvideData(Transform itemTransform, int index)
        {
            T itemData = ItemDataList[index];

            if (_itemTransformToItemMap.TryGetValue(itemTransform, out ScrollRectItem<T> item))
            {
                item.UpdateIndex(index);
                item.UpdateData(itemData);
            }
        }
    }
}