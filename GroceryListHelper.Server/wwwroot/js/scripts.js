function getToken() {
    const input = document.querySelector("[name='__RequestVerificationToken']");
    const token = input.value;
    return token;
}