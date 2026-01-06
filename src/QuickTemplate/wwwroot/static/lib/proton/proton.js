var proton;
(function (proton) {
    const webview = chrome.webview;
    /** Event register object that is used to add, remove listeners and dispatch them. */
    class EventRegister {
        /** Initializes a new instance of EventRegister object that is used to add, remove listeners and dispatch them.
         * @param target The value of <mark>this</mark> provided for dispatching to listeners. */
        constructor(target, option) {
            if (this instanceof EventRegister == false)
                return new EventRegister(target, option);
            this.option = { once: option?.once ?? false };
            this.listeners = [];
            this.target = target;
        }
        option;
        /** Private */
        listeners = [];
        /** Private */
        target;
        /** Registers an event listener callback to this event. Listeners can return "stop" to prevent further chain of callbacks. */
        addListener(callback) {
            if (typeof (callback) != 'function')
                throw new Error('The callback must be function.');
            this.listeners.push(callback);
        }
        /** Deregisters an event listener callback from this event. */
        removeListener(callback) {
            let index = this.listeners.indexOf(callback);
            if (index == -1)
                return;
            this.listeners.splice(index, 1);
        }
        /** Dispatches a synthetic event. */
        dispatch(...args) {
            // 1. dispatch event to the listeners
            //      a. if there is any error in a listener, log into the console and continue.
            // 2. if true, the listener would be automatically removed when invoked
            // 3. if any of the listeners return "stopPropagation", stop. This is internally used
            var once = this.option.once;
            for (var i = 0; i < this.listeners.length; i++) {
                var callback = this.listeners[i];
                // --1--
                try {
                    var action = callback.apply(this.target, arguments);
                }
                catch (e) {
                    console.error(e);
                }
                // --2--
                if (once === true) {
                    this.listeners.splice(i, 1);
                    i--;
                }
                // --3--
                if (action == "stop" || action == "stopPropagation")
                    break;
            }
        }
        /** Determines whether this event includes a listener among its entries.
         * @param callback Listener whose registration status shall be tested */
        hasListener(callback) { return this.listeners.indexOf(callback) != -1; }
        /** Determines whether this event has any listeners. */
        hasListeners() { return this.listeners.length > 0; }
    }
    proton.EventRegister = EventRegister;
    class RemoteError extends Error {
        constructor(message, type) {
            super(message);
            this.name = 'RemoteError';
            this.type = type;
        }
        /** Gets or sets Exception type. */
        type;
    }
    proton.RemoteError = RemoteError;
    const postMessagePromiseResolves = {};
    //#region methods
    function generateId() {
        var r4 = Math.round(Math.random() * 2176782335);
        return Date.now().toString(36) + ('000000' + r4.toString(36)).slice(-6);
    }
    function exceptionToError(exception) {
        let error;
        if (exception.type == 'AggregateException')
            error = new AggregateError(exception.errors.map(ex => exceptionToError(ex)), exception.message);
        else
            error = new Error(exception.message);
        error.name = exception.type;
        error.stack = exception.stack;
        return error;
    }
    function postMessage(action, data) { webview.postMessage({ action: action, data: data }); }
    proton.postMessage = postMessage;
    function postMessagePromise(action, data) {
        // 1. If postMessagePromise is called on the first time, create resolves object
        // 2. Add resolve to storage
        // 3. post a message with the correct structure. More details at ProtonWebViewMessage.cs.
        // --1--
        return new Promise(function (resolve, reject) {
            // --2--
            let id = generateId();
            postMessagePromiseResolves[id] = { resolve, reject };
            // --3--
            webview.postMessage({ action: action, id: id, data: data });
        });
    }
    proton.postMessagePromise = postMessagePromise;
    //#endregion
    // ======= events ========
    /** Fires when a message is received from .Net/C# side. */
    proton.onMessage = new proton.EventRegister();
    // add listeners
    webview.addEventListener('message', function (e) {
        var message = e.data;
        // 1. if action equal "callback", handle features for proton.postMessagePromise
        if (message.action == "callback") {
            const id = message.id;
            postMessagePromiseResolves[id] && (postMessagePromiseResolves[id].resolve(message.data), delete postMessagePromiseResolves[id]);
            return;
        }
        else if (message.action == "callback_exception") {
            const exception = message.data;
            const error = exceptionToError(exception);
            const id = message.id;
            postMessagePromiseResolves[id] && (postMessagePromiseResolves[id].reject(error), delete postMessagePromiseResolves[id]);
            return;
        }
        proton.onMessage.dispatch(message);
    });
})(proton || (proton = {}));
(function (proton) {
    var winform;
    (function (winform) {
        let FormWindowState;
        (function (FormWindowState) {
            FormWindowState[FormWindowState["Normal"] = 0] = "Normal";
            FormWindowState[FormWindowState["Minimized"] = 1] = "Minimized";
            FormWindowState[FormWindowState["Maximized"] = 2] = "Maximized";
        })(FormWindowState = winform.FormWindowState || (winform.FormWindowState = {}));
        let FormBorderStyle;
        (function (FormBorderStyle) {
            FormBorderStyle[FormBorderStyle["None"] = 0] = "None";
            FormBorderStyle[FormBorderStyle["FixedSingle"] = 1] = "FixedSingle";
            FormBorderStyle[FormBorderStyle["Fixed3D"] = 2] = "Fixed3D";
            FormBorderStyle[FormBorderStyle["FixedDialog"] = 3] = "FixedDialog";
            FormBorderStyle[FormBorderStyle["Sizable"] = 4] = "Sizable";
            FormBorderStyle[FormBorderStyle["FixedToolWindow"] = 5] = "FixedToolWindow";
            FormBorderStyle[FormBorderStyle["SizableToolWindow"] = 6] = "SizableToolWindow";
        })(FormBorderStyle = winform.FormBorderStyle || (winform.FormBorderStyle = {}));
        let _text;
        let _borderStyle;
        let _windowState;
        let _allowResizable;
        let properties = {
            text: {
                get: function () { return _text; },
                set: function (newValue) {
                    if (typeof newValue != 'string')
                        throw new Error("Value must be string.");
                    _text = newValue;
                    proton.postMessage("window.setText", _text);
                }
            },
            borderStyle: {
                get: function () { return _borderStyle; },
                set: function (newValue) {
                    if (typeof newValue != 'number')
                        throw new Error("Value must be number.");
                    _borderStyle = newValue;
                    proton.postMessage("window.setBorderStyle", _borderStyle);
                },
            },
            windowState: {
                get: function () { return _windowState; },
                set: function (newValue) {
                    if (typeof newValue != 'number')
                        throw new Error("Value must be number.");
                    _windowState = newValue;
                    proton.postMessage("window.setWindowState", _windowState);
                }
            },
            allowResizable: {
                get: function () { return _allowResizable; },
                set: function (newValue) {
                    if (typeof newValue != 'boolean')
                        throw new Error("Value must be boolean.");
                    _allowResizable = newValue;
                    proton.postMessage("window.setAllowResizable", _allowResizable);
                }
            }
        };
        Object.defineProperties(winform, properties);
        // ======= events ========
        winform.onWindowStateChange = new proton.EventRegister(winform);
        // methods
        /** Closes the form. */
        function close() {
            proton.postMessage('window.close');
        }
        winform.close = close;
        /** This will be called when the page is loaded on ProtonWebView. */
        function init() {
            +function () {
                // in this block, we will do
                // A. handle drag on bar
                // B. handle resize on border
                // C. handle double click on bar
                // D. handle right click on bar (context menu)
                const corner_size = 6;
                const border_size = 2;
                let previousTime = 0;
                let isLMouseDown = false;
                window.addEventListener('mousedown', function (e) {
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
                            if (appRegion !== 'drag')
                                return;
                            // --2--
                            var now = Date.now();
                            var elapsed = now - previousTime;
                            previousTime = now;
                            isLMouseDown = true; // L.mouse is down
                            if (elapsed > 300)
                                return;
                            // --3--
                            if (winform.windowState == winform.FormWindowState.Normal)
                                winform.windowState = winform.FormWindowState.Maximized;
                            else if (winform.windowState == winform.FormWindowState.Maximized)
                                winform.windowState = winform.FormWindowState.Normal;
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            isLMouseDown = false;
                            return;
                        }
                        else if (hit != 1 && winform.allowResizable == true) {
                            // L.mouse down on non-client && resizing is allowed
                            // --B--
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            proton.postMessage("window.startResize", hit);
                            return;
                        }
                    }
                }, { capture: true }); // mousedown
                window.addEventListener('mousemove', function (e) {
                    if (e.buttons == 0 && winform.allowResizable == true) {
                        var x = e.clientX;
                        var y = e.clientY;
                        var hit = hitTest(x, y);
                        var html = document.documentElement;
                        if (hit == 13)
                            html.style.cursor = 'nw-resize';
                        else if (hit == 16)
                            html.style.cursor = 'sw-resize';
                        else if (hit == 14)
                            html.style.cursor = 'ne-resize';
                        else if (hit == 17)
                            html.style.cursor = 'se-resize';
                        else if (hit == 10)
                            html.style.cursor = 'w-resize';
                        else if (hit == 11)
                            html.style.cursor = 'e-resize';
                        else if (hit == 12)
                            html.style.cursor = 'n-resize';
                        else if (hit == 15)
                            html.style.cursor = 's-resize';
                        else
                            html.style.cursor = '';
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
                window.addEventListener('contextmenu', function (e) {
                    // --D---
                    // 1. the style (-webkit-app-region) of target must be 'drag'
                    // 2. Chromium render have built-in zoom, A rectangle have size of (200px;100px) will take 250px;125px if window.devicePixelRatio is 1.25
                    // --1--
                    const element = e.target;
                    const appRegion = getComputedStyle(element)['-webkit-app-region'];
                    if (appRegion !== 'drag')
                        return;
                    // --2--
                    let clientRect = element.getBoundingClientRect();
                    let devicePixelRatio = window.devicePixelRatio;
                    let rect = {
                        x: parseInt(clientRect.x * devicePixelRatio),
                        y: parseInt(clientRect.y * devicePixelRatio),
                        width: parseInt(clientRect.width * devicePixelRatio),
                        height: parseInt(clientRect.height * devicePixelRatio),
                    };
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    proton.postMessage("window.setCaptionRectangle", rect);
                    proton.postMessage("window.openContextMenu");
                }); // contextmenu
                window.addEventListener('click', function (e) {
                    const minimize = e.target.closest('[data-proton-role=minimize]');
                    const maximize = e.target.closest('[data-proton-role=maximize]');
                    const close = e.target.closest('[data-proton-role=close]');
                    debugger;
                    if (minimize != null)
                        proton.winform.windowState = FormWindowState.Minimized;
                    else if (maximize != null) {
                        if (proton.winform.windowState != FormWindowState.Maximized)
                            proton.winform.windowState = FormWindowState.Maximized;
                        else if (proton.winform.windowState == FormWindowState.Maximized)
                            proton.winform.windowState = FormWindowState.Normal;
                    }
                    else if (close != null)
                        proton.winform.close();
                    //console.log(e, e.target, e.currentTarget);
                });
                function hitTest(x, y) {
                    if (winform.windowState == winform.FormWindowState.Maximized)
                        return 1; // HTCLIENT
                    if (x <= corner_size && y <= corner_size)
                        return 13; // HT_TOPLEFT
                    else if (x <= corner_size && y >= window.innerHeight - corner_size - 1)
                        return 16; // HT_BOTTOMLEFT
                    else if (x >= window.innerWidth - corner_size - 1 && y <= corner_size)
                        return 14; // HT_TOPRIGHT
                    else if (x >= window.innerWidth - corner_size - 1 && y >= window.innerHeight - corner_size - 1)
                        return 17; // HT_BOTTOMRIGHT
                    else if (x <= border_size)
                        return 10; // HT_LEFT
                    else if (x >= window.innerWidth - border_size - 1)
                        return 11; // HT_RIGHT
                    else if (y <= border_size)
                        return 12; // HT_TOP
                    else if (y >= window.innerHeight - border_size - 1)
                        return 15; // HT_BOTTOM
                    return 1; // HTCLIENT
                }
            }();
            proton.postMessagePromise("window.getAll").then(function (m) {
                winform.text = m.text;
                winform.windowState = m.windowState;
                winform.borderStyle = m.borderStyle;
                winform.allowResizable = m.allowResizable;
            });
        }
        // ======= startup =======
        proton.onMessage.addListener(function (message) {
            if (message.action == 'window.onWindowStateChange') {
                _windowState = message.data.windowState;
                winform.onWindowStateChange.dispatch();
                return "stop";
            }
        });
        init();
        //document.addEventListener('DOMContentLoaded', function() {
        //    console.log('DOMContentLoaded', chrome, chrome.webview);
        //});
    })(winform = proton.winform || (proton.winform = {}));
})(proton || (proton = {}));
