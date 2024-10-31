using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button btn;
    [SerializeField] private bool isActive;

    [Space(10)]

    [SerializeField] private bool isBG = true;
    [SerializeField] private Image image;
    [SerializeField] private Sprite sprite_On;
    [SerializeField] private Sprite sprite_Off;

    [Space(10)]

    [SerializeField] private bool isIcon = true;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite icon_On;
    [SerializeField] private Sprite icon_Off;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (isBG)
        {
            image = GetComponent<Image>();
        }

        if (isIcon)
        {
            icon = transform.Find("Image").GetComponent<Image>();
        }
    }

    public void ButtonOn()
    {
        isActive = true;
        SetBGImage(true);
        SetIconImage(true);
    }

    public void ButtonOff()
    {
        isActive = false;
        SetBGImage(false);
        SetIconImage(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetBGImage(true);
        SetIconImage(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isActive)
        {
            SetBGImage(true);
            SetIconImage(true);
        }
        else
        {
            SetBGImage(false);
            SetIconImage(false);
        }
    }

    private void SetBGImage(bool isOn)
    {
        if (isBG)
        {
            if (image != null)
            {
                if (isOn)
                {
                    image.sprite = sprite_On;
                }
                else
                {
                    image.sprite = sprite_Off;
                }
            }
        }
    }

    private void SetIconImage(bool isOn)
    {
        if (isIcon)
        {
            if (icon != null)
            {
                if (isOn)
                {
                    icon.sprite = icon_On;
                }
                else
                {
                    icon.sprite = icon_Off;
                }
            }
        }
    }
}
