// wwwroot/js/location.js

// Проверка загрузки скрипта
/*console.log('✅ location.js loaded successfully');*/

window.getBrowserLocation = (dotNetHelper) => {
   /* console.log('🔍 getBrowserLocation called', dotNetHelper);*/

    if (!dotNetHelper) {
      /*  console.error('❌ dotNetHelper is null or undefined');*/
        return;
    }

    if (!navigator.geolocation) {
        /*console.error('❌ Geolocation not supported');*/
        dotNetHelper.invokeMethodAsync('LocationError', -1, 'Geolocation is not supported by this browser.');
        return;
    }

    /*console.log('📍 Requesting geolocation...');*/

    navigator.geolocation.getCurrentPosition(
        (position) => {
            /*console.log('✅ Position received:', position.coords.latitude, position.coords.longitude);*/

            try {
                dotNetHelper.invokeMethodAsync('SetLocation',
                    position.coords.latitude,
                    position.coords.longitude)
                    .then(() => {
                        /*console.log('✅ SetLocation called successfully');*/
                    })
                    .catch((error) => {
                        /*console.error('❌ Error calling SetLocation:', error);*/
                    });
            } catch (error) {
                /*console.error('❌ Exception in success callback:', error);*/
            }
        },
        (error) => {
            /*console.error('❌ Geolocation error:', error.code, error.message);*/

            try {
                dotNetHelper.invokeMethodAsync('LocationError',
                    error.code,
                    error.message)
                    .then(() => {
                        /*console.log('✅ LocationError called successfully');*/
                    })
                    .catch((err) => {
                        /*console.error('❌ Error calling LocationError:', err);*/
                    });
            } catch (err) {
                /*console.error('❌ Exception in error callback:', err);*/
            }
        },
        {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 0
        }
    );
};

// Тестовая функция для проверки из консоли браузера
//window.testGeolocation = () => {
//    console.log('🧪 Testing geolocation...');
//    if (navigator.geolocation) {
//        navigator.geolocation.getCurrentPosition(
//            (pos) => console.log('✅ Test success:', pos.coords),
//            (err) => console.error('❌ Test error:', err)
//        );
//    } else {
//        console.error('❌ Geolocation not supported');
//    }
//};