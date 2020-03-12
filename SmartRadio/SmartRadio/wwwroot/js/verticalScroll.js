var current_page = 1;
var max_pages = 0;
var menu_is_opened = true;
var navHeight = 0;
var scrollTop = false;

$(window).on("load", function () {

    navHeight = $("nav").height();
    var height = $(window).height() - navHeight;
//    $(".page-wrapper").css("height", height);
    $("#wrapping-div").removeClass("container");
    $("#scrollToTop").hide();
    $("main").removeClass("pb-3");

    max_pages = $(".page-wrapper").children().length;
    $(".page").bind("mousewheel", scrollToPage());
});

function scrollToPage() {
    return function (e) {
        $(".page").unbind();
        if (e.originalEvent.wheelDelta / 120 > 0) {
            current_page--;
            nextPage();
        } else {
            current_page++;
            nextPage();
        }
    };
}

function scrollArrow() {
    $(".page").unbind();
    current_page++;
    nextPage();
}

function goToPage(page) {
    $(".page").unbind();
    current_page = page;
    nextPage();
}

function nextPage() {
    let valid = validate();
    scrollTopButton();

    if (valid) {
        let nextPage = "#page" + current_page;

        $("body").removeProp("background");
        $("html").removeProp("background");

        let color;
        let backgroundColor;

        switch (current_page) {
            case 1:
                color = "transparent";
                backgroundColor = "linear-gradient(34deg, rgba(247,119,84,1) 0%, rgba(83,125,145,1) 49%, rgba(164,209,200,1) 100%)";
                break;
            case 2:
                color = "#f77754";
                backgroundColor = "linear-gradient(36deg, rgba(83,125,145,1) 0%, rgba(247,119,84,1) 100%)";
                break;
            case 3:
                color = "#537d91";
                break;
            case 4:
                color = "#a4d1c8";
                break;
            default:
                color = "transparent";
                break;
        }

        $('.page-wrapper').animate({
                scrollTop: "+=" + ($(nextPage).offset().top - navHeight)
            },
            800,
            'swing',
            function() {
                $(".page").bind("mousewheel", scrollToPage());
                $("nav").css({
                    "transition": "background-color 0.3s ease",
                    "backgroundColor": color
                });
//                $("html").css({
//                    "transition": "background 4s ease-in",
//                    "background": backgroundColor
//                });
//                console.log(backgroundColor);
//                $("body").css({
//                    "transition": "all 4s ease-in",
//                    "background": backgroundColor
//                });
            }
            );

    } else {
        $(".page").bind("mousewheel", scrollToPage());
    }
}

function validate() {
    if (current_page > max_pages) {
        current_page = max_pages;
        return false;
    } else if (current_page < 1) {
        current_page = 1;
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
    let backgroundColor = "linear-gradient(317deg, rgba(247,119,84,1) 0%, rgba(83,125,145,1) 49%, rgba(164,209,200,1) 100%)";
    $("nav").css("backgroundColor", "transparent");
    $("html").css({
        "transition": "background 4s ease-in",
        "background": backgroundColor
    });
    $("body").css({
        "transition": "all 4s ease-in",
        "background": backgroundColor
    });
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