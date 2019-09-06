using System;
using System.Collections.Generic;
using UnityEngine;
using xcharts;

public class LineAndBarDemo : MonoBehaviour {

    private MixedLineAndBarChart lineAndBarChart;

    void Awake() {
        lineAndBarChart = GetComponent<MixedLineAndBarChart>();
        lineAndBarChart.OnEnter = code => { Debug.Log("enter:" + code); };
        lineAndBarChart.OnExit = code => { Debug.Log("exit:" + code); };
        lineAndBarChart.UpdateTheme(Theme.Light);
        GenerateData(24, lineAndBarChart);
    }

    void GenerateData(int count, MixedLineAndBarChart chart) {
        List<SeriesData> seriesData = new List<SeriesData>();

        var baseValue = UnityEngine.Random.Range(0, 1000);
        var time = new DateTime(2011, 1, 1);
        var smallBaseValue = 0;
        int temp = 3;
        //List<string> seriseNameList = new List<string>() { "2019年", "2018年"};
        List<string> seriseNameList = new List<string>() { "2019年", "2018年", "2017年" };
        List<KeyValuePair<string, Color>> templist = new List<KeyValuePair<string, Color>>();
        templist.Add(new KeyValuePair<string, Color>("2019年",Color.blue));
        templist.Add(new KeyValuePair<string, Color>("2018年", Color.red));
        templist.Add(new KeyValuePair<string, Color>("2017年", Color.yellow));
        for (var i = 0; i < count; i++) {
            string code = time.ToString("yyyy/MM/dd");
            string lable = time.ToString("MM月dd");
            float[] value = new float[temp];

            for (int j = 0; j < temp; j++) {
                smallBaseValue = i % 30 == 0
                     ? UnityEngine.Random.Range(0, 700)
                     : (smallBaseValue + UnityEngine.Random.Range(0, 500) - 250);

                baseValue += UnityEngine.Random.Range(0, 20) - 10;

                value[j] = Mathf.Max(0, Mathf.Round(baseValue + smallBaseValue) + UnityEngine.Random.Range(1000, 3000));
                value[j] = Mathf.Abs(value[j]);
            }

            seriesData.Add(new SeriesData() { code = code, label = lable, value = value });
            time = time.AddDays(1);
        }

        lineAndBarChart.ClearSeriesData();
        lineAndBarChart.ShowRightScale = false;
        lineAndBarChart.AddSeriesData(templist, seriesData);
    }
}
