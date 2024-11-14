// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



document.addEventListener('DOMContentLoaded', () => {

    let activeUserId;
    let activeUserName;

    // Реєстрація --------------------------------------------------------------------------

    const openRegisterModalButton = document.getElementById('openRegisterModal');
    const registerModal = document.getElementById('registerModal');
    const closeModalButton = document.getElementById('closeModal');

    const registerButton = document.getElementById('registerButton');
    const registerUsernameInput = document.getElementById('registerUsername');
    const registerEmailInput = document.getElementById('registerEmail');
    const registerPasswordInput = document.getElementById('registerPassword');

    // Показ/приховування пароля
    const togglePassword = document.getElementById('togglePassword');

    const registerForm = document.getElementById('registerForm');
    const registerUsernameError = document.getElementById('registerUsernameError');
    const registerEmailError = document.getElementById('registerEmailError');
    const registerPasswordError = document.getElementById('registerPasswordError');

    // Відкриття модального вікна
    openRegisterModalButton.addEventListener('click', () => {
        registerModal.style.display = 'block';
    });

    // Закриття модального вікна при натисканні на хрестик
    closeModalButton.addEventListener('click', () => {
        closeRegisterForm();
    });

    // Закриття модального вікна при кліку за його межами
    window.addEventListener('click', (event) => {
        if (event.target === registerModal) {
            closeRegisterForm();
        }
    });

    // Показ/приховування пароля
    togglePassword.addEventListener('click', () => {
        if (registerPasswordInput.type === 'password') {
            registerPasswordInput.type = 'text';
            togglePassword.textContent = '👁️‍🗨️'; // Символ для показу
        } else {
            registerPasswordInput.type = 'password';
            togglePassword.textContent = '👁️'; // Символ для приховування
        }
    });

    // Очищення полів форми і помилок
    function closeRegisterForm() {
        registerModal.style.display = 'none';
        // Очищення полів
        registerForm.reset();
        // Видалення повідомлень про помилки
        clearRegisterErrors();
    }

    function clearRegisterErrors() {
        removeError(registerUsernameInput, registerUsernameError, 'error');
        removeError(registerEmailInput, registerEmailError, 'error');
        removeError(registerPasswordInput, registerPasswordError, 'error');
    }

    // Зникнення помилки при новому введенні
    registerUsernameInput.addEventListener('input', () => {
        removeError(registerUsernameInput, registerUsernameError, 'error');
    });

    registerEmailInput.addEventListener('input', () => {
        removeError(registerEmailInput, registerEmailError, 'error');
    });

    registerPasswordInput.addEventListener('input', () => {
        removeError(registerPasswordInput, registerPasswordError, 'error');
    });

    // Реєстрація користувача за допомогою fetch
    registerButton.addEventListener('click', async () => {
        let isValid = true;

        // Очистка попередніх повідомлень про помилки
        document.querySelectorAll('.error-message').forEach(el => (el.style.display = 'none'));
        document.querySelectorAll('.form-input').forEach(el => el.classList.remove('error'));

        // Валідація імені користувача
        if (registerUsernameInput.value.trim().length < 3) {
            isValid = false;
            showError('registerUsernameError', 'Нікнейм користувача повинен містити мінімум 3 символи');
            registerUsernameInput.classList.add('error');
        }

        // Валідація email
        if (!validateEmail(registerEmailInput.value)) {
            isValid = false;
            showError('registerEmailError', 'Введіть коректну адресу електронної пошти');
            registerEmailInput.classList.add('error');
        }

        // Валідація пароля
        const passwordValue = registerPasswordInput.value;
        const passwordRegex = /^[a-zA-Z0-9]{8,25}$/;
        if (!passwordRegex.test(passwordValue)) {
            isValid = false;
            showError('registerPasswordError', 'Пароль повинен містити 8-25 латинських літер або цифр');
            registerPasswordInput.classList.add('error');
        }

         if (!isValid) {
            return;
        }

        $.ajax({
            async: false,
            type: "POST",
            url: '/Home/RegisterUser?username=' + registerUsernameInput.value + '&email=' + registerEmailInput.value + '&password=' + registerPasswordInput.value,
            success: function (response){
                if (response.success) {
                    closeRegisterForm();

                    // Збереження userId в локальному сховищі або в сесії

                    localStorage.setItem("activeUserId", response.userId);

                    //activeUserId = response.userId;

                    alert(response.message);
                    updateHeaderWithUsername();

                    localStorage.getItem("activeUserName");
                } else {
                    alert(response.message);
                }
            }
        });

    });

    // Валідація форми
    /*registerButton.addEventListener('click', async () => {
        // Перевірка унікальності користувача
*//*        const userExists = await checkIfUserExists(usernameInput.value, emailInput.value);
        if (userExists) {
            showError('emailError', 'Користувач із таким email або іменем вже існує');
            emailInput.classList.add('error');
            return;
        }*//*

        // Відправка даних на сервер (можна налаштувати на реальний запит)
        alert('Форма пройшла валідацію');
    });*/



    //---------------------------------------------------------------------------------------------------------

    // Вхід в систему

    const openLoginModalButton = document.getElementById('openLoginModal');
    const loginModal = document.getElementById('loginModal');
    const closeLoginModalButton = document.getElementById('closeLoginModal');

    const loginButton = document.getElementById('loginButton');
    const loginEmailInput = document.getElementById('loginEmail');
    const loginPasswordInput = document.getElementById('loginPassword');

    const loginForm = document.getElementById('loginForm');
    const loginEmailError = document.getElementById('loginEmailError');
    const loginPasswordError = document.getElementById('loginPasswordError');

    // Відкриття модального вікна
    openLoginModalButton.addEventListener('click', () => {
        loginModal.style.display = 'block';
    });

    // Закриття модального вікна при натисканні на хрестик
    closeLoginModalButton.addEventListener('click', () => {
        closeLoginForm();
    });

    // Закриття модального вікна при кліку за його межами
    window.addEventListener('click', (event) => {
        if (event.target === loginModal) {
            closeLoginForm();
        }
    });

     // Показ/приховування пароля
    const toggleLoginPassword = document.getElementById('toggleLoginPassword');

    toggleLoginPassword.addEventListener('click', () => {
        if (loginPasswordInput.type === 'password') {
            loginPasswordInput.type = 'text';
            toggleLoginPassword.textContent = '👁️‍🗨️'; // Зміна символу
        } else {
            loginPasswordInput.type = 'password';
            toggleLoginPassword.textContent = '👁️'; // Початковий символ
        }
    });

    // Очищення полів форми і помилок
    function closeLoginForm() {
        loginModal.style.display = 'none';
        // Очищення полів
        loginForm.reset();
        // Видалення повідомлень про помилки
        clearLoginErrors();
    }

    function clearLoginErrors() {
        removeError(loginEmailInput, loginEmailError, 'error');
        removeError(loginPasswordInput, loginPasswordError, 'error');
    }

    // Зникнення помилки при новому введенні
    loginEmailInput.addEventListener('input', () => {
        removeError(loginEmailInput, loginEmailError, 'error');
    });

    loginPasswordInput.addEventListener('input', () => {
        removeError(loginPasswordInput, loginPasswordError, 'error');
    });

    // Валідація форми при кліку на кнопку "Увійти"
    loginButton.addEventListener('click', async () => {
        let isValid = true;
        clearLoginErrors(); // Очищення помилок перед перевіркою

        // Валідація email
        if (!validateEmail(loginEmailInput.value)) {
            showError('loginEmailError', 'Введіть коректну адресу електронної пошти');
            loginEmailInput.classList.add('error');
            isValid = false;
        }

        // Валідація пароля
        const passwordRegex = /^[a-zA-Z0-9]{8,25}$/;
        if (!passwordRegex.test(loginPasswordInput.value)) {
            showError('loginPasswordError', 'Пароль повинен містити 8-25 латинських літер або цифр');
            loginPasswordInput.classList.add('error');
            isValid = false;
        }

        // Якщо форма валідна, виконуємо вхід
        if (isValid) {
            // Тут може бути логіка для входу
            alert('Форма успішно заповнена');
            
            $.ajax({
                async: false,
                type: "POST",
                url: '/Home/LoginUser?email=' + loginEmailInput.value + '&password=' + loginPasswordInput.value,
                success: function(response) {
                    if (response.success) {
                        closeLoginForm();
                        // Збереження userId в локальному сховищі або в сесії
                        localStorage.setItem("activeUserId", response.userId);

                        //activeUserId = response.userId;

                        alert(response.message);
                        updateHeaderWithUsername();
                        localStorage.getItem("activeUserName");
                    } else {
                        alert(response.message);
                    }
                }
            });

            // Перевірка унікальності користувача
            //const userExists = await checkIfUserExists(usernameInput.value, emailInput.value);
            //if (userExists) {
            //    showError('emailError', 'Користувач із таким email або іменем вже існує');
            //    emailInput.classList.add('error');
            //    return;
            //}

            // Відправка даних на сервер (можна налаштувати на реальний запит)

        }
        else {
            alert('Форма не пройшла валідацію');
        }

    });
    

    function validateEmail(email) {
        const re = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return re.test(email);
    }

    function showError(elementId, message) {
        const errorElement = document.getElementById(elementId);
        errorElement.textContent = message;
        errorElement.style.display = 'block';
    }

    // Видалення помилок
    function removeError(elementId, errorElement, errorClassName) {
        if (elementId.classList.contains(errorClassName)) {
            errorElement.textContent = '';
            elementId.classList.remove(errorClassName);
        }
    }

    function getUsernameById() {
        if (localStorage.length > 0) {
        //if (activeUserId){
            $.ajax({
                async: false,
                type: "POST",
                url: "/Home/GetUsername?userId=" + localStorage.getItem("activeUserId"),
                //   activeUserId,
                success: function(response) {
                    if (response.success) {
                        localStorage.setItem("activeUserName", response.username);

                        //activeUserName = response.username;

                    } else {
                        alert(response.message);
                    }
                },
                error: function() {
                    alert("An error occurred while trying to fetch the username.");
                }
            });
        }
        else{
            alert('Active user not found.')
        }
    }

    function updateHeaderWithUsername() {
        getUsernameById();
        const activeUserName = localStorage.getItem("activeUserName");

        if (activeUserName) {
            // Знаходження контейнера з кнопками
            const navContainer = document.querySelector(".navbar-nav.ml-auto");

            // Створення посилання на сторінку додавання сніпета
            const addSnippetBtn = document.createElement("li");
            addSnippetBtn.className = "nav-item";

            addSnippetBtn.innerHTML = `
                <a href="/AddSnippet/AddSnippet"
                    id="addSnippetBtn" class="secondary-btn btn-add-snippet">
                    + Додати сніпет</a>
            `;

            // Створення випадаючого меню з ім'ям користувача
            const userDropdown = document.createElement("li");
            userDropdown.className = "nav-item dropdown";
    
            userDropdown.innerHTML = `
                <a class="nav-link dropdown-toggle" href="#" id="userDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    ${activeUserName}
                </a>
                <div class="dropdown-menu dropdown-menu-right" aria-labelledby="userDropdown">
                    <a class="dropdown-item" href="#" id="logoutButton">Вийти</a>
                </div>
            `;

    
            // Очищення контейнера та додавання нового елементу
            navContainer.innerHTML = "";
            navContainer.appendChild(addSnippetBtn);
            navContainer.appendChild(userDropdown);

            // Додавання обробника для відкриття/розгортання меню
            document.getElementById("userDropdown").addEventListener("click", function (event) {
                event.preventDefault(); // Відміняємо стандартну дію
                const dropdownMenu = this.nextElementSibling;
                dropdownMenu.classList.toggle("show");
            });

            // Додавання обробника для кнопки виходу
            document.getElementById("logoutButton").addEventListener("click", function () {
                // Очищення локального сховища та перезавантаження сторінки
                localStorage.removeItem("activeUserId");
                localStorage.removeItem("activeUserName");
                location.reload();
            });

            // Закриття меню при кліку поза ним
            document.addEventListener("click", function (event) {
                const isClickInside = userDropdown.contains(event.target);
                if (!isClickInside) {
                    const dropdownMenu = document.querySelector(".dropdown-menu");
                    if (dropdownMenu) {
                        dropdownMenu.classList.remove("show");
                    }
                }
            });
        }
    }


    
    // Масив для зберігання вибраних категорій
/*    let selectedCategories = [];

    // Ініціалізація елементів DOM
    const categoriesDropdown = document.getElementById("dropdownList");
    const selectedCategoriesContainer = document.getElementById("selectedCategories");

    // Створення початкового тексту для вибору категорій
    const placeholderOption = document.createElement("div");
    placeholderOption.textContent = "--Оберіть до 3-х категорій--";
    placeholderOption.classList.add("dropdown-placeholder");
    categoriesDropdown.before(placeholderOption);

    // Додавання слухача подій для відкриття комбобоксу
    placeholderOption.addEventListener("click", () => {
        categoriesDropdown.style.display = categoriesDropdown.style.display === "block" ? "none" : "block";
    });

    // Оновлення категорій у випадаючому меню
    categoriesDropdown.addEventListener("change", (event) => {
        const selectedOption = event.target.options[event.target.selectedIndex];

        // Перевірка чи категорія вже вибрана
        if (!selectedCategories.includes(selectedOption.value) && selectedCategories.length < 3) {
            // Додавання вибраної категорії
            selectedCategories.push(selectedOption.value);

            // Відображення вибраної категорії як "кнопки"
            const categoryButton = document.createElement("div");
            categoryButton.classList.add("category-chip");
            categoryButton.textContent = selectedOption.text;
            selectedCategoriesContainer.appendChild(categoryButton);

            // Додавання кнопки закриття (хрестик)
            const closeButton = document.createElement("span");
            closeButton.classList.add("close-button");
            closeButton.textContent = "✖";
            categoryButton.appendChild(closeButton);

            // Слухач для видалення вибраної категорії
            closeButton.addEventListener("click", () => {
                selectedCategories = selectedCategories.filter((id) => id !== selectedOption.value);
                categoryButton.remove();

                // Зняття вибору у списку
                selectedOption.selected = false;
            });
        } else if (selectedCategories.length >= 3) {
            alert("Ви можете обрати не більше 3 категорій.");
            event.target.value = ""; // Скидання вибору
        }
    });
        
    // Закриття списку після вибору
    categoriesDropdown.style.display = "none";

    // CSS-класи для стилізації
    const style = document.createElement("style");
    style.textContent = `
        .category-chip {
          display: inline-block;
          padding: 5px 10px;
          margin: 5px;
          border: 2px solid darkgreen;
          border-radius: 20px;
          background-color: white;
          color: darkgreen;
          font-size: 14px;
          cursor: pointer;
        }
        .category-chip .close-button {
          margin-left: 8px;
          color: gray;
          cursor: pointer;
        }
        .dropdown-placeholder {
          cursor: pointer;
          color: #555;
          margin-bottom: 5px;
        }
        #Categories {
          display: none;
          margin-top: 5px;
        }
    `;
    document.head.appendChild(style);*/

    document.addEventListener("DOMContentLoaded", updateHeaderWithUsername);

    const categoryDropdown = document.getElementById('categoryDropdown');
    const dropdownPlaceholder = document.querySelector('.dropdown-placeholder');
    const dropdownList = document.getElementById('dropdownList');
    const selectedCategories = document.getElementById('selectedCategories');
    let selectedCategoriesList = [];

    dropdownPlaceholder.addEventListener('click', () => {
        dropdownList.style.display = dropdownList.style.display === 'block' ? 'none' : 'block';
    });

    dropdownList.addEventListener('change', (event) => {
        const checkbox = event.target;
        const categoryName = checkbox.nextElementSibling.textContent;

        if (checkbox.checked) {
            if (selectedCategoriesList.length < 3) {
                addCategory(checkbox.value, categoryName);
            } else {
                checkbox.checked = false;
                alert('Ви не можете обрати більше 3-х категорій');
            }
        } else {
            removeCategory(checkbox.value);
        }
    });

    function addCategory(id, name) {
        selectedCategoriesList.push(id);
        const categoryElement = document.createElement('div');
        categoryElement.className = 'selected-category';
        categoryElement.setAttribute('data-id', id);
        categoryElement.innerHTML = `${name}<span class="remove-icon">x</span>`;

        categoryElement.querySelector('.remove-icon').addEventListener('click', () => {
            removeCategory(id);
            document.querySelector(`input[value="${id}"]`).checked = false;
        });

        selectedCategories.appendChild(categoryElement);
    }

    function removeCategory(id) {
        selectedCategoriesList = selectedCategoriesList.filter(categoryId => categoryId !== id);
        const categoryElement = document.querySelector(`.selected-category[data-id="${id}"]`);
        if (categoryElement) {
            selectedCategories.removeChild(categoryElement);
        }
    }

    // Закриття випадаючого меню при натисканні за межами меню
    document.addEventListener('click', (event) => {
        if (!categoryDropdown.contains(event.target)) {
            dropdownList.style.display = 'none';
        }
    });


    async function checkIfUserExists(username, email) {
        // Тут відбудеться запит до бази для перевірки існування користувача
        // Наприклад:
        // const response = await fetch('/api/checkUserExists', {
        //   method: 'POST',
        //   body: JSON.stringify({ username, email }),
        //   headers: { 'Content-Type': 'application/json' }
        // });
        // const result = await response.json();
        // return result.exists;

        // Тимчасове значення для демонстрації
        return false;
    }
});