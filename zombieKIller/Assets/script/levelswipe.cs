using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PageSwiper : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public GameObject[] panels; // Assign your panels here
    public Button nextButton;
    public Button prevButton;
    public float swipeSpeed = 10f; // Speed of transition
    public float swipeThreshold = 50f; // Sensitivity of swiping

    private int currentPage = 0;
    private bool isSwiping = false;
    private Vector2 startDragPosition;

    void Start()
    {
        if (scrollRect == null || panels == null || panels.Length == 0 || nextButton == null || prevButton == null)
        {
            Debug.LogError("PageSwiper: Please ensure all public fields are assigned in the inspector.");
            enabled = false;
            return;
        }

        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);

        UpdateButtonStates();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startDragPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSwiping) return;

        float dragDistance = eventData.position.x - startDragPosition.x;

        if (Mathf.Abs(dragDistance) > swipeThreshold)
        {
            if (dragDistance < 0 && currentPage < panels.Length - 1) // Swipe left
            {
                MoveToPage(currentPage + 1);
            }
            else if (dragDistance > 0 && currentPage > 0) // Swipe right
            {
                MoveToPage(currentPage - 1);
            }
            else
            {
                MoveToPage(currentPage);
            }
        }
        else
        {
            MoveToPage(currentPage);
        }
    }

    public void NextPage()
    {
        if (currentPage < panels.Length - 1)
        {
            MoveToPage(currentPage + 1);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            MoveToPage(currentPage - 1);
        }
    }
    private void MoveToPage(int pageIndex)
    {
        if (isSwiping) return;

        currentPage = pageIndex;
        StartCoroutine(SmoothSwitch(panels[pageIndex]));
        UpdateButtonStates();
        Debug.Log(pageIndex);
    }
    private IEnumerator SmoothSwitch(GameObject targetPanel)
    {
        isSwiping = true;

        // Hide all panels
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        // Fade-in effect (optional)
        targetPanel.SetActive(true);
        CanvasGroup canvasGroup = targetPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * swipeSpeed;
                canvasGroup.alpha = Mathf.Lerp(0, 1, time);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        isSwiping = false;
    }

    private void UpdateButtonStates()
    {
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < panels.Length - 1;
    }
}