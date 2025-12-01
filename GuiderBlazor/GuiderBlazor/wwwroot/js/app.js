// wwwroot/js/app.js

// Глобальная переменная для хранения позиции, пока пользователь думает над предупреждением
let pendingGeoPosition = null;

window.toggleGeoLocation = function () {
    // 1. Проверяем, включен ли уже фильтр (есть ли Latitude в URL)
    const currentUrl = new URL(window.location.href);
    const hasLat = currentUrl.searchParams.has('Latitude');

    // === ЛОГИКА ОТМЕНЫ (ВЫКЛЮЧИТЬ) ===
    if (hasLat) {
        // Удаляем гео-параметры
        currentUrl.searchParams.delete('Latitude');
        currentUrl.searchParams.delete('Longitude');
        currentUrl.searchParams.delete('Distance');

        // Если сортировка была по дистанции, возвращаем сортировку по умолчанию (или name)
        if (currentUrl.searchParams.get('SortField') === 'distance') {
            currentUrl.searchParams.set('SortField', 'name');
            currentUrl.searchParams.set('SortOrder', 'asc'); // Обычно имена сортируют А-Я
        }

        // Сбрасываем пагинацию
        currentUrl.searchParams.set('Page', '1');

        // Применяем изменения
        window.location.assign(currentUrl.toString());
        return;
    }

    // === ЛОГИКА ВКЛЮЧЕНИЯ ===
    if (!navigator.geolocation) {
        alert("Geolocation is not supported by your browser");
        return;
    }

    // Визуализация загрузки
    const btn = document.querySelector('.hero-geo-btn');
    if (btn) btn.classList.add('loading-state'); // Можно добавить стиль в CSS

    navigator.geolocation.getCurrentPosition(
        (position) => {
            const accuracy = position.coords.accuracy; // Точность в метрах
            console.log(`Geo Accuracy: ${accuracy} meters`);

            // Если точность хуже 150 метров -> Показываем попап
            if (accuracy > 150) {
                pendingGeoPosition = position; // Сохраняем, чтобы не искать снова
                showGeoWarning(accuracy);      // Вызываем попап (функция ниже)
                if (btn) btn.classList.remove('loading-state');
            } else {
                // Точность хорошая -> Применяем сразу
                applyGeoLocation(position);
            }
        },
        (error) => {
            console.error("Geo error:", error);
            alert("Unable to retrieve your location.");
            if (btn) btn.classList.remove('loading-state');
        },
        {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 0
        }
    );
};

// Применяет координаты к URL (вынесено в отдельную функцию)
function applyGeoLocation(position) {
    const currentUrl = new URL(window.location.href);

    currentUrl.searchParams.set('Latitude', position.coords.latitude);
    currentUrl.searchParams.set('Longitude', position.coords.longitude);

    // При включении геопоиска логично сразу включить сортировку по дистанции
    currentUrl.searchParams.set('SortField', 'distance');
    currentUrl.searchParams.set('SortOrder', 'asc');

    // Ставим дефолтную дистанцию 10км, если её нет
    if (!currentUrl.searchParams.has('Distance')) {
        currentUrl.searchParams.set('Distance', '10000');
    }

    currentUrl.searchParams.set('Page', '1');
    window.location.assign(currentUrl.toString());
}

// === Управление Диалогом (Попапом) ===
function showGeoWarning(accuracy) {
    const dialog = document.getElementById('geo-warning-dialog');
    const text = document.getElementById('geo-warning-text');

    if (dialog && text) {
        text.innerText = `GPS accuracy is low (~${Math.round(accuracy)}m). Results may be inaccurate.`;
        dialog.showModal(); // Показывает нативный диалог
    } else {
        // Если вдруг диалога нет в DOM (например, не та страница), просто применяем как есть
        if (pendingGeoPosition) applyGeoLocation(pendingGeoPosition);
    }
}

window.closeGeoDialog = function (proceed) {
    const dialog = document.getElementById('geo-warning-dialog');
    if (dialog) dialog.close();

    if (proceed && pendingGeoPosition) {
        applyGeoLocation(pendingGeoPosition);
    }
    // Сбрасываем сохраненную позицию
    pendingGeoPosition = null;

    // Убираем спиннер с кнопки
    const btn = document.querySelector('.hero-geo-btn');
    if (btn) btn.classList.remove('loading-state');
}

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