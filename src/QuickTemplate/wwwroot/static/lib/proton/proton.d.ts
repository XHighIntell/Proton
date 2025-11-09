declare var chrome: {
    webview: WebViewPort;
};
interface WebViewPort extends MessagePort {
    hostObjects: object;
}
declare namespace proton {
    export type ProtonMessage = {
        action: 'proton.init';
    } | {
        action: 'window.onWindowStateChange';
        data: {
            windowState: winform.FormWindowState;
        };
    } | {
        action: 'callback';
        id: string;
        data: any;
    };
    /** A helper interface for intellisense of postMessage. */
    interface PostMessageMap {
        "example_action": {
            x: number;
        };
        "window.setText": string;
        "window.setWindowState": number;
        "window.setBorderStyle": number;
        "window.setAllowResizable": boolean;
        "window.startDrag": never;
        "window.startResize": number;
        "window.setCaptionRectangle": {
            x: number;
            y: number;
            width: number;
            height: number;
        };
        "window.openContextMenu": never;
    }
    /** A helper interface for intellisense of postMessagePromise. */
    interface PostMessagePromiseMap {
        [K: string]: {
            data: any;
            result: unknown;
        };
        "example_action": {
            data: string;
            result: {
                x: number;
                y: number;
            };
        };
        "window.getAll": {
            data: never;
            result: {
                text: string;
                windowState: proton.winform.FormWindowState;
                borderStyle: number;
                allowResizable: boolean;
            };
        };
    }
    interface EventRegisterOption {
        /** A boolean value indicating that the listener should be invoked at most once after being added. If true, the listener would be automatically removed when invoked. If not specified, defaults to false. */
        once: boolean;
    }
    /** Event register object that is used to add, remove listeners and dispatch them. */
    export class EventRegister<T extends (this: any, ...args: any) => any> {
        /** Initializes a new instance of EventRegister object that is used to add, remove listeners and dispatch them.
         * @param target The value of <mark>this</mark> provided for dispatching to listeners. */
        constructor(target?: object, option?: EventRegisterOption);
        protected option: EventRegisterOption;
        /** Private */
        protected listeners: T[];
        /** Private */
        target: ThisParameterType<T>;
        /** Registers an event listener callback to this event. Listeners can return "stop" to prevent further chain of callbacks. */
        addListener(callback: (this: ThisParameterType<T>, ...args: Parameters<T>) => "stop" | void): void;
        /** Deregisters an event listener callback from this event. */
        removeListener(callback: T): void;
        /** Dispatches a synthetic event. */
        dispatch(...args: Parameters<T>): void;
        /** Determines whether this event includes a listener among its entries.
         * @param callback Listener whose registration status shall be tested */
        hasListener(callback: T): boolean;
        /** Determines whether this event has any listeners. */
        hasListeners(): boolean;
    }
    /** Post a message through the channel to host window. */
    export function postMessage<K extends keyof PostMessageMap>(action: K, data?: PostMessageMap[K]): void;
    /** Post a message through the channel to host window. */
    export function postMessage(action: string, data?: any): void;
    /** Post a message that support callback through the channel to host window. */
    export function postMessagePromise<M extends PostMessagePromiseMap, K extends keyof M>(action: K, data?: M[K]["data"]): Promise<M[K]["result"]>;
    /** Post a message that support callback through the channel to host window. */
    export function postMessagePromise(action: string, data?: any): Promise<any>;
    /** Fires when a message is received from .Net/C# side. */
    export var onMessage: EventRegister<(message: ProtonMessage) => void>;
    export {};
}
declare namespace proton.winform {
    enum FormWindowState {
        Normal = 0,
        Minimized = 1,
        Maximized = 2
    }
    enum FormBorderStyle {
        None = 0,
        FixedSingle = 1,
        Fixed3D = 2,
        FixedDialog = 3,
        Sizable = 4,
        FixedToolWindow = 5,
        SizableToolWindow = 6
    }
    /** Gets or sets the window title. */
    let text: string;
    /** Gets or sets the border style of the form. */
    let borderStyle: FormBorderStyle;
    /** Gets or sets a value that indicates whether form is minimized, maximized, or normal. */
    let windowState: FormWindowState;
    /** Gets or sets a value indicating whether the form can be resized from Webview. */
    let allowResizable: boolean;
    var onWindowStateChange: EventRegister<(this: typeof proton.winform) => void>;
    /** This will be called when the page is loaded on ProtonWebView. */
    function init(): void;
}
type defineProperties<T> = {
    [K in keyof T]?: {
        get: (this: T) => T[K];
        set: (this: T, newValue: T[K]) => void;
    };
};
