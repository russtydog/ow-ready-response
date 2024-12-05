var cbMaximized = false;
$(document).ready(function () {
    $(".chat-button").draggable().click(function() {
        $('.chat-button').hide();
            $('.chat-box').show();
    });
    $('#cb-close').on('click', function () {
        $('.chat-button').show();
        $('.chat-box').hide();
    });
    $('#cb-maximize').on('click', function () {
        if (cbMaximized === "true") {
            $('.chat-box').css({
                "width": "",
                "height": ""
            });
            cbMaximized = "false";
        }
        else {
            $('.chat-box').css({
                "width": "calc(100% - 35px)",
                "height": "calc(100% - 35px)"
            });
            cbMaximized = "true";
        }
    });
});