$(function () {

    var form = $('#filterform');
    var url = form.attr('action');

    //filterbOX
    $('#filter').keyup(function () {
        $('table').fadeOut(300, function () {
            $('tbody').empty()
            $.ajax({
                url: url,
                data: form.serialize(),
                success: function (data) {
                    $('tbody').empty()
                    $('table').fadeIn(300);
                    $('#results').tmpl(data).appendTo('tbody');

                }
            });
        });
    });


    //prevent pressing enter so we dont go to jSON View
    $("#filter").keydown(function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
        }
    });



});
