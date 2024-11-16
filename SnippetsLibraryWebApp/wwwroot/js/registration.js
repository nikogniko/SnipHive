// wwwroot/js/registration.js

$(document).ready(function () {
    // Function to close the registration modal
    $('.close-register-modal').on('click', function () {
        $('#registerModal').fadeOut();
        resetRegisterForm();
    });

    // Function to toggle password visibility
    $('.toggle-register-password').on('click', function () {
        var passwordInput = $('#registerPassword');
        var type = passwordInput.attr('type') === 'password' ? 'text' : 'password';
        passwordInput.attr('type', type);
    });

    // Function to handle registration form submission
    $('#registerForm').on('submit', function (e) {
        e.preventDefault();
        var username = $('#registerUsername').val();
        var email = $('#registerEmail').val();
        var password = $('#registerPassword').val();

        // Basic validation
        if (!username || !email || !password) {
            alert("Будь ласка, заповніть всі поля.");
            return;
        }

        if (username.length < 3) {
            alert("Нікнейм повинен містити мінімум 3 символи.");
            return;
        }

        if (password.length < 8 || password.length > 25) {
            alert("Пароль повинен містити 8-25 символів.");
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/Home/RegisterUser',
            data: {
                username: username,
                email: email,
                password: password
            },
            success: function (response) {
                if (response.success) {
                    // Save user info to localStorage
                    localStorage.setItem("activeUserId", response.userId);
                    localStorage.setItem("activeUserName", username);

                    // Update UI
                    updateAuthUI(true, username);

                    // Close modal
                    $('#registerModal').fadeOut();
                    resetRegisterForm();
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Помилка сервера. Спробуйте пізніше.");
            }
        });
    });

    // Function to switch to login modal
    $('.btn-to-login').on('click', function () {
        $('#registerModal').fadeOut();
        $('#loginModal').fadeIn();
        resetRegisterForm();
    });

    // Helper function to reset registration form
    function resetRegisterForm() {
        $('#registerForm')[0].reset();
        $('#registerForm').find('.has-error').removeClass('has-error');
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
