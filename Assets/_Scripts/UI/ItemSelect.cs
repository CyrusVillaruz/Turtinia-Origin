using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{
    public GameObject itemSlot1;
    public GameObject itemSlot2;

    public GameObject weapon1;
    public GameObject weapon2;

    public float switchCooldown;

    private bool isSelected = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Transform weapon1Child = weapon1.transform.GetChild(0);
            weapon1Child.rotation = Quaternion.Euler(weapon1Child.rotation.eulerAngles.x, weapon1Child.rotation.eulerAngles.y, 0);
            weapon1Child.gameObject.SetActive(false);

            isSelected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isSelected = false;
        }

        UpdateItemSlotColors();
    }

    private void SelectItem(bool isSelected)
    {
        weapon1.SetActive(isSelected);
        weapon2.SetActive(!isSelected);
    }

    private void UpdateItemSlotColors()
    {
        Color selected = ColorUtility.TryParseHtmlString("#9ACD32", out Color green) ? green : Color.green;
        Color notSelected = ColorUtility.TryParseHtmlString("#808080", out Color gray) ? gray : Color.gray;

        if (isSelected)
        {
            Transform slot1Bg = itemSlot1.transform.Find("Background");
            Transform slot2Bg = itemSlot2.transform.Find("Background");
            slot1Bg.GetComponent<Image>().color = selected;
            slot2Bg.GetComponent<Image>().color = notSelected;
        }
        else
        {
            Transform slot1Bg = itemSlot1.transform.Find("Background");
            Transform slot2Bg = itemSlot2.transform.Find("Background");
            slot1Bg.GetComponent<Image>().color = notSelected;
            slot2Bg.GetComponent<Image>().color = selected;
        }

        SelectItem(isSelected);
    }

}
