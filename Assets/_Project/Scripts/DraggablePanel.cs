using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 offset;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint
            );

            offset = localPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPoint))
            {
                // Вычисляем новую позицию
                Vector3 newPos = localPoint - offset;

                // Ограничиваем позицию в пределах Canvas
                ClampToCanvas(ref newPos);

                rectTransform.localPosition = newPos;
            }
        }

        public void ClampToCanvas(ref Vector3 pos)
        {
            RectTransform canvasRect = canvas.transform as RectTransform;

            Vector3[] canvasCorners = new Vector3[4];
            canvasRect.GetLocalCorners(canvasCorners);

            Vector3[] panelCorners = new Vector3[4];
            rectTransform.GetLocalCorners(panelCorners);

            float panelWidth = panelCorners[2].x - panelCorners[0].x;
            float panelHeight = panelCorners[2].y - panelCorners[0].y;

            float minX = canvasCorners[0].x + panelWidth / 2f;
            float maxX = canvasCorners[2].x - panelWidth / 2f;
            float minY = canvasCorners[0].y + panelHeight / 2f;
            float maxY = canvasCorners[2].y - panelHeight / 2f;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }
    }
}