<%@ Page Title="PastPowerOutages" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PastPowerOutages.aspx.cs" Inherits="PowerInterruptions.PastPowerOutages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHead" runat="server">

    <!-- Styles to make your graphs work go here -->
    <link href="/Content/my-style-sheet.css" rel="stylesheet" />


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPBody" runat="server">

    <!-- Your HTML and JavaScript goes here -->
    <div id="tableData">
  
    </div>

    <div id="graphData">

    </div>


    <script>
        // setup bootstrap rows and columns
        $('#tableData').addClass('col-lg-4 col-sm-12');
        $('#graphData').addClass('col-lg-7 col-sm-10');
        $('div.background').wrapInner($('<div class="row">'));

        // create color object for holding different levlels of color
        var colorObject = {
            shadeOne: "#e6f5ff",
            shadeTwo: "#b3e0ff",
            shadeThree: "#80ccff",
            shadeFour: "#4db8ff",
            shadeFive: "#1aa3ff",
            shadeSix: "#008ae6",
            shadeSeven: "#006bb3",
            shadeEight: "#004d80",
            shadeNine: "#002e4d",
            shadeTen: "#000f1a"
        };
        // boolean flags to avoid reconstructing elements
        var doneTable = false;
        var doneYLabel = false;

        start();
        function start() {
            // Ajax for retrieving annual data
            $.ajax({
                // settings used for ajax response...
                url: "/api/AnnualPowerInterruptions",       // location of data being requested
                method: "GET",                              // method used to grab data
                dataType: "json",                           // the type of data coming back from the server
                timeout: 2000,                              // time allowed for request wait period
                success: buildPage,                         // call back function (called buildPage) for when data is successfully obtained
                error: error                                // call back function (called error) for when data is unsuccessfully obtained
            });

            function buildPage(data) {
                if (doneTable == false) {

                    // TABLE ----------------------------------------------------------------------------------------------------------------------

                    // create empty table 
                    var $table = $('<table>').attr('id', 'interruptionTable').addClass('table');
                    var $tableHead = $('<thead>').appendTo($table);
                    var $firstRow = $('<tr>')
                        .append('<th>Year')
                        .append('<th>Events')
                        .append('<th>Customers')
                        .append('<th>Duration')
                        .appendTo($tableHead);
                    var $tableBody = $('<tbody>').appendTo($table);
                    $table.appendTo($('#tableData'));

                    // loop through each object in array and append to table
                    $.each(data, function (index, interruption) {
                        var $newRow = $('<tr>').attr('id', 'row' + index).appendTo($tableBody);
                        $('<th>').text(interruption.year).appendTo($newRow);
                        $('<td>').text(interruption.totalEvents).appendTo($newRow);
                        $('<td>').text(interruption.customers).appendTo($newRow);
                        $('<td>').text(interruption.avgDuration).appendTo($newRow);
                    });

                    // highlight year of row hovered over in different colour 
                    var $parentTable = $('table tbody');
                    $parentTable.on('mouseenter mouseleave', 'tr', function () {
                        // get index of row being hovered over
                        var $rowIndex = $('table tbody tr').index(this);
                        $('table tbody tr[id = row' + $rowIndex + '] th').toggleClass('highlightYear');
                    })
                }
                // change boolean flag upon completing construction of table
                doneTable = true;

                // END TABLE ------------------------------------------------------------------------------------------------------------------

                // GRAPHS ---------------------------------------------------------------------------------------------------------------------

                $('#graphData').empty();
                $('#button').remove();

                // create y label - only if it's doesn't already exist
                if (doneYLabel == false) {
                    var $yearDiv = $('<div id="yLabel" class="col-lg-1 col-sm-2">').insertBefore($('#graphData'));
                    $('<h4>').text('Year').appendTo($yearDiv);
                    doneYLabel = true;
                }

                // reset some elements to their default settings
                $('#yLabel').css('margin-top', '116px');
                $('#yLabel h4').text('Year');
                clearTableHighlight();

                // create container for graph data
                var $graphContainer = $('<div class="bar-container">').appendTo($('#graphData'));
                // label x axis of graph
                var $xLabel = $('<h4 id="xLabel">').text('Customers Effected');
                $('<div>').append($xLabel).appendTo($('#graphData'));

                // variable for holding object with the greatest number of customers affected
                var count = 0;
                // search for highest customer count
                $.each(data, function (index, interruption) {
                    // set new count value
                    if (interruption.customers > count) {
                        count = interruption.customers
                    }
                })

                // loop through each object in array and append to graph
                $.each(data, function (index, interruption) {
                    // calculate how wide bar needs to be
                    var $percentageOfHighest = (interruption.customers / count) * 100;
                    var $barWidth = parseInt($percentageOfHighest);
                    // add another y axis element to show bar/year relationship
                    $('<span class="secondYLabel">').text(interruption.year).appendTo('.bar-container');
                    // add new bar with a width corresponding to it's comparison with other years (in %)
                    var $newBar = $('<div class="bar firstView">').appendTo('.bar-container');
                    var $barTotal = $('<p class="barTotal">').text(interruption.customers).appendTo($newBar);
                    // animate bar from left to right (2 sec duration)
                    $newBar.animate({ 'width': $barWidth + '%' }, 2000);
                    // determine what colour bar should be with respect to its width
                    if ($barWidth >= 90) {
                        $newBar.css('background-color', colorObject.shadeTen);
                    } else if ($barWidth >= 80 && $barWidth < 90) {
                        $newBar.css('background-color', colorObject.shadeNine);
                    } else if ($barWidth >= 70 && $barWidth < 80) {
                        $newBar.css('background-color', colorObject.shadeEight);
                    } else if ($barWidth >= 60 && $barWidth < 70) {
                        $newBar.css('background-color', colorObject.shadeSeven);
                    } else if ($barWidth >= 50 && $barWidth < 60) {
                        $newBar.css('background-color', colorObject.shadeSix);
                    } else if ($barWidth >= 40 && $barWidth < 50) {
                        $newBar.css('background-color', colorObject.shadeFive);
                    } else if ($barWidth >= 30 && $barWidth < 40) {
                        $newBar.css('background-color', colorObject.shadeFour);
                    } else if ($barWidth >= 20 && $barWidth < 30) {
                        $newBar.css('background-color', colorObject.shadeThree);
                    } else if ($barWidth >= 10 && $barWidth < 20) {
                        $newBar.css('background-color', colorObject.shadeTwo);
                    } else {
                        $newBar.css('background-color', colorObject.shadeOne);
                    }
                });

                // add event delegate for listening to bar click
                $('#graphData .bar-container').on('click', '.firstView', function (e) {
                    clearTableHighlight();
                    $('#errorMsg').remove();
                    // get index position of bar clicked (e.g. 0 for 2012 bar)
                    var index = $(this).parent().children('div').index(this);
                    // test which ajax uri to call
                    if (index == 0) {
                        var ajaxURL = '/api/MonthlyPowerInterruptions/2012/';
                        $('#row0').css({'outline-color' : '#fff321', 'outline-style' : 'groove'});
                        secondAjax(ajaxURL);
                    } else if (index == 1) {
                        var ajaxURL = '/api/MonthlyPowerInterruptions/2013/';
                        $('#row1').css({ 'outline-color': '#fff321', 'outline-style': 'groove' });
                        secondAjax(ajaxURL);
                    } else if (index == 2) {
                        var ajaxURL = '/api/MonthlyPowerInterruptions/2014/';
                        $('#row2').css({ 'outline-color': '#fff321', 'outline-style': 'groove' });
                        secondAjax(ajaxURL);
                    } else if (index == 3) {
                        var ajaxURL = '/api/MonthlyPowerInterruptions/2015/';
                        $('#row3').css({ 'outline-color': '#fff321', 'outline-style': 'groove' });
                        secondAjax(ajaxURL);
                    } else if (index == 4) {
                        var ajaxURL = '/api/MonthlyPowerInterruptions/2016/';
                        $('#row4').css({ 'outline-color': '#fff321', 'outline-style': 'groove' });
                        secondAjax(ajaxURL);
                        
                    }
                })

                // END GRAPHS -----------------------------------------------------------------------------------------------------------------
            }

            // function for unsuccessful retrieval of annual data 
            function error(data) {
                // change formatting to make room for error message
                $('#tableData').toggleClass('col-lg-4 col-lg-12');
                $('#tableData').append('<h3 id="errorMsg">Failed to load yearly Power Interruption Data');
            }
        }

        function secondAjax(ajaxURL) {

            // Ajax for retrieving monthly data
            $.ajax({
                // settings used for ajax response...
                url: ajaxURL,
                method: "GET",
                dataType: "json",
                timeout: 2000,
                success: buildMonthData,
                error: error
            });

            // variable for holding the greatest number of customers affected in a MONTH
            var count = 0;
            // loop through months to determine greatest customers disruption count 
            function buildMonthData(data) {

                // determine the highest number of customers disrupted in a month from a predetermined year
                $.each(data, function (index, interruption) {
                    if (interruption.customers > count) {
                        count = interruption.customers;
                    }
                });

                // change yLabel 
                var $newYLabel = $('#yLabel h4').text('Month');
                $('#yLabel').css('margin-top', '250px');

                // create and add button
                var $divButton = $('<div class="text-center" id="button">').appendTo($('#tableData'))
                $('<button type="button" class="btn btn-primary">').text('Return to Annual Data').appendTo($divButton);

                // empty contents of old graph
                var $newGraph = $('.bar-container').empty();                                                                                        // may not need this

                // create new graph
                $.each(data, function (index, interruption) {
                    var $percentageOfHighest = (interruption.customers / count) * 100;
                    var $barWidth = parseInt($percentageOfHighest);
                    $('<span class="secondYLabel">').text(interruption.monthName).appendTo($('.bar-container'));
                    var $newBar = $('<div class="bar">').css('margin-left', '80px').appendTo('.bar-container');
                    // don't show bar total (i.e customer count) if it's zero
                    if (interruption.customers != 0) {
                        var $barTotal = $('<p>').text(interruption.customers).addClass('barTotal').appendTo($newBar);
                    }
                    $newBar.animate({ 'width': $barWidth + '%' }, 3000);
                    // color bars according to their width
                    if ($barWidth >= 90) {
                        $newBar.css('background-color', colorObject.shadeTen);
                    } else if ($barWidth >= 80 && $barWidth < 90) {
                        $newBar.css('background-color', colorObject.shadeNine);
                    } else if ($barWidth >= 70 && $barWidth < 80) {
                        $newBar.css('background-color', colorObject.shadeEight);
                    } else if ($barWidth >= 60 && $barWidth < 70) {
                        $newBar.css('background-color', colorObject.shadeSeven);
                    } else if ($barWidth >= 50 && $barWidth < 60) {
                        $newBar.css('background-color', colorObject.shadeSix);
                    } else if ($barWidth >= 40 && $barWidth < 50) {
                        $newBar.css('background-color', colorObject.shadeFive);
                    } else if ($barWidth >= 30 && $barWidth < 40) {
                        $newBar.css('background-color', colorObject.shadeFour);
                    } else if ($barWidth >= 20 && $barWidth < 30) {
                        $newBar.css('background-color', colorObject.shadeThree);
                    } else if ($barWidth >= 10 && $barWidth < 20) {
                        $newBar.css('background-color', colorObject.shadeTwo);
                    } else {
                        $newBar.css('background-color', colorObject.shadeOne);
                    }
                });

                // event listener for returning the annual graph data
                $('#tableData').on('click', 'button', function () {
                    start();
                })
            }

            function error(data) {
                $("#tableData").append('<h3 id="errorMsg">Failed to load monthly Power Interruption Data');
            }

        }

        // clear any highlight of rows in table after user clicks on bar
        function clearTableHighlight() {
            $('#row0').css({ 'outline-color': 'initial', 'outline-style': 'none' });
            $('#row1').css({ 'outline-color': 'initial', 'outline-style': 'none' });
            $('#row2').css({ 'outline-color': 'initial', 'outline-style': 'none' });
            $('#row3').css({ 'outline-color': 'initial', 'outline-style': 'none' });
            $('#row4').css({ 'outline-color': 'initial', 'outline-style': 'none' });
        }

    </script>

</asp:Content>
