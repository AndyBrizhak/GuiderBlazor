// Файл: wwwroot/js/theme.js

// 1. Применяет тему (light-theme или dark-theme)
window.applyTheme = function (theme) {
    document.documentElement.className = theme;
    localStorage.setItem('theme', theme);
    document.cookie = `theme=${theme};path=/;max-age=31536000`;
}

// 2. Загружает сохраненную тему и возвращает isDarkMode
window.loadTheme = function () {
    var theme = localStorage.getItem('theme') || 'light-theme';
    document.documentElement.className = theme;
    document.cookie = `theme=${theme};path=/;max-age=31536000`;
    return theme === 'dark-theme';
}

// 3. Инициализация наблюдателя за Blazor навигацией
document.addEventListener('DOMContentLoaded', function () {
    if (typeof Blazor !== 'undefined') {
        // Восстанавливаем тему после каждой навигации
        Blazor.addEventListener('enhancedload', function () {
            var theme = localStorage.getItem('theme') || 'light-theme';
            document.documentElement.className = theme;
        });
    }
});

// 4. Применяем тему немедленно (при загрузке скрипта)
(function () {
    var theme = localStorage.getItem('theme') || 'light-theme';
    document.documentElement.className = theme;
})();
