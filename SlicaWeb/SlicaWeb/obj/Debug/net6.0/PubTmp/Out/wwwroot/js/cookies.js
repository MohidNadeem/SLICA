// cookies.js

window.cookies = {
    setCookie: function (name, value) {
        var expiryDate = new Date();
        expiryDate.setDate(expiryDate.getDate() + 7); // Set expiry date to 7 days from now

        var expires = "expires=" + expiryDate.toUTCString();
        document.cookie = name + "=" + value + ";path=/;" + expires;
    },

    getCookie: function (name) {
        var nameEQ = name + "=";
        var cookies = document.cookie.split(';');
        for (var i = 0; i < cookies.length; i++) {
            var cookie = cookies[i];
            while (cookie.charAt(0) === ' ') {
                cookie = cookie.substring(1, cookie.length);
            }
            if (cookie.indexOf(nameEQ) === 0) {
                return cookie.substring(nameEQ.length, cookie.length);
            }
        }
        return null;
    },

    deleteCookie: function (name) {
        document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:01 GMT;path=/';
    }
};
