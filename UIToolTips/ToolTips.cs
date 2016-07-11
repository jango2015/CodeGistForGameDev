using UnityEngine;
using System.Collections;

public class ToolTips : MonoBehaviour
{
    public Transform trsTips;
    // Tips显示范围.
    // the region to show the tips
    public UIPanel panelRegion;
    // Tips边界.
    // 该参数一般为Bounds，Prefab中的内容
    // 都设置Anchor对象为Bounds
    // 遇到存在Label可变
    // Widget
    //      LabelContent
    // 重新计算Bounds的Height即可，最终检测和位置计算都是通过Bounds完成
    // bounds for position check and final position calculated.
    public UIWidget contentRect;

    // 测试显示的内容
    // label content
    public UILabel lbContent;

    // get the right tips world position.
    private Transform trsFlagParent = null;
    public Transform trsFlag = null;

    private Transform mTrs;
    private Vector3 mPos;
    private Vector3 mCheckPos;
    private Bounds mBounds;

    // 上一个Tips显示对象.
    // last tips target
    private Transform mLastTipsTarget = null;

    // 检测percent
    // 根据当前实际的显示进行修改，检测位置偏移量
    // 一般直接设置成0.8，0.8，0.8即可
    public float fCheckTopPercent = 0.5f;
    public float fCheckLeftPercent = 0.5f;
    public float fCheckBottomPercent = 0.5f;

    // 定义实际Tips显示偏移量，目前是固定偏移量
    // 点按区域被遮挡可以调整该参数
    // define the final position with offset percent
    //public float fFinaTopPercent = 0.5f;
    //public float fFinalLeftPercent = 0.5f;
    //public float fFinalBottomPercent = 0.5f;

    private static ToolTips instance;
    public static ToolTips Instance
    {
        get { return instance; }
    }

    private enum TipsDir
    {
        Top,
        Left,
        Bottom,
    }

    void Awake()
    {
        instance = this;
        trsFlagParent = this.trsFlag.parent;
    }

    void Start()
    {
        mTrs = trsTips;
    }

    void Update()
    {
        if (mLastTipsTarget == null)
            return;
        if (mLastTipsTarget != UICamera.hoveredObject.transform)
            this.HideToolTips();
    }

    public static void ShowToolTips(Transform targetPos, string strText)
    {
        if (instance != null)
            instance.SetToolTipsContent(targetPos, strText);
    }

    public static void Hide()
    {
        if (instance != null)
            instance.HideToolTips();
    }

    private void HideToolTips()
    {
        // 直接将alpha设置为0.01f即可
        // set the widget alpha to 0.001f
        this.gameObject.SetActive(false);
        mLastTipsTarget = null;
    }

    private void SetToolTipsContent(Transform targetTrs, string strText)
    {
        if (UICamera.hoveredObject.transform != targetTrs)
            return;

        // for multi touch hide pre tooltip.
        this.HideToolTips();
        // 显示内容，自适应
        // 拉伸Bounds边界，根据当前label内容修改bounds的height
        // recalculate the bounds height by the label content
        if (this.lbContent.text.Length <= 150)
            this.lbContent.text = this.lbContent.text + strText;
        this.contentRect.height = this.lbContent.height + 80;
        // Debug.Log("! tool tips content:" + this.lbContent.text);

        // check tips fit screen direction.
        UIWidget rangeWidget = targetTrs.GetComponent<UIWidget>();
        mCheckPos = Vector3.zero;

        // get fit tips position.
        bool isFit = this.CheckTipsDirection(targetTrs, rangeWidget, TipsDir.Top);
        if (!isFit)
            isFit = this.CheckTipsDirection(targetTrs, rangeWidget, TipsDir.Left);
        if (!isFit)
            this.CheckTipsDirection(targetTrs, rangeWidget, TipsDir.Bottom);

        trsFlag.parent = trsFlagParent;
        trsFlag.position = mPos;

        mPos = trsFlag.localPosition;
        mPos.x = Mathf.Round(mPos.x);
        mPos.y = Mathf.Round(mPos.y);
        trsFlag.localPosition = mPos;

        mTrs.position = trsFlag.position;
        mPos = mTrs.localPosition;
        mPos.x = Mathf.Round(mPos.x);
        mPos.y = Mathf.Round(mPos.y);
        mTrs.localPosition = mPos;

        this.UpdateBounds();
        panelRegion.ConstrainTargetToBounds(trsTips, ref mBounds, true);

        // show the tips.
        mLastTipsTarget = targetTrs;
        this.gameObject.SetActive(true);
    }

    private bool CheckTipsDirection(Transform targetTrs, UIWidget widget, TipsDir dir)
    {
        bool isFit = false;
        if (dir == TipsDir.Top)
        {
            // mCheckPos = targetTrs.localPosition + new Vector3(0f, widget.height * 0.5f + contentRect.height * 0.5f, 0f);
            mCheckPos = targetTrs.localPosition
                + new Vector3(0f, widget.height * 0.5f + contentRect.height * fCheckTopPercent, 0f);
        }
        else if (dir == TipsDir.Left)
        {
            // mCheckPos = targetTrs.localPosition - new Vector3(widget.width * 0.5f + contentRect.width * 0.5f, 0f, 0f);
            mCheckPos = targetTrs.localPosition
                - new Vector3(widget.width * 0.5f + contentRect.width * fCheckLeftPercent, 0f, 0f);
        }
        else
        {
            // final tip position.
            // mCheckPos = targetTrs.localPosition - new Vector3(0f, widget.height * 0.5f + contentRect.height * 0.5f, 0f);
            mCheckPos = targetTrs.localPosition
                - new Vector3(0f, widget.height * 0.5f + contentRect.height * fCheckBottomPercent, 0f);
        }

        trsFlag.parent = targetTrs.parent;
        trsFlag.localPosition = mCheckPos;
        mCheckPos = mPos = trsFlag.position;

        if (dir == TipsDir.Bottom)
            isFit = true;
        if (dir == TipsDir.Top)
            mCheckPos.x = 0f;
        isFit = panelRegion.IsVisible(mCheckPos);

        if (isFit)
        {
            // you can set the final position with offset percent
            // fFinaTopPercent and so on
            Vector3 finalPos = this.mCheckPos;
            if (dir == TipsDir.Top)
                finalPos = targetTrs.localPosition + new Vector3(0, widget.height * 0.5f + contentRect.height * 0.5f, 0f);
            else if (dir == TipsDir.Left)
                finalPos = targetTrs.localPosition - new Vector3(widget.width * 0.5f + contentRect.width * 0.7f, 0f);
            else
                finalPos = targetTrs.localPosition - new Vector3(0f, widget.height * 0.5f + contentRect.height * 0.5f, 0f);

            trsFlag.parent = targetTrs.parent;
            trsFlag.localPosition = finalPos;
            mPos = trsFlag.position;
        }

        return isFit;
    }

    private void UpdateBounds()
    {
        if (contentRect)
        {
            Transform t = panelRegion.cachedTransform;
            Matrix4x4 toLocal = t.worldToLocalMatrix;
            Vector3[] corners = contentRect.worldCorners;
            for (int i = 0; i < 4; i++)
                corners[i] = toLocal.MultiplyPoint3x4(corners[i]);
            mBounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < 4; i++)
                mBounds.Encapsulate(corners[i]);
        }
        else
            mBounds = NGUIMath.CalculateRelativeWidgetBounds(panelRegion.cachedTransform, trsTips);
    }

    #region Test for mouse input.
    /// <summary>
    /// 鼠标移动Tips.
    /// </summary>
    private void MouseMoveTips()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        /*if (Time.time - this.mLastTime >= updateRate)
        {
            this.mLastTime = Time.time;
            // set the mouse position.
            mPos = Input.mousePosition;
            mPos.x = Mathf.Clamp01(mPos.x / Screen.width);
            mPos.y = Mathf.Clamp01(mPos.y / Screen.height);
            // Debug.Log("@mouse position:" + mPos.x + "  " + mPos.y);
            Vector3 worldPosition = uiCamera.ViewportToWorldPoint(mPos);
            mTrs.position = worldPosition;
            mPos = mTrs.localPosition;
            mPos.x = Mathf.Round(mPos.x);
            mPos.y = Mathf.Round(mPos.y);
            mTrs.localPosition = mPos;

            // Debug.Log("@local position:" + mPos.x + "  " + mPos.y);
            this.UpdateBounds();
            panelRegion.ConstrainTargetToBounds(trsToolTips, ref mBounds, true);
        }*/
    }
    #endregion

}
