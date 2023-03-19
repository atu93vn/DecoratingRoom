using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class HouseManager : MonoBehaviour
{
    public UpgradePanel upgradePanel;

    public GameObject[] items;
    public Transform scenceObjectTr;
    public Transform colorBtns;
    public Furniture[] furnitures;

    public Color[] colors;

    private void Start()
    {
        for (int i = 0; i < colorBtns.childCount; i++)
            colorBtns.GetChild(i).GetComponent<Image>().color = colors[i];
        UpdateHouse();
    }

    public void UpdateHouse()
    {
        for (int i = 0; i < furnitures.Length; i++)
            UpdateFurniture(i);
    }

    public void UpdateFurniture(int i)
    {
        if (items[i] != null) Destroy(items[i]);
        items[i] = Instantiate(furnitures[i].items[PlayerPrefs.GetInt("Furniture" + i)], scenceObjectTr);
        items[i].GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", colors[PlayerPrefs.GetInt("Furniture_Color" + i)]);
    }

    private void Update()
    {
        if (CUtils.IsPointerOverUIObject()) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 1000);

            for (int i = 0; i < hits.Length - 1; i++)
                for (int j = i + 1; j < hits.Length; j++)
                    if (hits[i].distance < hits[j].distance)
                    {
                        RaycastHit tmp = hits[j];
                        hits[j] = hits[i];
                        hits[i] = tmp;
                    }

            for (int i = hits.Length - 1; i >= 0;)
            {
                if (hits[i].collider.CompareTag("wall"))
                    upgradePanel.UpdateUi(3);
                else if (hits[i].collider.CompareTag("floor"))
                    upgradePanel.UpdateUi(2);
                else if (hits[i].collider.CompareTag("ceil"))
                    upgradePanel.UpdateUi(0);
                else if (hits[i].collider.CompareTag("desk"))
                    upgradePanel.UpdateUi(1);
                else if (hits[i].collider.CompareTag("window"))
                    upgradePanel.UpdateUi(4);
                break;
            }
        }
    }
}

[System.Serializable]
public class Furniture
{
    public List<GameObject> items;
}