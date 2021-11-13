
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
    //parse json to array
    function parsedata(data) {

        var arr = [];
        for (var k in data) {
            arr.push({
                Created: data[k].Created[k].month,
                Users: data[k]
            });
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
        console.log(data2);

        //creating the line
        var line = d3.line()
            .x(function (d) { return x(d.Created) })
            .y(function (d) { return y(d.Users) });
        x.domain(d3.extent(data2, function (d) { return d.Created }));
        y.domain(d3.extent(data2, function (d) { return d.Users }));

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

        //draw the line
        g.append("path")
            .datum(data2)
            .attr("fill", "none")
            .attr("stroke", "steelblue")
            .attr("stroke-linejoin", "round")
            .attr("stroke-linecap", "round")
            .attr("stroke-width", 1.5);
    }
});