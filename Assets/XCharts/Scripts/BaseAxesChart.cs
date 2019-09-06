using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

namespace xcharts {

    [System.Serializable]
    public class Coordinate {
        public bool show = true;
        public float left = 40f;
        public float right = 30f;
        public float top = 40;
        public float bottom = 25f;
        public float tickness = 1f;
        public float splitWidth = 5.0f;

        public float xScaleRectWidth = 20f;
        public float xScaleRectHeight = 20f;
    }

    [System.Serializable]
    public enum AxisType {
        value,
        category,
        time,
        log
    }

    public enum SplitLineType {
        solid,
        dashed,
        dotted
    }

    [System.Serializable]
    public class Axis {
        public AxisType type;
        public int splitNumber = 5;
        public int maxSplitNumber = 5;
        public bool showSplitLine = true;
        public SplitLineType splitLineType = SplitLineType.dashed;
        public bool boundaryGap = true;
        [SerializeField]
        private List<string> data;
        private List<string> multidata = new List<string>();

        private List<string> keyData = new List<string>();
        private List<string> xLabel = new List<string>();

        private List<string> Data {
            get {
                if (multidata.Count > 0) return multidata;
                else return data;
            }
        }

        public void AddData(string category) {
            if (data.Count >= maxSplitNumber && maxSplitNumber != 0) {
                data.RemoveAt(0);
            }
            data.Add(category);
        }

        public void AddMultiData(string category) {
            if (multidata.Count >= maxSplitNumber && maxSplitNumber != 0) {
                multidata.RemoveAt(0);
            }
            multidata.Add(category);
        }

        public void AddKeyData(string key) {
            if (keyData.Count >= maxSplitNumber && maxSplitNumber != 0) {
                keyData.RemoveAt(0);
            }
            keyData.Add(key);
        }

        public void AddxLabel(string label) {
            if (keyData.Count >= maxSplitNumber && maxSplitNumber != 0) {
                xLabel.RemoveAt(0);
            }
            xLabel.Add(label);
        }

        public void ClearData() {
            data.Clear();
        }

        public void ClearMultiData() {
            multidata.Clear();
        }

        public void ClearKeyData() {
            keyData.Clear();
        }

        public void ClearxLabel() {
            xLabel.Clear();
        }

        public string GetData(int index) {
            return Data[index];
        }

        public string GetKeyData(int index) {
            if(index < keyData.Count)
                return keyData[index];
            return null;
        }

        public string GetxLableData(int index) {
            if (index < xLabel.Count)
                return xLabel[index];
            return null;
        }

        public int GetSplitNumber() {
            if (Data.Count > 2 * splitNumber || Data.Count <= 0)
            //if (Data.Count > splitNumber || Data.Count <= 0)
                return splitNumber;
            else
                return Data.Count;
        }

        public float GetSplitWidth(float coordinateWidth) {
            return coordinateWidth / (boundaryGap ? GetSplitNumber() : GetSplitNumber() - 1);
        }

        public int GetDataNumber() {
            return Data.Count;
        }

        public float GetDataWidth(float coordinateWidth) {
            return coordinateWidth / (boundaryGap ? Data.Count : Data.Count - 1);
        }

        public string GetScaleName(int index, float maxData = 0,float minData = 0) {
            if (type == AxisType.value) {
                string str = (minData + ((maxData - minData) * index / (float)(GetSplitNumber() - 1))).ToString("N2");
                if (str.Substring(str.Length - 3, 3) == ".00") return str.Substring(0, str.Length - 3);
                return str;
            }
            int dataCount = Data.Count;
            if (dataCount <= 0) return "";
            float rate = dataCount / (GetScaleNumber() - 1);
            if (rate < 1) rate = 1;
            int newIndex = (int)(index * rate >= dataCount - 1 ? dataCount - 1 : index * rate);
            return xLabel[newIndex];
        }

        public int GetScaleNumber() {
            if (Data.Count > 2 * splitNumber || Data.Count <= 0)
                return boundaryGap ? splitNumber + 1 : splitNumber;
            else
                return boundaryGap ? Data.Count + 1 : Data.Count;
        }

        public float GetScaleWidth(float coordinateWidth) {
            int num = GetScaleNumber() - 1;
            if (num <= 0) num = 1;
            return coordinateWidth / num;
        }
    }

    [System.Serializable]
    public class XAxis : Axis {
        public bool horizontal = true;
        public HorizontalWrapMode horizontalWrapMode= HorizontalWrapMode.Overflow;
    }

    [System.Serializable]
    public class YAxis : Axis {
    }

    public class BaseAxesChart : BaseChart {
        private const int DEFAULT_YSACLE_NUM = 5;
        private const string YSCALE_TEXT_PREFIX = "yScale";
        private const string XSCALE_TEXT_PREFIX = "xScale";

        [SerializeField]
        protected Coordinate coordinate = new Coordinate();
        [SerializeField]
        protected XAxis xAxis = new XAxis();
        [SerializeField]
        protected YAxis yAxis = new YAxis();

        private float lastXMaxValue;
        private float lastYMaxValue;
        private float lastCoordinateWid;
        private float lastCoordinateHig;
        private float lastCoordinateScaleLen;

        private XAxis checkXAxis = new XAxis();
        private YAxis checkYAxis = new YAxis();
        private Coordinate checkCoordinate = new Coordinate();

        protected List<Text> yScaleTextList = new List<Text>();
        protected List<Text> xScaleTextList = new List<Text>();
        protected float zeroX { get { return coordinate.left; } }
        protected float zeroY { get { return coordinate.bottom; } }
        protected float coordinateWid { get { return chartWid - coordinate.left - coordinate.right; } }
        protected float coordinateHig { get { return chartHig - coordinate.top - coordinate.bottom; } }

        public Axis XAxis { get { return xAxis; } }
        public Axis YAxis { get { return yAxis; } }

        protected override void Awake() {
            base.Awake();
            lastCoordinateHig = chartHig;
            lastCoordinateWid = chartWid;
            lastCoordinateScaleLen = coordinate.splitWidth;
            checkXAxis = xAxis;
            checkYAxis = yAxis;
            InitXScale();
            InitYScale();
        }

        protected override void Update() {
            base.Update();
            CheckYAxisType();
            CheckXAxisType();
            //CheckMaxValue();
            CheckCoordinate();
        }

        protected override void DrawChart(VertexHelper vh) {
            base.DrawChart(vh);
            DrawCoordinate(vh);
        }

        protected override void CheckTootipArea(Vector2 local) {
            if (local.x < zeroX || local.x > zeroX + coordinateWid ||
                local.y < zeroY || local.y > zeroY + coordinateHig) {
                tooltip.DataIndex = 0;
                RefreshTooltip();
            } else {
                if (xAxis.type == AxisType.value) {
                    float splitWid = yAxis.GetDataWidth(coordinateHig);
                    for (int i = 0; i < yAxis.GetDataNumber(); i++) {
                        float pY = zeroY + i * splitWid;
                        if (yAxis.boundaryGap) {
                            if (local.y > pY && local.y <= pY + splitWid) {
                                tooltip.DataIndex = i + 1;
                                break;
                            }
                        } else {
                            if (local.y > pY - splitWid / 2 && local.y <= pY + splitWid / 2) {
                                tooltip.DataIndex = i + 1;
                                break;
                            }
                        }
                    }
                } else {
                    float splitWid = xAxis.GetDataWidth(coordinateWid);
                    for (int i = 0; i < xAxis.GetDataNumber(); i++) {
                        float pX = zeroX + i * splitWid;
                        if (xAxis.boundaryGap) {
                            if (local.x > pX && local.x <= pX + splitWid) {
                                tooltip.DataIndex = i + 1;
                                break;
                            }
                        } else {
                            if (local.x > pX - splitWid / 2 && local.x <= pX + splitWid / 2) {
                                tooltip.DataIndex = i + 1;
                                break;
                            }
                        }
                    }
                }
            }
            if (tooltip.DataIndex > 0) {
                tooltip.UpdatePos(new Vector2(local.x + 18, local.y - 25));
                RefreshTooltip();
                if (tooltip.LastDataIndex != tooltip.DataIndex) {
                    RefreshChart();

                    string lastDataCode = xAxis.GetKeyData(tooltip.LastDataIndex);
                    if (!string.IsNullOrEmpty(lastDataCode)) {
                        OnExit?.Invoke(lastDataCode);
                    }

                    string dataCode = xAxis.GetKeyData(tooltip.DataIndex);
                    if (!string.IsNullOrEmpty(dataCode)) {
                        OnEnter?.Invoke(dataCode);
                    }
                }
                tooltip.LastDataIndex = tooltip.DataIndex;
            }
        }

        protected override void RefreshTooltip() {
            base.RefreshTooltip();
            int index = tooltip.DataIndex - 1;
            Axis tempAxis = xAxis.type == AxisType.value ? (Axis)yAxis : (Axis)xAxis;
            if (index < 0) {
                tooltip.SetActive(false);
                return;
            }
            tooltip.SetActive(true);
            if (seriesList.Count == 1) {
                string txt = tempAxis.GetxLableData(index) + ": " + seriesList[0].DataList[index].ToString("N2");
                tooltip.UpdateTooltipText(txt);
            } else {
                StringBuilder sb = new StringBuilder(tempAxis.GetxLableData(index));
                for (int i = 0; i < seriesList.Count; i++) {
                    string strColor = ColorUtility.ToHtmlStringRGBA(themeInfo.GetColor(i));
                    string key = seriesList[i].name;

                    if (index < seriesList[i].DataList.Count) {
                        float value = seriesList[i].DataList[index];
                        sb.Append("\n");
                        sb.AppendFormat("<color=#{0}>● </color>", strColor);
                        sb.AppendFormat("{0}: {1}", key, value.ToString("N2"));
                    }
                }
                tooltip.UpdateTooltipText(sb.ToString());
            }
            var pos = tooltip.GetPos();
            if (pos.x + tooltip.Width > chartWid) {
                pos.x = chartWid - tooltip.Width;
            }
            if (pos.y - tooltip.Height - coordinate.bottom < 0) {
                pos.y = tooltip.Height + coordinate.bottom;
            }
            tooltip.UpdatePos(pos);
        }

        TextGenerationSettings GetTextSetting() {
            var setting = new TextGenerationSettings();
            var fontdata = FontData.defaultFontData;

            //setting.generationExtents = rectTransform.rect.size;
            setting.generationExtents = new Vector2(200.0F, 50.0F);
            setting.fontSize = 14;
            setting.textAnchor = TextAnchor.MiddleCenter;
            setting.scaleFactor = 1f;
            setting.color = Color.red;
            setting.font = themeInfo.font;
            setting.pivot = new Vector2(0.5f, 0.5f);
            setting.richText = false;
            setting.lineSpacing = 0;
            setting.fontStyle = FontStyle.Normal;
            setting.resizeTextForBestFit = false;
            setting.horizontalOverflow = HorizontalWrapMode.Overflow;
            setting.verticalOverflow = VerticalWrapMode.Overflow;

            return setting;

        }

        protected override void OnThemeChanged() {
            base.OnThemeChanged();
            InitXScale();
            InitYScale();
        }

        public void AddXAxisCategory(string category) {
            xAxis.AddData(category);
            OnXAxisChanged();
        }

        public void AddYAxisCategory(string category) {
            yAxis.AddData(category);
            OnYAxisChanged();
        }

        protected virtual void InitYScale() {
            foreach (var item in yScaleTextList) {
                item.text = string.Empty;
            }

            yScaleTextList.Clear();
            //float max = GetMaxValue();
            Vector2 v2 = GetMaxAndMinValue();
            float scaleWid = yAxis.GetScaleWidth(coordinateHig);
            for (int i = 0; i < yAxis.GetSplitNumber(); i++) {
                Text txt = ChartUtils.AddTextObject(YSCALE_TEXT_PREFIX + i, transform, themeInfo.font,
                        themeInfo.textColor, TextAnchor.MiddleRight, Vector2.zero, Vector2.zero,
                        new Vector2(1, 0.5f),
                        new Vector2(coordinate.left, 20));
                txt.transform.localPosition = GetYScalePosition(scaleWid, i);
                txt.text = yAxis.GetScaleName(i, v2.x,v2.y);
                txt.gameObject.SetActive(coordinate.show);
                yScaleTextList.Add(txt);
            }
        }

        protected virtual void InitXScale() {
            foreach (var item in xScaleTextList) {
                item.text = string.Empty;
            }

            xScaleTextList.Clear();
            //float max = GetMaxValue();
            Vector2 v2 = GetMaxAndMinValue();
            float scaleWid = xAxis.GetScaleWidth(coordinateWid);
            Vector2 sizeDelta;
            HorizontalWrapMode horizontalWrap;
            int fontSize = 14;
            if (xAxis.horizontal) {
                sizeDelta = new Vector2(scaleWid, 20);
                horizontalWrap = HorizontalWrapMode.Overflow;
            }
            else {
                sizeDelta = new Vector2(fontSize*1.5f, 20);
                horizontalWrap = HorizontalWrapMode.Wrap;
            }
           
            for (int i = 0; i < xAxis.GetSplitNumber(); i++) {
                Text txt = ChartUtils.AddTextObject(XSCALE_TEXT_PREFIX + i, transform, themeInfo.font,
                    themeInfo.textColor, TextAnchor.UpperCenter, Vector2.zero, Vector2.zero,
                  xAxis.horizontal? new Vector2(1, 0.5f) : new Vector2(0.5f, 1),
                    sizeDelta, 14, horizontalWrap);
                txt.transform.localPosition = GetXScalePosition(scaleWid, i, xAxis.horizontal, fontSize);

                txt.text = xAxis.GetScaleName(i, v2.x, v2.y);
                txt.gameObject.SetActive(coordinate.show);
                xScaleTextList.Add(txt);
            }
        }

        protected Vector3 GetYScalePosition(float scaleWid, int i) {
            if (yAxis.boundaryGap) {
                return new Vector3(zeroX - coordinate.splitWidth - 2f,
                    zeroY + (i + 0.5f) * scaleWid, 0);
            } else {
                return new Vector3(zeroX - coordinate.splitWidth - 2f,
                    zeroY + i * scaleWid, 0);
            }
        }

        protected Vector3 GetXScalePosition(float scaleWid, int i,bool horizontal,int fontSize) {
            if (horizontal) {
                if (xAxis.boundaryGap) {
                    return new Vector3(zeroX + (i + 1) * scaleWid, zeroY - coordinate.splitWidth - 5, 0);
                }
                else {
                    return new Vector3(zeroX + (i + 1 - 0.5f) * scaleWid,
                        zeroY - coordinate.splitWidth - 10, 0);
                }
            }
            else {
                if (xAxis.boundaryGap) {
                    return new Vector3(zeroX + (i + 1) * scaleWid-scaleWid/2, zeroY - coordinate.splitWidth - 5, 0);
                }
                else {
                    return new Vector3(zeroX + (i + 1 - 0.5f) * scaleWid - scaleWid / 2,
                        zeroY - coordinate.splitWidth - 10, 0);
                }
            }
           
        }

        private void CheckCoordinate() {
            if (lastCoordinateHig != coordinateHig
                || lastCoordinateWid != coordinateWid
                || lastCoordinateScaleLen != coordinate.splitWidth) {
                lastCoordinateWid = coordinateWid;
                lastCoordinateHig = coordinateHig;
                lastCoordinateScaleLen = coordinate.splitWidth;
                OnCoordinateSize();
            }
            if (checkCoordinate.show != coordinate.show) {
                checkCoordinate.show = coordinate.show;
                OnXAxisChanged();
                OnYAxisChanged();
            }
        }

        private void CheckYAxisType() {
            if (checkYAxis.type != yAxis.type ||
                checkYAxis.boundaryGap != yAxis.boundaryGap ||
                checkYAxis.showSplitLine != yAxis.showSplitLine ||
                checkYAxis.splitNumber != yAxis.splitNumber) {
                checkYAxis.type = yAxis.type;
                checkYAxis.boundaryGap = yAxis.boundaryGap;
                checkYAxis.showSplitLine = yAxis.showSplitLine;
                checkYAxis.splitNumber = yAxis.splitNumber;
                OnYAxisChanged();
            }
        }

        private void CheckXAxisType() {
            if (checkXAxis.type != xAxis.type ||
                checkXAxis.boundaryGap != xAxis.boundaryGap ||
                checkXAxis.showSplitLine != xAxis.showSplitLine ||
                checkXAxis.splitNumber != xAxis.splitNumber) {
                checkXAxis.type = xAxis.type;
                checkXAxis.boundaryGap = xAxis.boundaryGap;
                checkXAxis.showSplitLine = xAxis.showSplitLine;
                checkXAxis.splitNumber = xAxis.splitNumber;
                OnXAxisChanged();
            }
        }

        private void CheckMaxValue() {
            if (xAxis.type == AxisType.value) {
                float max = GetMaxValue();
                //if (lastXMaxValue != max) {
                //    lastXMaxValue = max;
                OnXMaxValueChanged();
                //}
            }
            if (yAxis.type == AxisType.value) {
                float max = GetMaxValue();
                //if (lastYMaxValue != max) {
                //    lastYMaxValue = max;
                OnYMaxValueChanged();
                //}
            }
        }

        protected virtual void OnCoordinateSize() {
            InitXScale();
            InitYScale();
        }

        protected virtual void OnYAxisChanged() {
            HideChild(YSCALE_TEXT_PREFIX);
            InitYScale();
        }

        protected virtual void OnXAxisChanged() {
            HideChild(XSCALE_TEXT_PREFIX);
            InitXScale();
        }

        protected virtual void OnXMaxValueChanged() {
            float max = GetMaxValue();
            for (int i = 0; i < xScaleTextList.Count; i++) {
                xScaleTextList[i].text = ((int)(max * i / xScaleTextList.Count)).ToString();
            }
        }

        protected override void OnSizeChanged() {
            base.OnSizeChanged();
            InitXScale();
            InitYScale();
        }

        protected override void OnYMaxValueChanged() {
            float max = GetMaxValue();
            for (int i = 0; i < yScaleTextList.Count; i++) {
                yScaleTextList[i].text = ((int)(max * i / (yScaleTextList.Count - 1))).ToString();
            }
        }

        protected virtual void DrawCoordinate(VertexHelper vh) {
            if (!coordinate.show) return;
            // draw splitline
            for (int i = 1; i < yAxis.GetScaleNumber(); i++) {
                float pX = zeroX - coordinate.splitWidth;
                float pY = zeroY + i * coordinateHig / (yAxis.GetScaleNumber() - 1);
                ChartUtils.DrawLine(vh, new Vector3(pX, pY), new Vector3(zeroX, pY), coordinate.tickness,
                    themeInfo.axisLineColor);
                if (yAxis.showSplitLine) {
                    DrawSplitLine(vh, true, yAxis.splitLineType, new Vector3(zeroX, pY),
                        new Vector3(zeroX + coordinateWid, pY));
                }
            }
            for (int i = 1; i < xAxis.GetScaleNumber(); i++) {
                float pX = zeroX + i * coordinateWid / (xAxis.GetScaleNumber() - 1);
                float pY = zeroY - coordinate.splitWidth - 2;
                ChartUtils.DrawLine(vh, new Vector3(pX, zeroY), new Vector3(pX, pY), coordinate.tickness,
                    themeInfo.axisLineColor);
                if (xAxis.showSplitLine) {
                    DrawSplitLine(vh, false, xAxis.splitLineType, new Vector3(pX, zeroY),
                        new Vector3(pX, zeroY + coordinateHig));
                }
            }
            //draw x,y axis
            ChartUtils.DrawLine(vh, new Vector3(zeroX, zeroY - coordinate.splitWidth),
                new Vector3(zeroX, zeroY + coordinateHig + 2), coordinate.tickness,
                themeInfo.axisLineColor);
            ChartUtils.DrawLine(vh, new Vector3(zeroX - coordinate.splitWidth, zeroY),
                new Vector3(zeroX + coordinateWid + 2, zeroY), coordinate.tickness,
                themeInfo.axisLineColor);
        }

        protected void DrawSplitLine(VertexHelper vh, bool isYAxis, SplitLineType type, Vector3 startPos,
            Vector3 endPos) {
            switch (type) {
                case SplitLineType.dashed:
                case SplitLineType.dotted:
                    var startX = startPos.x;
                    var startY = startPos.y;
                    var dashLen = type == SplitLineType.dashed ? 6 : 2.5f;
                    var count = isYAxis ? (endPos.x - startPos.x) / (dashLen * 2) :
                        (endPos.y - startPos.y) / (dashLen * 2);
                    for (int i = 0; i < count; i++) {
                        if (isYAxis) {
                            var toX = startX + dashLen;
                            ChartUtils.DrawLine(vh, new Vector3(startX, startY), new Vector3(toX, startY),
                                coordinate.tickness, themeInfo.axisSplitLineColor);
                            startX += dashLen * 2;
                        } else {
                            var toY = startY + dashLen;
                            ChartUtils.DrawLine(vh, new Vector3(startX, startY), new Vector3(startX, toY),
                                coordinate.tickness, themeInfo.axisSplitLineColor);
                            startY += dashLen * 2;
                        }

                    }
                    break;
                case SplitLineType.solid:
                    ChartUtils.DrawLine(vh, startPos, endPos, coordinate.tickness,
                        themeInfo.axisSplitLineColor);
                    break;
            }
        }

        public void ClearSeriesData() {
            XAxis.ClearKeyData();
            XAxis.ClearMultiData();
            XAxis.ClearxLabel();
            YAxis.ClearKeyData();
            YAxis.ClearMultiData();
            ClearMultiData();
            //RefreshChart();
        }

        public virtual void AddSeriesData(List<SeriesData> seriesData) {          
            foreach (var item in seriesData) {
                XAxis.AddKeyData(item.code);
                XAxis.AddMultiData(item.label);
                XAxis.AddxLabel(item.xLabel);
                for (int i = 0; i < item.value.Length; i++) {
                    AddMultiData(i, item.value[i]);
                }
            }
            //RefreshChart();
            InitXScale();
            InitYScale();
        }

        public virtual void SetSeriesName(List<string> seriesName) {
            for (int i = 0; i < seriesList.Count; i++) {
                seriesList[i].name = i < seriesName.Count ? seriesName[i] : string.Empty;
            }
        }
    }
}

