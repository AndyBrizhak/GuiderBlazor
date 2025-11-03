// Файл: wwwroot/js/theme.js

// 1. Применяет тему (light-theme или dark-theme) к <html>
function applyTheme(theme) {
    document.documentElement.className = theme;
    localStorage.setItem('theme', theme);
}

// 2. Загружает сохраненную тему при запуске
function loadTheme() {
    var theme = localStorage.getItem('theme') || 'light-theme'; // По умолчанию 'light'
    document.documentElement.className = theme;
    // Возвращаем true, если тема темная, чтобы Blazor знал
    return theme === 'dark-theme';
}