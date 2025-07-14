// fucntion to autofocus on the input field on the page load / component reload
window.focusElementById = function (elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        el.focus();
    }
};