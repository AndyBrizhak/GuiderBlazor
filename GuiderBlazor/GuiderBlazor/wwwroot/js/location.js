// wwwroot/js/location.js

window.getBrowserLocation = (dotNetHelper) => {
    if (!dotNetHelper) {
        return; // Ошибка (helper не передан)
    }

    if (!navigator.geolocation) {
        // Браузер не поддерживает геолокацию
        dotNetHelper.invokeMethodAsync('LocationError', -1, 'Geolocation is not supported by this browser.');
        return;
    }

    navigator.geolocation.getCurrentPosition(
        (position) => {
            // УСПЕХ: Передаем lat, lon И accuracy
            try {
                dotNetHelper.invokeMethodAsync('SetLocation',
                    position.coords.latitude,
                    position.coords.longitude,
                    position.coords.accuracy); // ⬅️ Передаем точность
            } catch (error) {
                // Игнорируем ошибки, если .NET-компонент уже удален
            }
        },
        (error) => {
            // ОШИБКА
            try {
                dotNetHelper.invokeMethodAsync('LocationError',
                    error.code,
                    error.message);
            } catch (err) {
                // Игнорируем ошибки
            }
        },
        {
            // ⬇️ ВАЖНО: Запрашиваем максимально точные данные (GPS)
            enableHighAccuracy: true,
            timeout: 10000, // 10 секунд на поиск
            maximumAge: 0     // Не использовать старые (кэшированные) данные
        }
    );
};