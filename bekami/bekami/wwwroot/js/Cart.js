

$(function ()
{

    $(".mdb-select").on('change', function () {  //the dropbox select
        var cartItemId = $(this).attr("id"); //get cartItem Id
        var quantity = $(this).val(); //get selected quantity
        var result = "tr." + cartItemId;
        if (quantity == 0) {
            updateQuantity(cartItemId, quantity); //sends quantity to server
            runEffect(result, "drop");      
        }

        else {
            updateQuantity(cartItemId, quantity); //sends quantity to server
            ShowEffect(result, "highlight");
        }

        
    });

    //send to server the quantity
    function updateQuantity(cartItemId, quantity) {
        $.ajax({
            url: '/Cart/UpdateItemQuantity',
            data: { cartItemId: cartItemId, quantity: quantity },
            success: function (data) {

                var message = "Sorry it doesn't exists";
                if (data == message)
                    alert("You can't delete it, because it doesn't exsits");

                var jqxhr = $.get("/Cart/GetNumOfItems")
                    .done(function (data) {
                        if (data == 0) {
                            location.reload();
                        }
                        $('.shopping-card').addClass('change').attr('data-content', data);
                    })
                    .fail(function () {
                        $('.shopping-card').addClass('change').attr('data-content', "Error");
                    });
                
            },


            error: function () {
                alert("error");
            }

        });
    }








    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //RunEffects code
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    function runEffect(ExplodingArea, effectType) {
        // get effect type from
        var selectedEffect = effectType;
        // Most effect types need no options passed by default
        var options = {};
        // some effects have required parameters
        if (selectedEffect === "scale") {
            options = { percent: 50 };
        } else if (selectedEffect === "size") {
            options = { to: { width: 200, height: 60 } };
        }

        // Run the effect
        $(ExplodingArea).toggle(selectedEffect, options, 1000);
    };


    //used for show
    function ShowEffect(Area, effectType) {
        // get effect type from
        var selectedEffect = effectType;

        // Most effect types need no options passed by default
        var options = {};
        // some effects have required parameters
        if (selectedEffect === "scale") {
            options = { percent: 50 };
        } else if (selectedEffect === "transfer") {
            options = { to: "#button", className: "ui-effects-transfer" };
        } else if (selectedEffect === "size") {
            options = { to: { width: 200, height: 60 } };
        }

        // Run the effect
        $(Area).effect(selectedEffect, options, 500, callback);
    };

    // Callback function to bring a hidden box back
    function callback() {
        setTimeout(function () {
            $(Area).removeAttr("style").hide().fadeIn();
        }, 1000);
    };


});








