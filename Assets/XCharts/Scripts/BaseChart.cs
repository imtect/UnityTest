﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace xcharts {
    [System.Serializable]
    public enum ChartType {
        line,
        bar
    }

    [System.Serializable]
    public class Title {
        public bool show = true;
        public string text = "Chart Title";
        public string subText = "";
        public Align align = Align.center;
        public float left;
        public float right;
        public float top = 5;
        public float bottom;
    }

    [System.Serializable]
    public enum Align {
        left,                       //左对齐
        right,                      //右对齐
        center                      //居中对齐
    }

    [System.Serializable]
    public enum Location {
        left,
        right,
        top,
        bottom,
        start,
        middle,
        center,
        end,
    }

    [System.Serializable]
    public class Legend {
        public bool show = true;
        public Location location = Location.right;
        public float itemWidth = 50.0f;
        public float itemHeight = 20.0f;
        public float itemGap = 5;
        public float left;
        public float right = 5;
        public float top;
        public float bottom;
        public List<string> dataList = new List<string>();
        public int checkDataListCount { get; set; }

        private List<bool> dataShowList = new List<bool>();
        private List<Button> dataBtnList = new List<Button>();

        public bool IsShowSeries(int seriesIndex) {
            if (seriesIndex < 0 || seriesIndex > dataList.Count - 1) seriesIndex = 0;
            if (seriesIndex >= dataList.Count) return false;
            if (seriesIndex < 0 || seriesIndex > dataShowList.Count - 1) {
                return true;
            } else {
                return dataShowList[seriesIndex];
            }
        }

        public void SetShowData(int index, bool flag) {
            dataShowList[index] = flag;
        }

        public void SetDataButton(int index, Button btn) {
            if (index < 0 || index > dataBtnList.Count - 1) {
                dataBtnList.Add(btn);
                dataShowList.Add(true);
            } else {
                dataBtnList[index] = btn;
            }
        }

        public void SetShowData(string name, bool flag) {
            for (int i = 0; i < dataList.Count; i++) {
                if (dataList[i].Equals(name)) {
                    dataShowList[i] = flag;
                    break;
                }
            }
        }

        public Button GetButton(int index) {
            return dataBtnList[index];
        }
    }

    [System.Serializable]
    public class Tooltip {
        public bool show;

        public int DataIndex { get; set; }
        public int LastDataIndex { get; set; }
        public float Width { get { return bgRect.sizeDelta.x; } }
        public float Height { get { return bgRect.sizeDelta.y; } }


        private GameObject gameObject;
        private Text text;
        private RectTransform bgRect;

        public void SetObj(GameObject obj) {
            gameObject = obj;
            bgRect = gameObject.GetComponent<RectTransform>();
            text = gameObject.GetComponentInChildren<Text>();
        }

        public void SetBackgroundColor(Color color) {
            gameObject.GetComponent<Image>().color = color;
        }

        public void SetTextColor(Color color) {
            text.color = color;
        }

        public void UpdateTooltipText(string txt) {
            text.text = txt;
            bgRect.sizeDelta = new Vector2(text.preferredWidth + 8, text.preferredHeight + 8);
        }

        public void SetActive(bool flag) {
            gameObject.SetActive(flag);
        }

        public void UpdatePos(Vector2 pos) {
            gameObject.transform.localPosition = pos;
        }

        public Vector3 GetPos() {
            return gameObject.transform.localPosition;
        }
    }

    [System.Serializable]
    public class Series {
        public string name;
        public int showDataNumber = 0;
        [SerializeField]
        private List<float> dataList = new List<float>();
        private List<float> multiDataList = new List<float>();

        public List<float> DataList {
            get { return multiDataList.Count > 0 ? multiDataList : dataList; }
        }

        public float Max {
            get {
                float max = 0;
                foreach (var data in DataList) {
                    if (data > max) {
                        max = data;
                    }
                }
                return max;
            }
        }

        public float Min {
            get {
                float min = 0;
                foreach (var data in DataList) {
                    if (data < min) {
                        min = data;
                    }
                }
                return min;
            }
        }

        public float Total {
            get {
                float total = 0;
                foreach (var data in DataList) {
                    total += data;
                }
                return total;
            }
        }

        public void AddData(float value) {
            if (dataList.Count >= showDataNumber && showDataNumber != 0) {
                dataList.RemoveAt(0);
            }
            dataList.Add(value);
        }

        public void AddMultiData(float value) {
            if (multiDataList.Count >= showDataNumber && showDataNumber != 0) {
                multiDataList.RemoveAt(0);
            }
            multiDataList.Add(value);
        }

        public float GetData(int index) {
            if (index >= 0 && index <= DataList.Count - 1) {
                return DataList[index];
            }
            return 0;
        }

        public void UpdateData(int index, float value) {
            if (index >= 0 && index <= dataList.Count - 1) {
                dataList[index] = value;
            }
        }

        public void UpdateMultiData(int index, float value) {
            if (index >= 0 && index <= multiDataList.Count - 1) {
                multiDataList[index] = value;
            }
        }

        public void ClearData() {
            dataList.Clear();
        }

        public void ClearMultiData() {
            multiDataList.Clear();
        }
    }

    public class SeriesData {
        public string code;
        public string label;
        public string xLabel;
        public float[] value;
    }

    public class BaseChart : MaskableGraphic,UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler{
        private const string TILTE_TEXT = "title";
        private const string SUB_TILTE_TEXT = "sub_title";
        private const string LEGEND_TEXT = "legend";
        [SerializeField]
        protected bool ResetTheme = true;
        [SerializeField]
        protected Theme theme = Theme.Dark;
        [SerializeField]
        protected ThemeInfo themeInfo = new ThemeInfo();
        [SerializeField]
        protected Title title = new Title();
        [SerializeField]
        protected Legend legend = new Legend();
        [SerializeField]
        protected Tooltip tooltip = new Tooltip();
        [SerializeField]
        protected List<Series> seriesList = new List<Series>();

        private Theme checkTheme = 0;
        private Title checkTitle = new Title();
        private Legend checkLegend = new Legend();
        private float checkWid = 0;
        private float checkHig = 0;

        protected List<Text> legendTextList = new List<Text>();
        protected float chartWid { get { return rectTransform == null ? 0 : rectTransform.sizeDelta.x; } }
        protected float chartHig { get { return rectTransform == null ? 0 : rectTransform.sizeDelta.y; } }

        public bool checkTooltip;
        public System.Action<string> OnEnter;
        public System.Action<string> OnExit;

        protected override void Awake() {
            if (ResetTheme)
                themeInfo = ThemeInfo.Dark;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            checkWid = chartWid;
            checkHig = chartHig;
            checkTheme = theme;
            InitTitle();
            InitLegend();
            InitTooltip();
        }

        protected virtual void Update() {
            CheckSize();
            CheckTheme();
            CheckTile();
            CheckLegend();
            CheckTooltip();
        }

        protected override void OnDestroy() {
            for (int i = transform.childCount - 1; i >= 0; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public void AddData(string legend, float value) {
            for (int i = 0; i < seriesList.Count; i++) {
                if (seriesList[i].name.Equals(legend)) {
                    seriesList[i].AddData(value);
                    break;
                }
            }
            RefreshChart();
        }

        public void AddMultiData(string legend, float value) {
            for (int i = 0; i < seriesList.Count; i++) {
                if (seriesList[i].name.Equals(legend)) {
                    seriesList[i].AddMultiData(value);
                    break;
                }
            }
            RefreshChart();
        }

        public void AddData(int legend, float value) {
            seriesList[legend].AddData(value);
        }

        public void AddMultiData(int legend, float value) {
            seriesList[legend].AddMultiData(value);
        }

        public void UpdateData(string legend, float value, int dataIndex = 0) {
            for (int i = 0; i < seriesList.Count; i++) {
                if (seriesList[i].name.Equals(legend)) {
                    seriesList[i].UpdateData(dataIndex, value);
                    break;
                }
            }
            RefreshChart();
        }

        public void UpdateData(int legendIndex, float value, int dataIndex = 0) {
            for (int i = 0; i < seriesList.Count; i++) {
                if (i == legendIndex) {
                    seriesList[i].UpdateData(dataIndex, value);
                    break;
                }
            }
            RefreshChart();
        }

        public void UpdateMultiData(string legend, float value, int dataIndex = 0) {
            for (int i = 0; i < seriesList.Count; i++) {
                if (seriesList[i].name.Equals(legend)) {
                    seriesList[i].UpdateMultiData(dataIndex, value);
                    break;
                }
            }
            RefreshChart();
        }

        public void UpdateMultiData(int legendIndex, float value, int dataIndex = 0) {
            for (int i = 0; i < seriesList.Count; i++) {
                if (i == legendIndex) {
                    seriesList[i].UpdateMultiData(dataIndex, value);
                    break;
                }
            }
            RefreshChart();
        }

        public void ClearData() {
            for (int i = 0; i < seriesList.Count; i++) {
                seriesList[i].ClearData();
            }
            RefreshChart();
        }

        public void ClearMultiData() {
            for (int i = 0; i < seriesList.Count; i++) {
                seriesList[i].ClearMultiData();
            }
            RefreshChart();
        }

        public void UpdateTheme(Theme theme) {
            this.theme = theme;
            OnThemeChanged();
            SetAllDirty();
        }

        protected void HideChild(string match = null) {
            for (int i = 0; i < transform.childCount; i++) {
                if (match == null)
                    transform.GetChild(i).gameObject.SetActive(false);
                else {
                    var go = transform.GetChild(i);
                    if (go.name.StartsWith(match)) {
                        go.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void InitTitle() {
            TextAnchor anchor = TextAnchor.MiddleCenter;
            Vector2 anchorMin = new Vector2(0, 0);
            Vector2 anchorMax = new Vector2(0, 0);
            Vector3 titlePosition;
            float titleWid = 200;
            float titleHig = 20;
            switch (title.align) {
                case Align.left:
                    anchor = TextAnchor.MiddleLeft;
                    titlePosition = new Vector3(title.left, chartHig - title.top, 0);
                    break;
                case Align.right:
                    anchor = TextAnchor.MiddleRight;
                    titlePosition = new Vector3(chartWid - title.right - titleWid,
                        chartHig - title.top, 0);
                    break;
                case Align.center:
                    anchor = TextAnchor.MiddleCenter;
                    titlePosition = new Vector3(chartWid / 2 - titleWid / 2, chartHig - title.top, 0);
                    break;
                default:
                    anchor = TextAnchor.MiddleCenter;
                    titlePosition = new Vector3(0, -title.top, 0);
                    break;
            }
            Text titleText = ChartUtils.AddTextObject(TILTE_TEXT, transform, themeInfo.font,
                        themeInfo.textColor, anchor, anchorMin, anchorMax, new Vector2(0, 1),
                        new Vector2(titleWid, titleHig), 16);
            titleText.alignment = anchor;
            titleText.gameObject.SetActive(title.show);
            titleText.transform.localPosition = titlePosition;
            titleText.text = title.text;

            Text subText = ChartUtils.AddTextObject(SUB_TILTE_TEXT, transform, themeInfo.font,
                        themeInfo.textColor, anchor, anchorMin, anchorMax, new Vector2(0, 1),
                        new Vector2(titleWid, titleHig), 14);
            subText.alignment = anchor;
            subText.gameObject.SetActive(title.show && !string.IsNullOrEmpty(title.subText));
            subText.transform.localPosition = titlePosition - new Vector3(0, 15, 0);
            subText.text = title.subText;
        }

        protected virtual void InitLegend() {
            for (int i = 0; i < legend.dataList.Count; i++) {
                //LegendData data = legend.dataList[i];
                Button btn = ChartUtils.AddButtonObject(LEGEND_TEXT + "_" + i, transform, themeInfo.font,
                    themeInfo.textColor, Vector2.zero, Vector2.zero, Vector2.zero,
                    new Vector2(legend.itemWidth, legend.itemHeight));
                legend.SetDataButton(i, btn);
                Color bcolor = themeInfo.GetColor(i);
                btn.gameObject.SetActive(legend.show);
                btn.transform.localPosition = GetLegendPosition(i);
                btn.GetComponent<Image>().color = bcolor;
                btn.GetComponentInChildren<Text>().text = legend.dataList[i];
                btn.onClick.AddListener(delegate () {
                    int index = int.Parse(btn.name.Split('_')[1]);
                    legend.SetShowData(index, !legend.IsShowSeries(index));
                    btn.GetComponent<Image>().color = legend.IsShowSeries(index) ?
                        themeInfo.GetColor(index) : themeInfo.unableColor;
                    OnYMaxValueChanged();
                    OnLegendButtonClicked();
                    RefreshChart();
                });
            }
        }

        private void InitTooltip() {
            GameObject obj = ChartUtils.AddTooltipObject("tooltip", transform, themeInfo.font);
            tooltip.SetObj(obj);
            tooltip.SetBackgroundColor(themeInfo.tooltipBackgroundColor);
            tooltip.SetTextColor(themeInfo.tooltipTextColor);
            tooltip.SetActive(false);
        }

        private Vector3 GetLegendPosition(int i) {
            int legendCount = legend.dataList.Count;
            switch (legend.location) {
                case Location.bottom:
                case Location.top:
                    float startX = legend.left;
                    if (startX <= 0) {
                        startX = (chartWid - (legendCount * legend.itemWidth -
                            (legendCount - 1) * legend.itemGap)) / 2;
                    }
                    float posY = legend.location == Location.bottom ?
                        legend.bottom : chartHig - legend.top - legend.itemHeight;
                    return new Vector3(startX + i * (legend.itemWidth + legend.itemGap), posY, 0);
                case Location.left:
                case Location.right:
                    float startY = 0;
                    if (legend.top > 0) {
                        startY = chartHig - legend.top - legend.itemHeight;
                    } else if (startY <= 0) {
                        float legendHig = legendCount * legend.itemHeight - (legendCount - 1) * legend.itemGap;
                        float offset = (chartHig - legendHig) / 2;
                        startY = chartHig - offset - legend.itemHeight;
                    }
                    float posX = legend.location == Location.left ?
                        legend.left :
                        chartWid - legend.right - legend.itemWidth;
                    return new Vector3(posX, startY - i * (legend.itemHeight + legend.itemGap), 0);
                default: break;
            }
            return Vector3.zero;
        }

        protected virtual float GetMaxValue() {
            float max = 0;
            for (int i = 0; i < seriesList.Count; i++) {
                if (legend.IsShowSeries(i) && seriesList[i].Max > max) max = seriesList[i].Max;
            }
            //int bigger = (int)(max * 1.3f);
            //return bigger < 10 ? bigger : bigger - bigger % 10;

            Vector2 v2 = ChartUtils.MagicNumbers(max, 0);
            if (v2.x < 5) v2.x = 5;
            return v2.x;
        }

        protected virtual Vector2 GetMaxAndMinValue() {
            float max = 0,min = 0;
            for (int i = 0; i < seriesList.Count; i++) {
                if (legend.IsShowSeries(i) && seriesList[i].Max > max) max = seriesList[i].Max;
                if (legend.IsShowSeries(i) && seriesList[i].Min < min) min = seriesList[i].Min;
            }

            Vector2 v2 = ChartUtils.MagicNumbers(max, min);
            return v2;
        }

        private void CheckSize() {
            if (checkWid != chartWid || checkHig != chartHig) {
                checkWid = chartWid;
                checkHig = chartHig;
                OnSizeChanged();
            }
        }

        private void CheckTheme() {
            if (checkTheme != theme) {
                checkTheme = theme;
                OnThemeChanged();
            }
        }

        private void CheckTile() {
            if (checkTitle == null || title == null) return;
            if (checkTitle.align != title.align ||
                checkTitle.left != title.left ||
                checkTitle.right != title.right ||
                checkTitle.top != title.top) {
                checkTitle.align = title.align;
                checkTitle.left = title.left;
                checkTitle.right = title.right;
                checkTitle.top = title.top;
                OnTitleChanged();
            }
        }

        private void CheckLegend() {
            if (checkLegend.checkDataListCount != legend.dataList.Count) {
                checkLegend.checkDataListCount = legend.dataList.Count;
                OnLegendDataListChanged();
            }

            if (checkLegend.itemWidth != legend.itemWidth ||
                checkLegend.itemHeight != legend.itemHeight ||
                checkLegend.itemGap != legend.itemGap ||
                checkLegend.left != legend.left ||
                checkLegend.right != legend.right ||
                checkLegend.bottom != legend.bottom ||
                checkLegend.top != legend.top ||
                checkLegend.location != legend.location ||
                checkLegend.show != legend.show) {
                checkLegend.itemWidth = legend.itemWidth;
                checkLegend.itemHeight = legend.itemHeight;
                checkLegend.itemGap = legend.itemGap;
                checkLegend.left = legend.left;
                checkLegend.right = legend.right;
                checkLegend.bottom = legend.bottom;
                checkLegend.top = legend.top;
                checkLegend.location = legend.location;
                checkLegend.show = legend.show;
                OnLegendChanged();
            }
        }

        private void CheckTooltip() {
            if (!tooltip.show) return;
            tooltip.DataIndex = 0;
            Vector2 local;

            if (!checkTooltip)
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                Input.mousePosition, null, out local))
                return;

            if (local.x < 0 || local.x > chartWid ||
                local.y < 0 || local.y > chartHig)
                return;

            CheckTootipArea(local);
        }

        protected virtual void CheckTootipArea(Vector2 localPostion) {
        }

        protected virtual void OnSizeChanged() {
            InitTitle();
            InitLegend();
        }

        protected virtual void OnThemeChanged() {
            switch (theme) {
                case Theme.Dark:
                    themeInfo.Copy(ThemeInfo.Dark);
                    break;
                case Theme.Default:
                    themeInfo.Copy(ThemeInfo.Default);
                    break;
                case Theme.Light:
                    themeInfo.Copy(ThemeInfo.Light);
                    break;
                case Theme.MtLineStyle:
                    themeInfo.Copy(ThemeInfo.MtLineStyle);
                    break;
                case Theme.MtBarStyle:
                    themeInfo.Copy(ThemeInfo.MtBarStyle);
                    break;
            }
            InitTitle();
            InitLegend();
        }

        protected virtual void OnTitleChanged() {
            InitTitle();
        }

        protected virtual void OnLegendChanged() {
            for (int i = 0; i < legend.dataList.Count; i++) {
                Button btn = legend.GetButton(i);
                btn.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(legend.itemWidth, legend.itemHeight);
                Text txt = btn.GetComponentInChildren<Text>();
                txt.transform.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(legend.itemWidth, legend.itemHeight);
                txt.transform.localPosition = Vector3.zero;
                btn.transform.localPosition = GetLegendPosition(i);
                btn.gameObject.SetActive(legend.show);
            }
        }

        protected virtual void OnLegendDataListChanged() {
            InitLegend();
        }

        protected virtual void OnYMaxValueChanged() {
        }

        protected virtual void OnLegendButtonClicked() {
        }

        public void RefreshChart() {
            int tempWid = (int)chartWid;
            if (!rectTransform) return;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempWid - 1);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempWid);
        }

        protected virtual void RefreshTooltip() {
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            DrawBackground(vh);
            DrawChart(vh);
            DrawTooltip(vh);
        }

        protected virtual void DrawChart(VertexHelper vh) {
        }

        protected virtual void DrawTooltip(VertexHelper vh) {
        }

        private void DrawBackground(VertexHelper vh) {
            // draw bg
            Vector3 p1 = new Vector3(0, chartHig);
            Vector3 p2 = new Vector3(chartWid, chartHig);
            Vector3 p3 = new Vector3(chartWid, 0);
            Vector3 p4 = new Vector3(0, 0);
            ChartUtils.DrawPolygon(vh, p1, p2, p3, p4, themeInfo.backgroundColor);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            checkTooltip = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            checkTooltip = false;
        }
    }
}
