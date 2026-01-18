using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLoader : MonoBehaviour
{
    [SerializeField] GameObject loading;
    [SerializeField] TMP_Text text;
    [Tooltip("Objects to hide during loading")]
    [SerializeField] List<GameObject> loadHideObjects;
    [SerializeField] GameObject disableShowObject;

    private string defaultText = "Button";
    [SerializeField] string loadingText = "Loading...";
    [SerializeField] string disabledText = "Done!";

    [HideInInspector] public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        defaultText = text.text;
        SetLoading(false);
    }

    private void OnEnable()
    {
        SetLoading(false);
    }

    public void SetLoading(bool isLoading)
    {
        if (button != null)
            button.interactable = !isLoading;

        loading.SetActive(isLoading);
        foreach (var obj in loadHideObjects)
        {
            obj.SetActive(!isLoading);
        }

        text.text = isLoading ? loadingText : defaultText;
    }

    public void SetInteractable(bool isInteractable)
    {
        if (button != null)
            button.interactable = isInteractable;

        text.text = isInteractable ? defaultText : disabledText;
        if (disableShowObject.gameObject != null)
            disableShowObject.SetActive(!isInteractable);
    }
}
