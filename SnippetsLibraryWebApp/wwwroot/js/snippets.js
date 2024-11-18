// wwwroot/js/snippets.js

$(document).ready(function () {
    // Посилання на різні набори сніпетів
    $('#loadAllSnippets').on('click', function (e) {
        e.preventDefault();
        loadSnippets('/Snippets/AllSnippets');
    });

    $('#loadFavoriteSnippets').on('click', function (e) {
        e.preventDefault();
        loadSnippets('/Snippets/FavoriteSnippets');
    });

    $('#loadMySnippets').on('click', function (e) {
        e.preventDefault();
        loadSnippets('/Snippets/MySnippets');
    });

    // Обробка кліку на кнопку "Деталі"
    $(document).on('click', '.details-btn', function (e) {
        e.preventDefault();
        var snippetId = $(this).data('id');
        window.location.href = '/Snippets/Details/' + snippetId;
    });

    // Функція для завантаження сніпетів через AJAX
    window.loadSnippets = function (url) { // Експортуємо функцію до глобального об'єкта
        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                $('#snippetsContent').html(data);
            },
            error: function (xhr, status, error) {
                if (xhr.status === 401) { // Unauthorized
                    // Перенаправити на головну сторінку або показати повідомлення
                    window.location.href = '/Snippets/AllSnippets'; // Перенаправлення на головну
                } else {
                    alert("Помилка при завантаженні сніпетів.");
                }
            }
        });
    };
});
