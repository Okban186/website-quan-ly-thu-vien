function getAntiForgeryToken() {
    return document.querySelector(
        'input[name="__RequestVerificationToken"]'
    )?.value;
}