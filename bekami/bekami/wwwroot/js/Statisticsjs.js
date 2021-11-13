
$(function () {
    //gets the json from the controller : 
    $.ajax({
        url: '/orders/Usersinmonth',
        data: {},
        success: function (data) {
            var parse = parsedata(data);
            drawChart(parse);
        },
        error: function () {
            alert("can't create users in month chart")
        }
    });

    $.ajax({
        url: '/orders/Ordersinmonth',
        data: {},
        success: function (data) {
            var parse = parsedata(data);
            drawChart2(parse);
        },
        error: function () {
            alert("can't create Orders in month")
        }
    });




    //parse json to array
    function parsedata(data) {

        var arr = [];
        for (var k in data) {
            arr.push(data[k]);
        }
        return arr;
    }

    function drawChart(data2)
    {
        var svgWidth = 600, svgHeight = 400;
        var margin = { top: 20, right: 20, bottom: 30, left: 50 };
        var width = svgWidth - margin.left - margin.right;
        var height = svgHeight - margin.top - margin.bottom;
        var svg = d3.select('svg')
            .attr("width", svgWidth)
            .attr("height", svgHeight);
        // a group element to hold our line chart
        var g = svg.append("g")
            .attr("transform",
                "translate(" + margin.left + "," + margin.top + ")"
        );

        //scalas
        var x = d3.scaleLinear().rangeRound([0, width]);
        var y = d3.scaleLinear().rangeRound([height, 0]);


        //creating the line
        var line = d3.line()
            .x(function (d) { return x(d.created) })
            .y(function (d) { return y(d.users) });
        x.domain([0,12]);
        y.domain(d3.extent(data2, function (d) { return d.users }));

        //x - scale 
        g.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x))
            .select(".domain");

       //y-scale
        g.append("g")
            .call(d3.axisLeft(y))
            .append("text")
            .attr("fill", "#000")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", "0.71em")
            .attr("text-anchor", "end")
            .text("registered users");

        //draw  line
        g.append("path")
            .datum(data2)
            .attr("fill", "none")
            .attr("stroke", "steelblue")
            .attr("stroke-linejoin", "round")
            .attr("stroke-linecap", "round")
            .attr("stroke-width", 1.5)
            .attr("d", line);
    }


    function drawChart2(data2) {
        var svgWidth = 600, svgHeight = 400;
        var margin = { top: 20, right: 20, bottom: 30, left: 50 };
        var width = svgWidth - margin.left - margin.right;
        var height = svgHeight - margin.top - margin.bottom;
        var svg = d3.select('svg2')
            .attr("width", svgWidth)
            .attr("height", svgHeight);
        // a group element to hold our line chart
        var g = svg.append("g")
            .attr("transform",
                "translate(" + margin.left + "," + margin.top + ")"
            );

        //scalas
        var x = d3.scaleLinear().rangeRound([0, width]);
        var y = d3.scaleLinear().rangeRound([height, 0]);


        //creating the line
        var line = d3.line()
            .x(function (d) { return x(d.created) })
            .y(function (d) { return y(d.orders) });
        x.domain([0, 12]);
        y.domain(d3.extent(data2, function (d) { return d.orders }));

        //x - scale 
        g.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x))
            .select(".domain");
        console.log(data2);

        //y-scale
        g.append("g")
            .call(d3.axisLeft(y))
            .append("text")
            .attr("fill", "#000")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", "0.71em")
            .attr("text-anchor", "end")
            .text("Orders");

        //draw  line
        g.append("path")
            .datum(data2)
            .attr("fill", "none")
            .attr("stroke", "steelblue")
            .attr("stroke-linejoin", "round")
            .attr("stroke-linecap", "round")
            .attr("stroke-width", 1.5)
            .attr("d", line);
    }




});