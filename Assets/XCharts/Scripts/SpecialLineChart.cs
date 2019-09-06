using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace xcharts {
    public class SpecialLineChart : LineChart {

        string[] content = new string[] { "断开", "闭合" };


        protected override Vector2 GetMaxAndMinValue() {
            return new Vector2(1, 0);
        }

        protected override void InitYScale() {
            InitYScaleLabel(content);
        }

        public void InitYScaleLabel(string[] content) {
            this.content = content;
            transform.Find("yScale0").GetComponent<Text>().text = content[0];
            transform.Find("yScale1").GetComponent<Text>().text = content[1];
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
                string strValue = seriesList[0].DataList[index] == 0 ? content[0] : content[1];
                string txt = tempAxis.GetData(index) + ": " + strValue;
                tooltip.UpdateTooltipText(txt);
            } else {
                StringBuilder sb = new StringBuilder(tempAxis.GetData(index));
                for (int i = 0; i < seriesList.Count; i++) {
                    string strColor = ColorUtility.ToHtmlStringRGBA(themeInfo.GetColor(i));
                    string key = seriesList[i].name;

                    if (index < seriesList[i].DataList.Count) {
                        float value = seriesList[i].DataList[index];
                        sb.Append("\n");
                        sb.AppendFormat("<color=#{0}>● </color>", strColor);
                        string strValue = value == 0 ? content[0] : content[1];
                        sb.AppendFormat("{0}: {1}", key, strValue);
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

        protected override void DrawChart(VertexHelper vh) {
            DrawCoordinate(vh);
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
                Vector3 mp1 = Vector3.zero;
                Vector3 mp2 = Vector3.zero;

                float startX = zeroX + (xAxis.boundaryGap ? scaleWid / 2 : 0);
                int showDataNumber = series.showDataNumber;
                int startIndex = 0;
                if (series.showDataNumber > 0 && series.DataList.Count > series.showDataNumber) {
                    startIndex = series.DataList.Count - series.showDataNumber;
                }
                lp = new Vector3(startX, zeroY);
                for (int i = startIndex; i < series.DataList.Count; i++) {
                    float value = series.DataList[i];
                    float preValue = i > 0 ? series.DataList[i - 1] : 0;
                 
                    float x = (lp.x + startX + i * scaleWid) * 0.5f;
                    if (value == 0) {
                        if (preValue == 0) {
                            mp1 = new Vector3(x, zeroY);
                            mp2 = new Vector3(x, zeroY);
                            np = new Vector3(startX + i * scaleWid, zeroY );
                        } else {
                            mp1 = new Vector3(x, zeroY + coordinateHig);
                            mp2 = new Vector3(x, zeroY );
                            np = new Vector3(startX + i * scaleWid, zeroY );
                        }
                    } else {
                        if (preValue == 0) {
                            mp1 = new Vector3(x, zeroY );
                            mp2 = new Vector3(x, zeroY + coordinateHig);
                            np = new Vector3(startX + i * scaleWid, zeroY + coordinateHig);
                        } else {
                            mp1 = new Vector3(x, zeroY + coordinateHig);
                            mp2 = new Vector3(x, zeroY + coordinateHig);
                            np = new Vector3(startX + i * scaleWid, zeroY +  coordinateHig);
                        }
                    }

                    //np = new Vector3(startX + i * scaleWid, zeroY + (value - minValue) * coordinateHig / max);
                    ChartUtils.DrawLine(vh, lp, mp1, lineInfo.tickness, color);
                    ChartUtils.DrawLine(vh, mp1, mp2, lineInfo.tickness, color);
                    ChartUtils.DrawLine(vh, mp2, np, lineInfo.tickness, color);
                    //} else {
                    //    np = new Vector3(startX + i * scaleWid, zeroY + (value - minValue) * coordinateHig / max);
                    //    ChartUtils.DrawLine(vh, lp, np, lineInfo.tickness, color);
                    //}
                    //np = new Vector3(startX + i * scaleWid, zeroY + (value - minValue) * coordinateHig / max);
                    //if (i > 0) {
                    //    if (lineInfo.smooth) {
                    //        var list = ChartUtils.GetBezierList(lp, np, lineInfo.smoothStyle);
                    //        Vector3 start, to;
                    //        start = list[0];
                    //        for (int k = 1; k < list.Length; k++) {
                    //            to = list[k];
                    //            ChartUtils.DrawLine(vh, start, to, lineInfo.tickness, color);
                    //            start = to;
                    //        }
                    //    } else {
                    //        ChartUtils.DrawLine(vh, lp, np, lineInfo.tickness, color);
                    //        if (lineInfo.area) {
                    //            ChartUtils.DrawPolygon(vh, lp, np, new Vector3(np.x, zeroY),
                    //                new Vector3(lp.x, zeroY), color);
                    //        }
                    //    }

                    //}
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
    }
}
