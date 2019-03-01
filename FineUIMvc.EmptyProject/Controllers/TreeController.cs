using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FineUIMvc.EmptyProject.Controllers
{
    public class TreeController : Controller
    {
        // GET: Tree 页面框架初始化
        public ActionResult Index()
        {
            return View();
        }

        //ECharts示例1
        public ActionResult ECharts_1()
        {
            return View();
        } //ECharts表单

        public ActionResult echarts1_IFrame()  //ECharts页面
        {
            var barOptions = String.Empty;
            var paramType = Request.QueryString["type"];         
            
            if (String.IsNullOrEmpty(paramType) || paramType == "sales")
            {
                //{
                //    "title": {
                //        "text": "销量图表"
                //    },
                //    "xAxis": {
                //        "type": "category",
                //        "data": ["衬衫", "羊毛衫", "雪纺衫", "裤子", "高跟鞋", "袜子"]
                //    },
                //    "yAxis": {
                //        "type": "value"
                //    },
                //    "series": [{
                //        "name": "销量",
                //        "type": "bar",
                //        "data": [5.0, 20.0, 36.0, 10.0, 10.0, 20.0]
                //    }]
                //}
                barOptions = "{\"color\":[\"#5793f3\"],\"title\":{\"text\":\"销量图表\"},\"xAxis\":{\"type\":\"category\",\"data\":[\"衬衫\",\"羊毛衫\",\"雪纺衫\",\"裤子\",\"高跟鞋\",\"袜子\"]},\"yAxis\":{\"type\":\"value\"},\"series\":[{\"name\":\"销量\",\"type\":\"bar\",\"data\":[5.0,20.0,36.0,10.0,10.0,20.0]}]}";
            }
            else
            {
                //{
                //    "title": {
                //        "text": "产量图表"
                //    },
                //    "xAxis": {
                //        "type": "category",
                //        "data": ["衬衫", "羊毛衫", "雪纺衫", "裤子", "高跟鞋", "袜子"]
                //    },
                //    "yAxis": {
                //        "type": "value"
                //    },
                //    "series": [{
                //        "name": "销量",
                //        "type": "bar",
                //        "data": [6.0, 18.0, 17.0, 30.0, 20.0, 5.0]
                //    }]
                //}
                barOptions = "{\"color\":[\"#d14a61\"],\"title\":{\"text\":\"产量图表\"},\"xAxis\":{\"type\":\"category\",\"data\":[\"衬衫\",\"羊毛衫\",\"雪纺衫\",\"裤子\",\"高跟鞋\",\"袜子\"]},\"yAxis\":{\"type\":\"value\"},\"series\":[{\"name\":\"销量\",\"type\":\"bar\",\"data\":[6.0,18.0,17.0,35.0,20.0,5.0]}]}";
            }

            ViewBag.BarOptions = barOptions;

            return View();
        }

        public ActionResult ECharts_2()
        {

            return View();
        }
        public ActionResult ECharts2_IFrame()
        {
            var barOptions = String.Empty;

            //{
            //    "title": {
            //        "text": "销量图表"
            //    },
            //    "xAxis": {
            //        "type": "category",
            //        "data": ["衬衫", "羊毛衫", "雪纺衫", "裤子", "高跟鞋", "袜子"]
            //    },
            //    "yAxis": {
            //        "type": "value"
            //    },
            //    "series": [{
            //        "name": "销量",
            //        "type": "bar",
            //        "data": [5.0, 20.0, 36.0, 10.0, 10.0, 20.0]
            //    }]
            //}
            barOptions = "{" +
                "\"color\":[" +
                "\"#5793f3\"" +
                "]," +
                "\"title\":{" +
                "\"text\":\"销量图表\"" +
                "}," +
                "\"xAxis\":{" +
                "\"type\":\"category\"," +
                "\"data\":[\"衬衫\",\"羊毛衫\",\"雪纺衫\",\"裤子\",\"高跟鞋\",\"袜子\"]" +
                "}," +
                "\"yAxis\":{" +
                "\"type\":\"value\"" +
                "}," +
                "\"series\":[{" +
                "\"name\":\"销量\",\"type\":\"bar\",\"data\":[5.0,20.0,36.0,10.0,10.0,20.0]" +
                "}]" +
                "}";

            /*
                 xAxis: {
                    type: 'category',
                    data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
                },
                yAxis: {
                    type: 'value'
                },
                series: [{
                    data: [820, 932, 901, 934, 1290, 1330, 1320],
                    type: 'line',
                    smooth: true
                }]
             */
            //"\"title\":{" +
            //"\"text\":\"销量图表\"" +
            //"}," +
            //"\"xAxis\":{" +
            //"\"type\":\"category\"," +
            //"\"data\":[\"衬衫\",\"羊毛衫\",\"雪纺衫\",\"裤子\",\"高跟鞋\",\"袜子\"]" +
            //"}," +

            //barOptions = "{" +
            //    "\"xAxis\":{"+
            //    "\"type\":\"category","+
            //    "\"data\":[\"Mon\", \"Tue\", \"Wed\", \"Thu\", \"Fri\", \"Sat\", \"Sun\"]"+
            //    "}",+

            //    "}";
                

            ViewBag.BarOptions = barOptions;

            return View();
        } //将ECharts代码写到前端视图页面

        public ActionResult ECharts_3() //实例3：数据库交互
        {
            return View();
        }
        public ActionResult ECharts3_IFrame()
        {
            return View();
        }

    }
}