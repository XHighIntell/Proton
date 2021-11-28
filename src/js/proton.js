


window.proton = function() {
    /** @type Proton.Namespace */
    var proton = {};
    var webview = chrome.webview;

    

    /** @type Proton.BrowserWindow.Namespace */
    var browserWindow = proton.browserWindow = function() {
        browserWindow = new EventTarget();
        

        // fields
        const corner_size = 6;
        const border_size = 2;
        const resolves = {};

        // properties
        var borderStyle = 0;
        var windowState = 0;
        var allowResizable = false;

        /** @type defineProperties<Proton.BrowserWindow> */
        var properties = {
            windowState: {
                get: function() { return windowState },
                set: function(newValue) {
                    if (windowState == newValue) return;

                    windowState = newValue;

                    messenger.setWindowState(windowState);
                }
            }
        };
        Object.defineProperties(browserWindow, properties);

        /** @type Proton.BrowserWindow.Messenger */
        var messenger = browserWindow.messenger = new function() {
            messenger = this;

            messenger.getAll = function() { return browserWindow.postMessagePromise({ action: 'browserWindow.getAll' }); }
            messenger.drag = function() { browserWindow.postMessage({ action: 'browserWindow.drag' }) };
            messenger.startResize = function(hittest) { browserWindow.postMessage({ action: "browserWindow.startResize", hit: hittest }) }
            messenger.setCaptionRectangle = function(rect) { browserWindow.postMessage({ action: "browserWindow.setCaptionRectangle", rect: rect }) }
            messenger.openContextMenu = function() { browserWindow.postMessage({ action: "browserWindow.openContextMenu" }); }
            messenger.setWindowState = function(windowState) { browserWindow.postMessage({ action: "browserWindow.setWindowState", value: windowState }) }
            messenger.releaseCapture = function() { browserWindow.postMessage({ action: 'browserWindow.releaseCapture' }) }
        }


       
        // methods
        browserWindow.generateId = function() {
            var r4 = Math.round(Math.random() * 2176782335);
            return Date.now().toString(36) + ('000000' + r4.toString(36)).slice(-6);
        }
        browserWindow.maximize = function() { this.windowState = 2 }
        browserWindow.minimize = function() { this.windowState = 1 }
        browserWindow.hitTest = function(x, y) {

            if (windowState == 2) return 1;

            if (x <= corner_size && y <= corner_size) return 13; // HT_TOPLEFT
            else if (x <= corner_size && y >= window.innerHeight - corner_size - 1) return 16; // HT_BOTTOMLEFT
            else if (x >= window.innerWidth - corner_size - 1 && y <= corner_size) return 14; // HT_TOPRIGHT
            else if (x >= window.innerWidth - corner_size - 1 && y >= window.innerHeight - corner_size - 1) return 17; // HT_BOTTOMRIGHT
            else if (x <= border_size) return 10; // HT_LEFT
            else if (x >= window.innerWidth - border_size - 1) return 11; // HT_RIGHT
            else if (y <= border_size) return 12; // HT_TOP
            else if (y >= window.innerHeight - border_size - 1) return 15; // HT_BOTTOM

            return 1; // HTCLIENT
        }
        browserWindow.postMessage = function(message) { chrome.webview.postMessage(message); }
        browserWindow.postMessagePromise = function(message) {
            if (typeof message !== 'object') throw new Error("message must be object.")

            return new Promise(function(resolve, reject) {
                var id = browserWindow.generateId();

                resolves[id] = resolve;
                message.__callback = id;

                chrome.webview.postMessage(message);
            })
        }
        browserWindow.onmessage = function(data) {

            if (data.action == 'callback') {
                var id = data.id;

                resolves[id] && (resolves[id](data.data), delete resolves[id]);
            }
            else if (data.action == 'browserWindow.onWindowStateChange') windowState = data.windowState;
            
        }


        // handle events
        var previousPoint = { x: 0, y: 0 };
        var previousTime = 0;

        var LButtonDown = false;
        

        window.addEventListener('mousedown', function(e) {
            var x = e.clientX;
            var y = e.clientY;
            var hit = browserWindow.hitTest(x, y);

            if (e.button == 0) {

                if (hit != 1 && allowResizable == true) {
                    e.preventDefault();
                    e.stopPropagation();
                    messenger.startResize(hit);
                    return;
                }

                var target = e.target;
                var appRegion = getComputedStyle(target)['-webkit-app-region'];
                if (appRegion === 'drag') {
                    var now = Date.now();
                    var elapsed = now - previousTime;
                    previousTime = now;

                    if (elapsed <= 300) {
                        var target = e.target;
                        var appRegion = getComputedStyle(target)['-webkit-app-region'];

                        if (appRegion === 'drag') {

                            if (windowState == 0) browserWindow.windowState = 2;
                            else if (windowState == 2) browserWindow.windowState = 0

                            e.preventDefault();
                            e.stopImmediatePropagation();
                            LButtonDown = false;

                            return;
                        }

                    }

                    previousPoint = { x: e.clientX, y: e.clientY };
                    LButtonDown = true;
                }
            }

            else if (hit != 1) {
                e.preventDefault();

                messenger.releaseCapture();

                return;
            }

        }); // mousedown
        window.addEventListener('mousemove', function(e) {

            if (e.buttons == 0 && allowResizable == true) {
                var x = e.clientX;
                var y = e.clientY;

                var hit = browserWindow.hitTest(x, y);
                var html = document.documentElement;

                if (hit == 13) html.style.cursor = 'nw-resize';
                else if (hit == 16) html.style.cursor = 'sw-resize';
                else if (hit == 14) html.style.cursor = 'ne-resize';
                else if (hit == 17) html.style.cursor = 'se-resize';
                else if (hit == 10) html.style.cursor = 'w-resize';

                else if (hit == 11) html.style.cursor = 'e-resize';
                else if (hit == 12) html.style.cursor = 'n-resize';
                else if (hit == 15) html.style.cursor = 's-resize';
                else html.style.cursor = '';
            }
            


            if ((e.buttons & 1) == 1 && LButtonDown == true) {
                messenger.drag();

                LButtonDown = false;

                e.preventDefault();
                e.stopImmediatePropagation();
            }

            
        }); // mousemove
        window.addEventListener('contextmenu', function(e) {
            const element = e.target;
            const appRegion = getComputedStyle(element)['-webkit-app-region'];

            if (appRegion === 'drag') {
                /** @type DOMRect */
                var clientRect = element.getBoundingClientRect();
                var devicePixelRatio = window.devicePixelRatio;
                var rect = {
                    x: parseInt(clientRect.x * devicePixelRatio),
                    y: parseInt(clientRect.y * devicePixelRatio),
                    width: parseInt(clientRect.width * devicePixelRatio),
                    height: parseInt(clientRect.height * devicePixelRatio),
                }

                messenger.setCaptionRectangle(rect);
                messenger.openContextMenu();


                e.preventDefault();
                e.stopPropagation();
            }
        }); // contextmenu
        webview.addEventListener('message', function(e) { browserWindow.onmessage(e.data) });

        messenger.getAll().then(function(value) {
            windowState = value.windowState;
            borderStyle = value.borderStyle;
            allowResizable = value.allowResizable;
        });

        return browserWindow;
    }();
    


    return proton;

}();




//document.addEventListener('contextmenu', function(e) {
//    const target = e.target;
//    const appRegion = getComputedStyle(target)['-webkit-app-region'];
//
//    if (appRegion === 'drag') {
//        console.log('2');
//        e.preventDefault();
//        e.stopImmediatePropagation();
//    }
//}); // contextmenu

//function BrowserWindow() {
//    
//}
////BrowserWindow.prototype = EventTarget.prototype;
//
//BrowserWindow.prototype = new EventTarget();
//
//BrowserWindow.__proto__ = EventTarget;
//BrowserWindow.prototype.__proto__ = EventTarget.prototype;
//
//
//
//
//
//class BrowserWindow2 extends EventTarget {
//
//}