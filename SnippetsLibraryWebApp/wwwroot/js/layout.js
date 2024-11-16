// wwwroot/js/layout.js

$(document).ready(function () {
    // Initialize user state based on localStorage
    var isAuthenticated = localStorage.getItem("activeUserId") ? true : false;
    var activeUserName = localStorage.getItem("activeUserName") || '';

    updateAuthUI(isAuthenticated, activeUserName);

    // Function to update UI based on authentication state
    function updateAuthUI(authenticated, username) {
        if (authenticated) {
            $('.nav-item-login, .nav-item-register').hide();
            $('.nav-item-add-snippet, .nav-item-user').show();
            $('#userDropdown').text(username);
        } else {
            $('.nav-item-login, .nav-item-register').show();
            $('.nav-item-add-snippet, .nav-item-user').hide();
            $('#userDropdown').text('');
        }
    }

    // Function to show login modal
    $('#openLoginModal').on('click', function () {
        $('#loginModal').fadeIn();
    });

    // Function to show register modal
    $('#openRegisterModal').on('click', function () {
        $('#registerModal').fadeIn();
    });

    // Logout functionality
    $('#logout').on('click', function (e) {
        e.preventDefault();
        localStorage.removeItem("activeUserId");
        localStorage.removeItem("activeUserName");
        updateAuthUI(false, '');
    });
});
