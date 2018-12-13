// Scroll page to the top if the server info on list is clicked

$("a.submit-link").click(function() {
    $("html, body").animate({ scrollTop: 0 }, "fast");
    return false;
});

$(".submit-link").click(function () {
    $(this).closest("form").submit();
});