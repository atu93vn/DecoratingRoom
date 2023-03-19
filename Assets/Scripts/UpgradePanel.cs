using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public HouseManager houseManager;

    public Text nameFurnitureText;
    public Transform textureBtnsTr, colorBtnsTr;
    public TextureFurniture[] textureFurnitures;

    public int furnitureIndex;
    
    public void UpdateUi(int i)
    {
        furnitureIndex = i;
        gameObject.SetActive(true);

        if (i == 0)
            nameFurnitureText.text = "Ceil";
        else if (i == 1)
            nameFurnitureText.text = "Desk";
        else if (i == 2)
            nameFurnitureText.text = "Floor";
        else if (i == 3)
            nameFurnitureText.text = "Wall";
        else 
            nameFurnitureText.text = "Window";

        for (int j = 0; j < textureBtnsTr.childCount; j++)
        {
            textureBtnsTr.GetChild(j).GetComponent<Image>().sprite = textureFurnitures[i].textures[j];
            textureBtnsTr.GetChild(j).GetChild(1).gameObject.SetActive(j == PlayerPrefs.GetInt("Furniture" + i));
        }

        for (int j = 0; j < colorBtnsTr.childCount; j++)
        {
            colorBtnsTr.GetChild(j).GetChild(1).gameObject.SetActive(j == PlayerPrefs.GetInt("Furniture_Color" + i));
        }
    }

    public void SetTexture(int i)
    {
        PlayerPrefs.SetInt("Furniture" + furnitureIndex, i);
        houseManager.UpdateFurniture(furnitureIndex);
        UpdateUi(furnitureIndex);
    }

    public void SetColor(int i)
    {
        PlayerPrefs.SetInt("Furniture_Color" + furnitureIndex, i);
        houseManager.UpdateFurniture(furnitureIndex);
        UpdateUi(furnitureIndex);
    }
}

[System.Serializable]
public class TextureFurniture
{
    public Sprite[] textures;
}