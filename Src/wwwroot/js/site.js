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
