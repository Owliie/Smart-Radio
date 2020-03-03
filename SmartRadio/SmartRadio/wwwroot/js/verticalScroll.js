var current_page = 1;
var max_pages = 0;
var menu_is_opened = true;
var navHeight = 0;
var scrollTop = false;

$(window).on("load", function () {

    navHeight = $("nav").height();
    var height = $(window).height() - navHeight;
    $(".page-wrapper").css("height", height);

    $("#scrollToTop").hide();

    max_pages = $(".page-wrapper").children().length;
    $(".page").bind("mousewheel", scrollToPage());
});

function scrollToPage() {
    return function (e) {
        $(".page").unbind();
        if (e.originalEvent.wheelDelta / 120 > 0) {
            current_page--;
            console.log("prev page coming");
            nextPage();
        } else {
            current_page++;
            console.log("next page coming");
            nextPage();
        }
    };
}

function scrollArrow() {
    current_page++;
    nextPage();
}

function nextPage() {
    let valid = validate();
    scrollTopButton();

    if (valid) {
        let nextPage = "#page" + current_page;
        console.log("page scroll animation");
        $('.page-wrapper').animate({
                scrollTop: "+=" + ($(nextPage).offset().top - navHeight)
            },
            800,
            'swing',
            function() {
                $(".page").bind("mousewheel", scrollToPage());
            });
        console.log(nextPage + " " + $(nextPage).offset().top);
        console.log('scrolling up !');
    } else {
        $(".page").bind("mousewheel", scrollToPage());
    }
}

function validate() {
    if (current_page > max_pages) {
        current_page = max_pages;
        console.log("max pages reset");
        return false;
    } else if (current_page < 1) {
        current_page = 1;
        console.log("minimum page reached");
    }
    return true;
}

function scrollTopButton() {
    if (current_page === 1) {
        $("#scrollToTop").fadeOut();
        scrollTop = false;
    } else if (!scrollTop) {
        $("#scrollToTop").fadeIn();
        scrollTop = true;
    }
}

function scrollToHome() {
    $(".page").unbind();
    current_page = 1;

    console.log("top");
    $('.page-wrapper').animate({
        scrollTop: $("#page" + current_page).offset().top - navHeight
    }, 1000, 'swing', function () {
        $(".page").bind("mousewheel", scrollToPage());
    });

    scrollTopButton();
}