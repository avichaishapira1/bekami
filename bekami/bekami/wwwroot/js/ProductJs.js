$(function () {
    //vars to keep reviews we loaded 5 reviews
    var skipCount = 0;
    var takeCount = 6;
    var hasMoreRecords = true;
    var color = 0;
    var category = 0;
    var sort = "newest";
    showProducts();



    function showProducts() {
        $.ajax({
            url: '/Products/getProducts',
            data: { colorId: color, categoryId: category, sort: sort, skipCount: skipCount, takeCount: takeCount },
            success: function (data) {
                if (data == 0) {
                    hasMoreRecords = false; // signal no more records to display
                    $('#ShowMoreProducts').text("No More Products To Show").css('background-color', 'red');
                }
                else {
                    $('#resultsProdcuts').tmpl(data).appendTo('#productInsert').hide().fadeIn(1000);
                    skipCount += takeCount; // update for next iteration
                    $("#showMoreButton").show(); //button after turning red and reload get hide() so need to show him again after reload
                }
            },
            error: function () {
                alert("error");
            }
        });
    }


    //show more products button
    $(document).on("click", '#ShowMoreProducts', function (event) {
        $([document.documentElement, document.body]).animate({
            scrollTop: $("#resultsProdcuts").offset().top
        }, 400);
        showProducts();
    });




    $(".select").on('change', function () {  //the dropbox select

        color = $("#ColorId").val();
        category = $("#CategoryId").val();
        sort = $("#Sort").val();


        skipCount = 0;
        takeCount = 6;
        hasMoreRecords = true;
        $("#showMoreButton").hide(); //remove button because might be red and jumpy
        $(".col-xl-4").remove(); //remove other products
        showProducts();
        $("#showMoreButton").load(window.location.href + " #showMoreButton");
    });


});







