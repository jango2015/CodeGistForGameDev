using UnityEngine;
using System.Collections;

public class LongPressItemLogic : MonoBehaviour
{
    private float mLongClickDuration = 1.5f;
    private float mLastPressTime = 0.0f;
    private bool mIsStartPress = false;
    private bool mHasNotify = false;

    public void UpdateData(string itemName)
    {
    }

    // long press to select the item.
    private void OnItemLongPress()
    {
        LoopManager.GetInstance.OnItemLongPress();
    }

    void OnPress(bool isPress)
    {
        if (isPress)
        {
            this.mLastPressTime = Time.realtimeSinceStartup;
            this.mIsStartPress = true;
        }
        else
            this.mIsStartPress = false;
        this.mHasNotify = false;
    }

    void OnDrag()
    {
        this.mIsStartPress = false;
        this.mHasNotify = false;
    }

    void Update()
    {
        if (this.mIsStartPress)
        {
            if (Time.realtimeSinceStartup - this.mLastPressTime > this.mLongClickDuration
                && !this.mHasNotify)
            {
                this.mHasNotify = true;
                this.OnItemLongPress();
            }
        }
    }
}
