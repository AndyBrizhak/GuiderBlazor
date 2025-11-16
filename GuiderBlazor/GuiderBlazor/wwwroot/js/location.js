// wwwroot/js/location.js

window.getBrowserLocation = (dotNetHelper) => {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            (position) => {
                // Success: Call C# method 'SetLocation'
                dotNetHelper.invokeMethodAsync('SetLocation',
                    position.coords.latitude,
                    position.coords.longitude);
            },
            (error) => {
                // Error: Call C# method 'LocationError'
                dotNetHelper.invokeMethodAsync('LocationError',
                    error.code,
                    error.message);
            }
        );
    } else {
        // Error: Browser doesn't support Geolocation
        dotNetHelper.invokeMethodAsync('LocationError',
            -1, // Custom code for "not supported"
            'Geolocation is not supported by this browser.');
    }
};