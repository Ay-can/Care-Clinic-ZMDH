// Password show/hide switch
// Set the needed constants.
const inputPassword = document.querySelector(".input-password");

// Check if password input field is on the page.
if (inputPassword) {
    // Detect click on the password toggle icon.
    document.querySelector(".toggle-password").onclick = function () {
        // Switches between password and text type input field depending on it's value.
        inputPassword.setAttribute(
            "type",
            inputPassword.getAttribute("type") === "password" ? "text" : "password"
        );
        // Switches the icon between different ones depending on its current icon.
        this.innerHTML === "visibility_off"
            ? (this.innerHTML = "visibility")
            : (this.innerHTML = "visibility_off");
    };
}

// Load icon
document.onreadystatechange = function () {
    $("body").css("overflow", "hidden");
    const state = document.readyState;
    if (state === "interactive") {
        document.querySelector("main").style.visibility = "hidden";
    } else if (state === "complete") {
        setTimeout(function () {
            $("body").css("overflow", "visible");
            // document.getElementById("interactive");
            document.querySelector(".load").style.visibility = "hidden";
            document.querySelector("main").style.visibility = "visible";
        }, 100);
    }
};

// Smooth scrolling button
$('a[href*="#"]').not('[href="#"]').not('[href="#0"]').click(function (event) {
    if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '') && location.hostname === this.hostname) {
        let target = $(this.hash);
        target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
        if (target.length) {
            event.preventDefault();
            $('html, body').animate({ scrollTop: target.offset().top - 64 }, 500);
        }
    }
});
