// wwwroot/js/addSnippet.js

$(document).ready(function () {
    const maxCategories = 3;
    const maxTags = 5; // Встановіть ліміт для тегів за потребою або видаліть, якщо без ліміту
    var language = "plaintext"

    const languageModes = {
        bash: "shell",
        c: "text/x-csrc", // Режим для C
        csharp: "text/x-csharp", // Режим для C#
        cpp: "text/x-c++src", // Режим для C++
        css: "css",
        dart: "dart",
        elixir: "plaintext",
        erlang: "plaintext",
        fsharp: "plaintext",
        go: "go",
        groovy: "plaintext",
        haskell: "plaintext",
        html: "htmlmixed",
        java: "text/x-java", // Режим для Java
        javascript: "javascript",
        json: "application/json",
        julia: "plaintext",
        kotlin: "plaintext",
        lua: "lua",
        matlab: "plaintext",
        objectivec: "text/x-objectivec", // Режим для Objective-C
        other: "plaintext",
        perl: "perl",
        php: "php",
        plsql: "sql",
        powershell: "plaintext",
        python: "python",
        r: "plaintext",
        ruby: "ruby",
        rust: "rust",
        scala: "plaintext",
        shell: "shell",
        sql: "sql",
        swift: "swift",
        tsql: "sql",
        typescript: "javascript",
        vbnet: "plaintext",
        xml: "xml",
        yaml: "yaml",
    };


    // Ініціалізація випадайок
    initializeDropdown('#categoryDropdown', '.category-checkbox', '#selectedCategories', maxCategories, 'category', true);
    initializeDropdown('#tagDropdown', '.tag-checkbox', '#selectedTags', maxTags, 'tag', false);

    var editor = CodeMirror.fromTextArea(document.getElementById('Code'), {
        mode: "plaintext",
        theme: "monokai",
        lineNumbers: true,
        matchBrackets: true,
        autoCloseTags: true,
        lineWrapping: true,
    });
    editor.save()

    $('#ProgrammingLanguageID').on('change', function (e) {
        language = $('#ProgrammingLanguageID')[0].selectedOptions[0].text

        language = language == "C#" ? "csharp" :
            language == "C++" ? "cpp" :
                language == "F#" ? "fsharp" :
                    language == "VB.NET" ? "vbnet" :
                        language == "PL/SQL" ? "plsql" :
                            language == "Objective-C" ? "objectivec" :
                                language == "T-SQL" ? "tsql" :
                                    language == "Visual Basic" ? "vbnet" :
                                        language == "Shell script" ? "shell" : language

        var mode = languageModes[language.toLowerCase()] || "plaintext"

        editor.setOption("mode", mode);
    })

    // Синхронізація CodeMirror з textarea при відправці форми
    $('form').on('submit', function () {
        $('#Code').val(codeMirrorInstance.getValue());
    });

    // Функція для ініціалізації поведінки випадайки
    function initializeDropdown(dropdownSelector, checkboxSelector, selectedListSelector, maxSelection, type, hasRemoveButton) {
        const dropdown = $(dropdownSelector);
        const dropdownList = dropdown.find('.dropdown-content');
        const selectedList = $(selectedListSelector);

        // Перемикання видимості списку при кліку на контейнер випадайки
        dropdown.on('click', function (e) {
            e.stopPropagation(); // Запобігаємо поширенню кліку до документа
            $('.dropdown-content').not(dropdownList).hide(); // Приховуємо інші випадайки
            dropdownList.toggle();
        });

        // Приховування випадайки при кліку поза нею
        $(document).on('click', function () {
            dropdownList.hide();
        });

        // Запобігання закриттю випадайки при кліку всередині її вмісту
        dropdownList.on('click', function (e) {
            e.stopPropagation();
        });

        // Обробка змін у чекбоксах
        $(checkboxSelector).on('change', function () {
            updateSelectedItems();
        });

        // Функція для оновлення відображення вибраних елементів
        function updateSelectedItems() {
            const selected = [];
            $(checkboxSelector + ':checked').each(function () {
                selected.push({
                    id: $(this).val(),
                    name: $(this).closest('label').text().trim()
                });
            });

            // Дотримання ліміту вибору
            if (selected.length > maxSelection) {
                alert(`Ви можете обрати максимум ${maxSelection} ${type === 'category' ? 'категорії' : 'теги'}.`);
                $(this).prop('checked', false);
                return;
            }

            // Оновлення відображення вибраних елементів
            selectedList.empty();
            selected.forEach(function (item) {
                const itemTag = $('<span>', {
                    class: `selected-${type}`,
                    'data-id': item.id,
                    text: item.name
                });

                if (hasRemoveButton) {
                    const removeBtn = $('<span>', {
                        class: `remove-${type}`,
                        text: ' ×', // Пробіл перед хрестиком для кращого відступу
                        'aria-label': `Remove ${item.name}`,
                        role: 'button',
                        tabindex: '0' // Робимо фокусованим
                    });

                    // Додаємо хрестик до тегу
                    itemTag.append(removeBtn);
                }

                // Додаємо тег до списку вибраних
                selectedList.append(itemTag);
            });

            // Оновлення плейсхолдера випадайки
            if (selected.length > 0) {
                dropdown.find('.dropdown-placeholder').text(`${selected.length} обрано`);
            } else {
                dropdown.find('.dropdown-placeholder').text(`--Оберіть ${type === 'category' ? 'до 3-х категорій' : 'теги'}--`);
            }
        }

        // Делегування події кліку для видалення (тільки для категорій)
        if (hasRemoveButton) {
            selectedList.on('click', `.remove-${type}`, function () {
                const itemTag = $(this).parent(`.selected-${type}`);
                const itemId = itemTag.data('id');

                // Відмічаємо відповідний чекбокс
                $(`${checkboxSelector}[value="${itemId}"]`).prop('checked', false);

                // Видаляємо тег з відображення
                itemTag.remove();

                // Оновлення плейсхолдера випадайки
                const selectedCount = $(checkboxSelector + ':checked').length;
                if (selectedCount > 0) {
                    dropdown.find('.dropdown-placeholder').text(`${selectedCount} обрано`);
                } else {
                    dropdown.find('.dropdown-placeholder').text(`--Оберіть ${type === 'category' ? 'до 3-х категорій' : 'теги'}--`);
                }
            });

            // Обробка клавіатурних подій для видалення
            selectedList.on('keydown', `.remove-${type}`, function (e) {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    $(this).click();
                }
            });
        }
    }

    // Опціонально: Обробка відправки форми для перевірки лімітів
    $('form').on('submit', function (e) {
        var selectedCount = $('.category-checkbox:checked').length;
        var selectedTagsCount = $('.tag-checkbox:checked').length;

        if (selectedCount > maxCategories) {
            alert("Ви можете обрати максимум 3 категорії.");
            e.preventDefault(); // Запобігаємо відправці форми
        }

        if (selectedTagsCount > maxTags) {
            alert("Ви можете обрати максимум 5 тегів.");
            e.preventDefault(); // Запобігаємо відправці форми
        }
    });
    function autoResize(element) {
        element.style.height = 'auto'; // Спочатку встановлюємо висоту на автоматичну
        element.style.height = (element.scrollHeight) + 'px'; // Змінюємо висоту на основі scrollHeight
    }

    const description = document.getElementById('Description');
    const title = document.getElementById('Title');

    description.addEventListener('input', function () {
        autoResize(this);
    });

    title.addEventListener('input', function () {
        autoResize(this);
    });
});