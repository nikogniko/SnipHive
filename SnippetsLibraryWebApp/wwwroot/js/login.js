// wwwroot/js/login.js

$(document).ready(function () {

    // Function to close the login modal
    $('.close-login-modal').on('click', function () {
        $('#loginModal').fadeOut();
        resetLoginForm();
    });

    // Function to toggle password visibility
    $('.toggle-login-password').on('click', function () {
        var loginInput = $('#loginPassword');
        var type = loginInput.attr('type') === 'password' ? 'text' : 'password';
        loginInput.attr('type', type);
    });

    // Function to handle login form submission
    $('#loginForm').on('submit', function (e) {
        e.preventDefault();
        var email = $('#loginEmail').val();
        var password = $('#loginPassword').val();

        // Basic validation
        if (!email || !password) {
            alert("Будь ласка, заповніть всі поля.");
            return;
        }

        // Get the anti-forgery token
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            type: 'POST',
            url: '/Account/Login',
            data: {
                __RequestVerificationToken: token,
                Email: email,
                Password: password
            },
            success: function (responseLogin) {
                if (responseLogin.success) {
                    $.ajax({
                        type: 'GET',
                        url: '/Home/GetUsername',
                        data: {
                            userId: responseLogin.id
                        },
                        success: function (responseGetUsername) {
                            //location.reload();
                            // Save user info to localStorage
                            localStorage.setItem("activeUserId", responseLogin.id);
                            localStorage.setItem("activeUserName", responseGetUsername.username);

                            // Update UI
                            updateAuthUI(true, responseGetUsername.username);

                            // Close modal
                            $('#loginModal').fadeOut();
                            resetLoginForm();
                        }
                    })
                } else {
                    alert(responseLogin.message);
                }
            },
            error: function () {
                alert("Помилка сервера. Спробуйте пізніше.");
            }
        });
    });

    // Function to switch to registration modal
    $('.btn-to-register').on('click', function () {
        $('#loginModal').fadeOut();
        $('#registerModal').fadeIn();
        resetLoginForm();
    });

    // Helper function to reset login form
    function resetLoginForm() {
        $('#loginForm')[0].reset();
        $('#loginForm').find('.has-error').removeClass('has-error');
    }

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
});
