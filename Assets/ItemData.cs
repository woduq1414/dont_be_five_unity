using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemData
{
    public string name;
    public string imagePath;
    public string caption;

    public ItemData(string name, string caption, string imagePath)
    {
        this.name = name;
        this.imagePath = imagePath;
        this.caption = caption;
    }



    public static ItemData isolate = new ItemData(name: "isolate", caption: "자가격리", imagePath: ImagePath.isolate);
    public static ItemData release = new ItemData(name: "release", caption: "격리해제", imagePath: ImagePath.release);
    public static ItemData vaccine = new ItemData(name: "vaccine", caption: "백신", imagePath: ImagePath.vaccine);
    public static ItemData diagonal = new ItemData(name: "diagonal", caption: "대각선 이동", imagePath: ImagePath.diagonal);

    public static List<ItemData> getItemDataList()
    {
        return new List<ItemData>{ItemData.isolate, ItemData.release, ItemData.vaccine, ItemData.diagonal};
    }


}
