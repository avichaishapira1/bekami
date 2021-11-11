// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(function () {


    //Load the right table with cartitems and total price
    $('#insertHTML').load("/Cart/Table");

   //Make an Order button, active only if checkbox is checked!
    $("#Confirm").click(function () {
        $("#DetailsForm").submit();
       
    });



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //RunEffects code
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
