// wwwroot/js/app.js

window.locateMe = function () {
    if (!navigator.geolocation) {
        alert("Geolocation is not supported by your browser");
        return;
    }

    // Показываем пользователю, что процесс пошел (можно добавить спиннер на кнопку)
    const btn = document.querySelector('.hero-geo-btn');
    const originalText = btn ? btn.innerText : "Find Near Me";
    if (btn) btn.innerText = "Locating...";

    navigator.geolocation.getCurrentPosition(
        (position) => {
            const lat = position.coords.latitude;
            const long = position.coords.longitude;

            // 1. Берем текущий URL
            const currentUrl = new URL(window.location.href);

            // 2. Добавляем/Обновляем параметры гео
            currentUrl.searchParams.set('Latitude', lat);
            currentUrl.searchParams.set('Longitude', long);

            // 3. Обычно при поиске рядом мы хотим сортировку по дистанции
            currentUrl.searchParams.set('SortField', 'distance');
            currentUrl.searchParams.set('SortOrder', 'asc');

            // 4. Сбрасываем страницу на 1
            currentUrl.searchParams.set('Page', '1');

            // 5. Выполняем навигацию (Enhanced Navigation подхватит это)
            window.location.assign(currentUrl.toString());
        },
        (error) => {
            console.error("Geo error:", error);
            alert("Unable to retrieve your location. Please check permissions.");
            if (btn) btn.innerText = originalText;
        },
        {
            enableHighAccuracy: true, // Важно для точного поиска мест рядом
            timeout: 10000,
            maximumAge: 0
        }
    );
};

// --- Улучшение UX для Enhanced Navigation ---

// Blazor Enhanced Navigation не перезагружает страницу полностью.
// Поэтому мобильное меню может остаться открытым после клика по ссылке.
// Мы слушаем событие окончания загрузки и закрываем меню принудительно.

Blazor.addEventListener('enhancedload', () => {
    // 1. Закрываем мобильное меню, если оно открыто
    const navLinks = document.getElementById('nav-links');
    if (navLinks && navLinks.classList.contains('is-active')) {
        navLinks.classList.remove('is-active');
    }

    // 2. Скроллим наверх (опционально, так как Enhanced Nav старается сохранять позицию, 
    // но для пагинации лучше скроллить вверх)
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('Page')) {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }
});