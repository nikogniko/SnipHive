var app = angular.module('SnipHiveFrontend', []);

app.controller('MainController', ['$scope', '$rootScope', function ($scope, $rootScope) {
    // Initialize user state
    $scope.isAuthenticated = false;
    $scope.activeUserName = '';
    
    // Function to update user state after login or registration
    $scope.updateUserState = function (username) {
        $scope.isAuthenticated = true;
        $scope.activeUserName = username;
    };

    $scope.userMenuVisible = false;

    $scope.toggleUserMenu = function () {
        $scope.userMenuVisible = !$scope.userMenuVisible;
    };

    // Close the user menu when clicking outside
    //document.addEventListener('click', function (event) {
    //    var isClickInside = document.getElementById('userDropdown')?.contains(event.target);
    //    if (!isClickInside && $scope.userMenuVisible) {
    //        $scope.$apply(function () {
    //            $scope.userMenuVisible = false;
    //        });
    //    }
    //});

    $scope.$on('userLoggedIn', function (event, username) {
        $scope.isAuthenticated = true;
        $scope.activeUserName = username;
    });

    // Function to log out
    $scope.logout = function () {
        $scope.isAuthenticated = false;
        $scope.activeUserName = '';
        localStorage.removeItem("activeUserId");
        localStorage.removeItem("activeUserName");
    };

    // Check if user is already logged in (e.g., after page refresh)
    if (localStorage.getItem("activeUserId")) {
        $scope.isAuthenticated = true;
        $scope.activeUserName = localStorage.getItem("activeUserName");
    }

    // Functions to open modals (to be called from header buttons)
    $scope.openLoginModal = function () {
        $rootScope.$broadcast('openLoginModal');
    };

    $scope.openRegisterModal = function () {
        $rootScope.$broadcast('openRegisterModal');
    };
}]);

app.controller('RegistrationController', ['$scope', '$rootScope', function ($scope, $rootScope) {
    $scope.registerData = {};
    $scope.showRegisterModal = false;
    $scope.showPassword = false;

    // Listen for open modal event
    $scope.$on('openRegisterModal', function () {
        $scope.showRegisterModal = true;
    });

    // Function to close the registration modal
    $scope.closeRegisterModal = function () {
        $scope.showRegisterModal = false;
        $scope.registerData = {};
        $scope.registerForm.$setPristine();
        $scope.registerForm.$setUntouched();
    };

    // Function to toggle password visibility
    $scope.togglePasswordVisibility = function () {
        $scope.showPassword = !$scope.showPassword;
    };

    // Function to register the user
    $scope.register = function () {
        if ($scope.registerForm.$valid) {
            // Perform registration via AJAX
            $.ajax({
                type: 'POST',
                url: '/Home/RegisterUser',
                data: {
                    username: $scope.registerData.username,
                    email: $scope.registerData.email,
                    password: $scope.registerData.password
                },
                success: function (response) {
                    if (response.success) {
                        // Save user info
                        localStorage.setItem("activeUserId", response.userId);
                        localStorage.setItem("activeUserName", $scope.registerData.username);

                        // Update user state in MainController
                        $scope.$apply(function () {
                            $rootScope.$broadcast('userLoggedIn', $scope.registerData.username);
                            $scope.closeRegisterModal();
                        });
                    } else {
                        alert(response.message);
                    }
                }
            });
        }
    };

    // Switch to login modal
    $scope.switchToLogin = function () {
        $scope.closeRegisterModal();
        $rootScope.$broadcast('openLoginModal');
    };
}]);

app.controller('LoginController', ['$scope', '$rootScope', function ($scope, $rootScope) {
    $scope.loginData = {};
    $scope.showLoginModal = false;
    $scope.showPassword = false;

    // Listen for open modal event
    $scope.$on('openLoginModal', function () {
        $scope.showLoginModal = true;
        $scope.$apply
    });

    // Function to close the login modal
    $scope.closeLoginModal = function () {
        $scope.showLoginModal = false;
        $scope.loginData = {};
        $scope.loginForm.$setPristine();
        $scope.loginForm.$setUntouched();
    };

    // Function to toggle password visibility
    $scope.togglePasswordVisibility = function () {
        $scope.showPassword = !$scope.showPassword;
    };

    // Function to log in the user
    $scope.login = function () {
        if ($scope.loginForm.$valid) {
            // Perform login via AJAX
            $.ajax({
                type: 'POST',
                url: '/Home/LoginUser',
                data: {
                    email: $scope.loginData.email,
                    password: $scope.loginData.password
                },
                success: function (response) {
                    if (response.success) {
                        // Save user info
                        localStorage.setItem("activeUserId", response.userId);
                        localStorage.setItem("activeUserName", response.username);

                        // Update user state in MainController
                        $scope.$apply(function () {
                            $rootScope.$broadcast('userLoggedIn', response.username);
                            $scope.closeLoginModal();
                        });
                    } else {
                        alert(response.message);
                    }
                }
            });
        }
    };

    // Switch to registration modal
    $scope.switchToRegister = function () {
        $scope.closeLoginModal();
        $rootScope.$broadcast('openRegisterModal');
    };
}]);