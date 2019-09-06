using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;

namespace xcharts {
    public class MixedLineAndBarChart : BaseAxesChart {
        [SerializeField]
        public LineInfo lineInfo;

        [SerializeField]
        public BarInfo barInfo = new BarInfo();

        List<Text> rightYScaleTextList = new List<Text>();

        public Text RightYUnit;

        public Text YUnit;

        public ChartMode Mode = ChartMode.Mixed;


        public bool ShowRightScale = true;

        public string Title {
            set {
                title.text = value;
            }
        }

        protected override void Awake() {
            base.Awake();
        }

        protected override void Update() {
            base.Update();
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            base.OnPopulateMesh(vh);
            DrawBarChart(vh);
            DrawLineChart(vh);
        }

        protected void DrawBarChart(VertexHelper vh) {
            if (Mode == ChartMode.Line) return;
            int num= Mode == ChartMode.Mixed?1:seriesList.Count;
            if (yAxis.type == AxisType.category) {
                float scaleWid = yAxis.GetDataWidth(coordinateHig);
                float barWid = barInfo.barWid > 1 ? barInfo.barWid : scaleWid * barInfo.barWid;
                float offset = (scaleWid - barWid * num - barInfo.space * (num - 1)) / 2;
                offset = offset < 0 ? 0 : offset;
                float max = GetMaxValue();
                if (tooltip.show && tooltip.DataIndex > 0) {
                    float pX = zeroX + coordinateWid;
                    float pY = zeroY + scaleWid * (tooltip.DataIndex - 1);
                    Vector3 p1 = new Vector3(zeroX, pY);
                    Vector3 p2 = new Vector3(zeroX, pY + scaleWid);
                    Vector3 p3 = new Vector3(pX, pY + scaleWid);
                    Vector3 p4 = new Vector3(pX, pY);
                    ChartUtils.DrawPolygon(vh, p1, p2, p3, p4, themeInfo.tooltipFlagAreaColor);
                }
                for (int j = 0; j < num; j++) {
                    if (!legend.IsShowSeries(j)) continue;
                    Series series = seriesList[j];
                    Color color = themeInfo.GetColor(j);
                    int startIndex = 0;
                    if (series.showDataNumber > 0 && series.DataList.Count > series.showDataNumber) {
                        startIndex = series.DataList.Count - series.showDataNumber;
                    }
                    for (int i = startIndex; i < series.DataList.Count; i++) {
                        float data = series.DataList[i];
                        float pX = zeroX + coordinate.tickness;
                        float pY = zeroY + i * scaleWid;
                        if (!yAxis.boundaryGap) pY -= scaleWid / 2;
                        float barHig = data / max * coordinateWid;
                        float space = offset + j * (barWid + barInfo.space);
                        Vector3 p1 = new Vector3(pX, pY + space + barWid);
                        Vector3 p2 = new Vector3(pX + barHig, pY + space + barWid);
                        Vector3 p3 = new Vector3(pX + barHig, pY + space);
                        Vector3 p4 = new Vector3(pX, pY + space);
                        ChartUtils.DrawPolygon(vh, p1, p2, p3, p4, color);
                    }
                }
            }
            else {
                float scaleWid = xAxis.GetDataWidth(coordinateWid);
                float barWid = barInfo.barWid > 1 ? barInfo.barWid : scaleWid * barInfo.barWid;
                float offset = (scaleWid - barWid * num - barInfo.space * (num - 1)) / 2;
                     offset = offset < 0 ? 0 : offset;
                float max = GetMaxValue();
                if (tooltip.show && tooltip.DataIndex > 0) {
                    float tooltipSplitWid = scaleWid < 1 ? 1 : scaleWid;
                    float pX = zeroX + scaleWid * (tooltip.DataIndex - 1);
                    float pY = zeroY + coordinateHig;
                    Vector3 p1 = new Vector3(pX, zeroY);
                    Vector3 p2 = new Vector3(pX, pY);
                    Vector3 p3 = new Vector3(pX + tooltipSplitWid, pY);
                    Vector3 p4 = new Vector3(pX + tooltipSplitWid, zeroY);
                    ChartUtils.DrawPolygon(vh, p1, p2, p3, p4, themeInfo.tooltipFlagAreaColor);
                }
                for (int j = 0; j < num; j++) {
                    if (!legend.IsShowSeries(j)) continue;
                    Series series = seriesList[j];
                    Color color = themeInfo.GetColor(j);
                    int startIndex = 0;
                    if (series.showDataNumber > 0 && series.DataList.Count > series.showDataNumber) {
                        startIndex = series.DataList.Count - series.showDataNumber;
                    }
                    for (int i = startIndex; i < series.DataList.Count; i++) {
                        float data = series.DataList[i];
                        float pX = zeroX + i * scaleWid;
                        if (!xAxis.boundaryGap) pX -= scaleWid / 2;
                        float pY = zeroY + coordinate.tickness;
                        float barHig = data / max * coordinateHig;
                        float space = offset + j * (barWid + barInfo.space);
                        Vector3 p1 = new Vector3(pX + space, pY);
                        Vector3 p2 = new Vector3(pX + space, pY + barHig);
                        Vector3 p3 = new Vector3(pX + space + barWid, pY + barHig);
                        Vector3 p4 = new Vector3(pX + space + barWid, pY);
                        ChartUtils.DrawPolygon(vh, p1, p2, p3, p4, color);
                    }
                }
            }
        }

        void DrawLineChart(VertexHelper vh) {
            if (Mode == ChartMode.Bar)
                return;
            int seriesCount = seriesList.Count;

            Vector2 v2 = GetMaxAndMinValue();
            float max = v2.x - v2.y;
            //float max = GetMaxValue();

            float minValue = v2.y;

            float scaleWid = xAxis.GetDataWidth(coordinateWid);
            int num = Mode==ChartMode.Mixed?1:0;
            for (int j = num; j < seriesCount; j++) {
                if (!legend.IsShowSeries(j)) continue;
                Series series = seriesList[j];
                Color32 color = themeInfo.GetColor(j);
                Vector3 lp = Vector3.zero;
                Vector3 np = Vector3.zero;
                float startX = zeroX + (xAxis.boundaryGap ? scaleWid / 2 : 0);
                int showDataNumber = series.showDataNumber;
                int startIndex = 0;
                if (series.showDataNumber > 0 && series.DataList.Count > series.showDataNumber) {
                    startIndex = series.DataList.Count - series.showDataNumber;
                }
                for (int i = startIndex; i < series.DataList.Count; i++) {
                    float value = series.DataList[i];

                    np = new Vector3(startX + i * scaleWid, zeroY + (value - minValue) * coordinateHig / max);
                    if (i > 0) {
                        if (lineInfo.smooth) {
                            var list = ChartUtils.GetBezierList(lp, np, lineInfo.smoothStyle);
                            Vector3 start, to;
                            start = list[0];
                            for (int k = 1; k < list.Length; k++) {
                                to = list[k];
                                ChartUtils.DrawLine(vh, start, to, lineInfo.tickness, color);
                                start = to;
                            }
                        } else {
                            ChartUtils.DrawLine(vh, lp, np, lineInfo.tickness, color);
                            if (lineInfo.area) {
                                ChartUtils.DrawPolygon(vh, lp, np, new Vector3(np.x, zeroY),
                                    new Vector3(lp.x, zeroY), color);
                            }
                        }

                    }
                    lp = np;
                }
                // draw point
                if (lineInfo.showPoint) {
                    for (int i = num; i < series.DataList.Count; i++) {
                        float value = series.DataList[i];

                        Vector3 p = new Vector3(startX + i * scaleWid,
                            zeroY + (value - minValue) * coordinateHig / max);
                        float pointWid = lineInfo.pointWid;
                        if (tooltip.show && i == tooltip.DataIndex - 1) {
                            pointWid = pointWid * 1.8f;
                        }
                        if (theme == Theme.Dark) {

                            ChartUtils.DrawCricle(vh, p, pointWid, color,
                                (int)lineInfo.pointWid * 5);
                        } else {
                            ChartUtils.DrawCricle(vh, p, pointWid, Color.white);
                            ChartUtils.DrawDoughnut(vh, p, pointWid - lineInfo.tickness,
                                pointWid, 0, 360, color);
                        }
                    }
                }
            }
        }

        protected override void DrawCoordinate(VertexHelper vh) {
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

            for (int i = 1; i < yAxis.GetScaleNumber(); i++) {
                float pX = zeroX + coordinateWid;
                float pY = zeroY + i * coordinateHig / (yAxis.GetScaleNumber() - 1);
                ChartUtils.DrawLine(vh, new Vector3(pX, pY), new Vector3(pX + coordinate.splitWidth, pY), coordinate.tickness,
                    themeInfo.axisLineColor);
            }

            ChartUtils.DrawLine(vh, new Vector3(zeroX, zeroY - coordinate.splitWidth),
                new Vector3(zeroX, zeroY + coordinateHig + 2), coordinate.tickness,
                themeInfo.axisLineColor);
            ChartUtils.DrawLine(vh, new Vector3(zeroX - coordinate.splitWidth, zeroY),
                new Vector3(zeroX + coordinateWid + 5, zeroY), coordinate.tickness,
                themeInfo.axisLineColor);


            ChartUtils.DrawLine(vh, new Vector3(zeroX + coordinateWid, zeroY - coordinate.splitWidth),
                       new Vector3(zeroX + coordinateWid , zeroY + coordinateHig + 2), coordinate.tickness,
                       themeInfo.axisLineColor);
        }

        protected override void InitXScale() {
            foreach (var item in xScaleTextList) {
                item.text = string.Empty;
            }

            xScaleTextList.Clear();
            //float max = GetMaxValue();
            Vector2 v2 = GetMaxAndMinValue();
            float scaleWid = xAxis.GetScaleWidth(coordinateWid);
            Vector2 sizeDelta;
            HorizontalWrapMode horizontalWrap;
            VerticalWrapMode verticalWrap;
            int fontSize = 14;
            if (xAxis.horizontal) {
                sizeDelta = new Vector2(scaleWid, 20);
                horizontalWrap = HorizontalWrapMode.Overflow;
                verticalWrap = VerticalWrapMode.Overflow;
            } else {
                sizeDelta = new Vector2(fontSize * 1.5f, coordinate.xScaleRectHeight);
                horizontalWrap = HorizontalWrapMode.Wrap;
                verticalWrap = VerticalWrapMode.Truncate;
            }

            for (int i = 0; i < xAxis.GetSplitNumber(); i++) {
                Text txt = ChartUtils.AddTextObject("xScale" + i, transform, themeInfo.font,
                    themeInfo.textColor, TextAnchor.UpperCenter, Vector2.zero, Vector2.zero,
                  xAxis.horizontal ? new Vector2(1, 0.5f) : new Vector2(0.5f, 1),
                    sizeDelta, 14, horizontalWrap,verticalWrap);
                txt.transform.localPosition = GetXScalePosition(scaleWid, i, xAxis.horizontal, fontSize);

                txt.text = xAxis.GetScaleName(i, v2.x, v2.y);
                txt.gameObject.SetActive(coordinate.show);
                xScaleTextList.Add(txt);
            }
        }

        protected override void InitYScale() {
            if (seriesList.Count == 0) return; 
            foreach (var item in yScaleTextList) {
                item.text = string.Empty;
            }

            yScaleTextList.Clear();
            float max = GetMaxValue();
            float scaleWid = yAxis.GetScaleWidth(coordinateHig);
            for (int i = 0; i < yAxis.GetSplitNumber(); i++) {
                Text txt = ChartUtils.AddTextObject("yScale" + i, transform, themeInfo.font,
                        themeInfo.textColor, TextAnchor.MiddleRight, Vector2.zero, Vector2.zero,
                        new Vector2(1, 0.5f),
                        new Vector2(coordinate.left, 20));
                txt.transform.localPosition = GetYScalePosition(scaleWid, i);
                txt.text = yAxis.GetScaleName(i, max);
                txt.gameObject.SetActive(coordinate.show);
                yScaleTextList.Add(txt);
            }

            foreach (var item in rightYScaleTextList) {
                item.text = string.Empty;
            }

            rightYScaleTextList.Clear();
            Vector2 v2 = GetMaxAndMinValue();
            for (int i = 0; i < yAxis.GetSplitNumber(); i++) {
                Text txt = ChartUtils.AddTextObject("rightYScale" + i, transform, themeInfo.font,
                        themeInfo.textColor, TextAnchor.MiddleLeft, Vector2.zero, Vector2.zero,
                        new Vector2(1, 0.5f),
                        new Vector2(coordinate.left, 20));
                txt.transform.localPosition = GetRightYScalePosition(scaleWid, i);
                txt.text = yAxis.GetScaleName(i, v2.x, v2.y);
                txt.gameObject.SetActive(ShowRightScale&&coordinate.show);
                rightYScaleTextList.Add(txt);
            }
        }

        protected override void InitLegend() {
            base.InitLegend();
        }

        protected override void RefreshTooltip() {
            int index = tooltip.DataIndex - 1;
            Axis tempAxis = xAxis.type == AxisType.value ? (Axis)yAxis : (Axis)xAxis;
            if (index < 0) {
                tooltip.SetActive(false);
                return;
            }
            tooltip.SetActive(true);
            if (seriesList.Count == 1) {
                string txt = tempAxis.GetxLableData(index) + ": " + seriesList[0].DataList[index].ToString(Mode == ChartMode.Bar ? "N0":"N2");
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
                        /*barchart 暂时没有显示小数的需要 */
                        sb.AppendFormat("{0}: {1}", key, value.ToString(Mode == ChartMode.Bar ? "N0" : "N2"));
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

        protected override float GetMaxValue() {
            float max = 0;
            int count = ShowRightScale ? 1:seriesList.Count;
            for (int i = 0; i < count; i++) {
                if (legend.IsShowSeries(i) && seriesList[i].Max > max) max = seriesList[i].Max;
            }

            Vector2 v2 = ChartUtils.MagicNumbers(max, 0,true);
            return v2.x;
        }

        protected override Vector2 GetMaxAndMinValue() {
            float max = 0, min = 0;
            int ori= ShowRightScale ? 1 : 0;
            for (int i = ori; i < seriesList.Count; i++) {
                if (legend.IsShowSeries(i) && seriesList[i].Max > max) max = seriesList[i].Max;
                if (legend.IsShowSeries(i) && seriesList[i].Min < min) min = seriesList[i].Min;
            }

            Vector2 v2 = ChartUtils.MagicNumbers(max, min,true);
            return v2;
        }

        protected Vector3 GetRightYScalePosition(float scaleWid, int i) {
            if (yAxis.boundaryGap) {
                return new Vector3(zeroX + coordinate.splitWidth + coordinateWid + 40,
                    zeroY + (i + 0.5f) * scaleWid, 0);
            } else {
                return new Vector3(zeroX + coordinate.splitWidth + coordinateWid + 40,
                    zeroY + i * scaleWid, 0);
            }
        }

        public void AddSeriesData(List<KeyValuePair<string, Color>> seriesNameAndColors, List<SeriesData> seriesData) {
            ClearSeriesData();
            RefreshChart();
            legend.dataList = seriesNameAndColors.Select(item => item.Key).ToList();
            seriesList.Clear();
            Color32[] colors = new Color32[seriesNameAndColors.Count];
            for (int i = 0; i < seriesNameAndColors.Count; i++) {
                Series series = new Series();
                series.name = seriesNameAndColors[i].Key;
                seriesList.Add(series);
                colors[i] = seriesNameAndColors[i].Value;
            }
            foreach (var item in seriesNameAndColors) {
                Series series = new Series();
                series.name = item.Key;
                seriesList.Add(series);
            }
            foreach (var item in rightYScaleTextList) {
                item.gameObject.SetActive(ShowRightScale);
            }
            if (RightYUnit != null)
                RightYUnit.gameObject.SetActive(ShowRightScale);
            themeInfo.colorPalette = colors;
            base.AddSeriesData(seriesData);

        }
    }

    public enum ChartMode {
        Line,
        Bar,
        Mixed
    }
}