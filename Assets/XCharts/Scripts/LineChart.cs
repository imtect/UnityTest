using UnityEngine;
using UnityEngine.UI;

namespace xcharts {
    [System.Serializable]
    public class LineInfo {
        public float tickness = 0.8f;

        [Header("Point")]
        public bool showPoint = true;
        public float pointWid = 2.5f;

        [Header("Smooth")]
        public bool smooth = false;

        [Range(1f, 10f)]
        public float smoothStyle = 2f;

        [Header("Area")]
        public bool area = false;
        public Color areaColor;
    }

    public class LineChart : BaseAxesChart {
        [SerializeField]
        protected LineInfo lineInfo;

        public float upper = float.NaN;
        public float lower = float.NaN;

        protected override void Awake() {
            base.Awake();
            if (ResetTheme)
                themeInfo = ThemeInfo.MtLineStyle;
        }

        protected override void Update() {
            base.Update();
        }

        protected override void DrawChart(VertexHelper vh) {
            base.DrawChart(vh);
            int seriesCount = seriesList.Count;

            Vector2 v2 = GetMaxAndMinValue();
            float max = v2.x - v2.y;
            //float max = GetMaxValue();

            float minValue = v2.y;

            //draw tooltip line
            if (tooltip.show && tooltip.DataIndex > 0) {
                float splitWid = coordinateWid / (xAxis.GetDataNumber() - 1);
                float px = zeroX + (tooltip.DataIndex - 1) * splitWid + (xAxis.boundaryGap ? splitWid / 2 : 0);
                Vector2 sp = new Vector2(px, zeroY);
                Vector2 ep = new Vector2(px, zeroY + coordinateHig);
                ChartUtils.DrawLine(vh, sp, ep, coordinate.tickness, themeInfo.tooltipFlagAreaColor);
            }

            DrawLine(vh, upper, new Color32(13, 217, 157, 255));
            DrawLine(vh, lower, new Color32(246, 72, 82, 255));

            float scaleWid = xAxis.GetDataWidth(coordinateWid);
            for (int j = 0; j < seriesCount; j++) {
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
                    for (int i = 0; i < series.DataList.Count; i++) {
                        float value = series.DataList[i];

                        Vector3 p = new Vector3(startX + i * scaleWid,
                            zeroY + (value - minValue) * coordinateHig / max);
                        float pointWid = lineInfo.pointWid;
                        if (tooltip.show && i == tooltip.DataIndex - 1) {
                            pointWid = pointWid * 5f;
                        }
                        if (theme == Theme.Dark) {

                            ChartUtils.DrawCricle(vh, p, pointWid, color,
                                (int)lineInfo.pointWid * 5);
                        } else {
                            ChartUtils.DrawCricle(vh, p, pointWid, color);
                            //ChartUtils.DrawDoughnut(vh, p, pointWid - lineInfo.tickness,
                                //pointWid, 0, 360, color);
                        }
                    }
                }                
            }         
        }

        protected override Vector2 GetMaxAndMinValue() {
            float max = 0, min = 0;
            for (int i = 0; i < seriesList.Count; i++) {
                if (legend.IsShowSeries(i) && seriesList[i].Max > max) max = seriesList[i].Max;
                if (legend.IsShowSeries(i) && seriesList[i].Min < min) min = seriesList[i].Min;
            }

            if (!float.IsNaN(upper)) {
                max = upper > max ? upper : max;
            }
            if (!float.IsNaN(lower)) {
                min = lower < min ? lower : min;
            }

            Vector2 v2 = ChartUtils.MagicNumbers(max, min);
            //if (v2.x < 5) v2.x = 5;
            //if (v2.y < 0 && v2.y > -5) v2.y = -5;
            return v2;
        }

        public void DrawLine(VertexHelper vh,float value,Color color) {
            float y = GetPostionY(value);
            float scaleWid = xAxis.GetDataWidth(coordinateWid);
            float startX = zeroX + (xAxis.boundaryGap ? scaleWid / 2 : 0);
            Vector3 start = new Vector3(startX + 0 * scaleWid, y);
            Vector3 to = new Vector3(startX + (seriesList[0].DataList.Count -1) * scaleWid, y);
            ChartUtils.DrawLine(vh, start, to, lineInfo.tickness * 0.5f, color);
        }

        public float GetPostionY(float value) {
            Vector2 v2 = GetMaxAndMinValue();
            float max = v2.x - v2.y;
            float minValue = v2.y;
            return zeroY + (value - minValue) * coordinateHig / max;
        }
    }
}
