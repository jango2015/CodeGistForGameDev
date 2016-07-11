using UnityEngine;
using System.Collections;

public class ToolTipsTrigger : MonoBehaviour
{
    void OnPress(bool isPress)
    {
        if (isPress)
            ToolTips.ShowToolTips(this.transform, "Hello Game world from" + this.name);
        else
            ToolTips.Hide();
    }

    void OnDragStart()
    {
        ToolTips.Hide();
    }
}