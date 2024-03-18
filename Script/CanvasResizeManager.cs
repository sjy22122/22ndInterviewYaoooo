using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResizeManager : MonoBehaviour
{
    /*
    float background_width;
    float background_height;
    float current_width;
    float current_height;

    public RawImage background;*/

    float Screen_Width;
    float Screen_Height;


    // Start is called before the first frame update
    void Start()
    {
        Screen_Width = this.gameObject.GetComponent<RectTransform>().rect.width;
        Screen_Height = this.gameObject.GetComponent<RectTransform>().rect.height;
        /*
        background_rect = background.GetComponent<RectTransform>().rect;
        background_rect.width = current_width;
        background_rect.height = current_height;*/

        //background.GetComponent<RectTransform>().sizeDelta = new Vector2(background_rect.width, background_rect.height);

        //how to resize rectransform
        /*
        background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, background_width);
        background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, background_height);*/
    }

    // Update is called once per frame
    void Update()
    {
        /*
        current_width = this.gameObject.GetComponent<RectTransform>().rect.width;
        current_height = this.gameObject.GetComponent<RectTransform>().rect.height;

        if (background_width != current_width || background_height != current_height)
        {
            background_width = current_width;
            background_height = current_height;
            background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, background_width);
            background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, background_height);
        }*/


        //Debug.Log("current canvas size:" + current_width + " * " + current_height);

    }
}
