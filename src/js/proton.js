'use strict';

window.proton = function() {    
    var proton = window.proton; proton = {};
    var webview = chrome.webview;

    // classes
    proton.EventRegister = function EventRegister(target) {

        // 1. If EventRegister is called without the new keyword, recall if with the new keyword
        // 2. If functions are not added to the prototype, create/add them
        // 3. 

        // --1--
        if (this instanceof EventRegister == false) return new EventRegister(target);

        // --2--
        /** @type proton.EventRegister<(this: {x:123}, name: string)=>void> */
        var prototype = EventRegister.prototype;

        if (prototype.addListener == null) {
            prototype.addListener = function(callback) {
                if (typeof (callback) != 'function') throw new Error('The callback must be function.');

                this.listeners.push(callback);
            }
            prototype.removeListener = function(callback) {
                var index = this.listeners.indexOf(callback);
                if (index == -1) return;

                this.listeners.splice(index, 1);
            }
            prototype.dispatch = function() {
                this.listeners.forEach((callback) => {
                    callback.apply(this.target, arguments)
                });
            }
            prototype.hasListener = function(callback) { return this.listeners.indexOf(callback) != -1; }
            prototype.hasListeners = function() { return this.listeners.length > 0; }
        }

        // --3--
        /** @type proton.EventRegister<(this: {x:123}, name: string)=>void> */
        var _this; _this = this;
        _this.listeners = [];
        _this.target = target;

    }

    // methods
    proton.generateId = function() {
        var r4 = Math.round(Math.random() * 2176782335);
        return Date.now().toString(36) + ('000000' + r4.toString(36)).slice(-6);
    }
    proton.postMessage = function(action, data) { webview.postMessage({ action: action, data: data }); }
    proton.postMessagePromise = function postMessagePromise(action, data) {
        
        // 1. If postMessagePromise is called on the first time, create resolves object
        // 2. Add resolve to storage
        // 3. post a message with the correct structure. More details at ProtonWebViewMessage.cs.

        // --1--
        if (postMessagePromise.resolves == null) postMessagePromise.resolves = {};        
        /** @type {{ [T:string]: ()=>void }} */
        var resolves = postMessagePromise.resolves;


        return new Promise(function(resolve, reject) {
            // --2--
            var id = proton.generateId();
            resolves[id] = resolve;

            // --3--
            webview.postMessage({ action: action, id: id, data: data });
        })
    }
    
    // events
    proton.onMessage = new proton.EventRegister();

    // add listeners
    webview.addEventListener('message', function(e) {
        /** @type proton.Message */
        var message = e.data;

        // 1. if action equal "callback", handle features for proton.postMessagePromise

        if (message.action == 'callback') {
            /** @type {{ [T:string]: ()=>void }} */
            var resolves = proton.postMessagePromise.resolves;
            if (resolves == null) return;

            var id = message.id;

            resolves[id] && (resolves[id](message.data), delete resolves[id]);
            return;
        }

        proton.onMessage.dispatch(message);
    });

    // sub-namespace
    proton.window = function() {
        var winform = proton.window; winform = {};

        // ======= enums =======
        winform.FormWindowState = { Normal: 0, Minimized: 1, Maximized: 2 };
        winform.FormBorderStyle = { None: 0, FixedSingle: 1, Fixed3D: 2, FixedDialog: 3, Sizable: 4, FixedToolWindow: 5, SizableToolWindow: 6 };

        
        // ===== properties ====
        var text = "";
        var windowState = winform.windowState;
        var borderStyle = winform.borderStyle;
        var allowResizable = winform.allowResizable;



        /** @type defineProperties<typeof proton.window> */
        var properties = {
            text: {
                get: function() { return text },
                set: function(newValue) {
                    if (typeof newValue != 'string') throw new Error("Value must be string.");

                    text = newValue;
                    proton.postMessage("window.setText", text);
                }
            },
            borderStyle: {
                get: function() { return borderStyle; },
                set: function(newValue) {
                    if (typeof newValue != 'number') throw new Error("Value must be number.")

                    borderStyle = newValue;
                    proton.postMessage("window.setBorderStyle", borderStyle);
                },
            },
            windowState: {
                get: function() { return windowState; },
                set: function(newValue) {
                    if (typeof newValue != 'number') throw new Error("Value must be number.")

                    windowState = newValue;
                    proton.postMessage("window.setWindowState", windowState);
                }
            },
            allowResizable: {
                get: function() { return allowResizable },
                set: function(newValue) {
                    if (typeof newValue != 'boolean') throw new Error("Value must be boolean.")

                    allowResizable = newValue;
                    proton.postMessage("window.setAllowResizable", allowResizable);
                }
            }
        }
        Object.defineProperties(winform, properties);

        // methods

        // ======= events ========
        winform.onWindowStateChange = new proton.EventRegister(winform);

        // ==== add listeners =====
        !function() {
            // in this block, we will do

            // A. handle drag on bar
            // B. handle resize on border
            // C. handle double click on bar
            // D. handle right click on bar (context menu)
            const corner_size = 6;
            const border_size = 2;

            var previousTime = 0;
            var isLMouseDown = false;


            window.addEventListener('mousedown', function(e) {
                var x = e.clientX;
                var y = e.clientY;



                if (e.button == 0) {
                    var hit = hitTest(x, y);

                    if (hit == 1) {
                        // L.mouse down on client
                        // --C--                        
                        // 1. the style (-webkit-app-region) of target must be 'drag'
                        // 2. the time between the first & seconds click must smaller 300ms
                        // 3. switch window state

                       
                        // --1--
                        var target = e.target;
                        var appRegion = getComputedStyle(target)['-webkit-app-region'];
                        if (appRegion !== 'drag') return;

                        

                        // --2--
                        var now = Date.now();
                        var elapsed = now - previousTime;
                        previousTime = now;
                        isLMouseDown = true; // L.mouse is down

                        if (elapsed > 300) return;

                        // --3--
                        if (windowState == winform.FormWindowState.Normal) winform.windowState = winform.FormWindowState.Maximized;
                        else if (windowState == winform.FormWindowState.Maximized) winform.windowState = winform.FormWindowState.Normal;

                        e.preventDefault();
                        e.stopImmediatePropagation();
                        isLMouseDown = false;

                        return;
                    }
                    else if (hit != 1 && allowResizable == true) {
                        // L.mouse down on non-client && resizing is allowed
                        // --B--

                        e.preventDefault();
                        e.stopImmediatePropagation();

                        proton.postMessage("window.startResize", hit);
                        return;
                    }

                }

                //else if (hit != 1) {
                //    //e.preventDefault();
                //
                //    //messenger.releaseCapture();
                //
                //    return;
                //}

            }, { capture: true }); // mousedown
            window.addEventListener('mousemove', function(e) {
                if (e.buttons == 0 && allowResizable == true) {
                    var x = e.clientX;
                    var y = e.clientY;

                    var hit = hitTest(x, y);
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



                if ((e.buttons & 1) == 1 && isLMouseDown == true) {
                    // --A--
                    // Mouse move while L.mouse is down
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    isLMouseDown = false;

                    proton.postMessage("window.startDrag");
                }

            }, { capture: true }); // mousemove
            window.addEventListener('contextmenu', function(e) {
                // --D---
                // 1. the style (-webkit-app-region) of target must be 'drag'
                // 2. Chromium render have built-in zoom, A rectangle have size of (200px;100px) will take 250px;125px if window.devicePixelRatio is 1.25



                // --1--
                const element = e.target;
                const appRegion = getComputedStyle(element)['-webkit-app-region'];

                if (appRegion !== 'drag') return;

                // --2--
                /** @type DOMRect */
                var clientRect = element.getBoundingClientRect();
                var devicePixelRatio = window.devicePixelRatio; 
                var rect = {
                    x: parseInt(clientRect.x * devicePixelRatio),
                    y: parseInt(clientRect.y * devicePixelRatio),
                    width: parseInt(clientRect.width * devicePixelRatio),
                    height: parseInt(clientRect.height * devicePixelRatio),
                }

                e.preventDefault();
                e.stopImmediatePropagation();

                proton.postMessage("window.setCaptionRectangle", rect);
                proton.postMessage("window.openContextMenu");
            }); // contextmenu


            function hitTest(x, y) {

                if (windowState == winform.FormWindowState.Maximized) return 1; // HTCLIENT

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
        }();
        proton.onMessage.addListener(function(m) {

            /** @type proton.window.Message */
            var message = m;
            
            if (message.action == 'window.onWindowStateChange') {
                windowState = message.data.windowState;

                winform.onWindowStateChange.dispatch();
            }
            
        });

        // ======= startup =======
        proton.postMessagePromise("window.getAll").then(function(m) {
            text = m.text;
            windowState = m.windowState;
            borderStyle = m.borderStyle;
            allowResizable = m.allowResizable;
        })

        return winform;
    }();

    // startup
    


    return proton;
}();
