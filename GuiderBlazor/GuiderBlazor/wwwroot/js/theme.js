// Файл: wwwroot/js/theme.js

// 1. Применяет тему (light-theme или dark-theme)
function applyTheme(theme) {
    document.documentElement.className = theme;
    localStorage.setItem('theme', theme);
    // ⬇️ ДОБАВЬТЕ ЭТУ СТРОКУ:
    document.cookie = `theme=${theme};path=/;max-age=31536000`; // Сохраняем cookie на год
}

// 2. Загружает сохраненную тему при запуске
function loadTheme() {
    var theme = localStorage.getItem('theme') || 'light-theme';
    document.documentElement.className = theme;
    // ⬇️ ДОБАВЬТЕ ЭТУ СТРОКУ (чтобы cookie был с 1-й загрузки):
    document.cookie = `theme=${theme};path=/;max-age=31536000`;

    return theme === 'dark-theme';
}