using System;
using UnityEngine;
using xcharts;

public class BarChartTest : MonoBehaviour {

    private BarChart barChart;

    void Awake() {
        barChart = GetComponent<BarChart>();
        GenerateData(10, barChart);
    }

    void GenerateData(int count, BarChart chart) {
        var baseValue = UnityEngine.Random.Range(0, 1000);
        var time = new DateTime(2011, 1, 1);
        var smallBaseValue = 0;


        for (var i = 0; i < count; i++) {
            chart.XAxis.AddMultiData(time.ToString("yyyy/MM/dd"));

            for (int j = 0; j < 2; j++) {
                smallBaseValue = i % 30 == 0
                     ? UnityEngine.Random.Range(0, 700)
                     : (smallBaseValue + UnityEngine.Random.Range(0, 500) - 250);

                baseValue += UnityEngine.Random.Range(0, 20) - 10;
                float value = Mathf.Max(0, Mathf.Round(baseValue + smallBaseValue) + UnityEngine.Random.Range(1000, 3000));
                value = Mathf.Abs(value);
                chart.AddMultiData(j, value);
            }

            time = time.AddDays(1);
        }
    }
}
