const accessibilitySwitch = document.getElementById("accessibilitySwitch");

if (accessibilitySwitch) {
    initTheme();

    accessibilitySwitch.addEventListener("change", function (event) {
        resetTheme();
    });

    function initTheme() {
        const accessibilityToggled =
            localStorage.getItem("accessibilityMode") !== null &&
            localStorage.getItem("accessibilityMode") === "on";
        accessibilitySwitch.checked = accessibilityToggled;
        accessibilityToggled
            ? document.body.setAttribute("accessibility", "on")
            : document.body.removeAttribute("accessibility");
    }

    function resetTheme() {
        if (accessibilitySwitch.checked) {
            document.body.setAttribute("accessibility", "on");
            localStorage.setItem("accessibilityMode", "on");
        } else {
            document.body.removeAttribute("accessibility");
            localStorage.removeItem("accessibilityMode");
        }
    }
}

document.querySelector(".accessibility").onkeypress = function (e) {
    if (e.keyCode == 13) {
        e.preventDefault();
        accessibilitySwitch.click();
    }
};
