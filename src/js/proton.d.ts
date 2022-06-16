declare var chrome: {
    webview: WebViewPort;
};

interface WebViewPort extends MessagePort {
    hostObjects: object;
}

type NotFunctionKeys<T> = {
    [k in keyof T]: T[k] extends Function ? never : k
}[keyof T];

type defineProperties<T> = {
    [K in NotFunctionKeys<T>]: {
        get: (this: T) => T[K];
        set: (this: T, newValue: T[K]) => void;
    }
}





declare namespace proton {

    interface Message {
        action: string;
        data: any;
        /** It is only for callback */
        id: string;
    }

    /** A helper interface for intellisense of postMessage. */
    interface PostMessageMap {
        "example_action": { x: number },
    }

    /** A helper interface for intellisense of postMessagePromise. */
    interface PostMessagePromiseMap {
        "example_action": {
            data: string,
            result: { x: number, y: number }
        }
    }

    // ====== classes ========
    /** Event register object that is used to add, remove listeners and dispatch them. */
    export class EventRegister<T extends (this: any, ...args: any) => any> {
        /** Private */
        protected listeners: T[];

        /** Private */
        protected target: ThisParameterType<T>;
        
        /** Registers an event listener callback to this event. */
        addListener(callback: T): void;

        /** Deregisters an event listener callback from this event. */
        removeListener(callback: T): void;

        /** Dispatches a synthetic event to target. */
        dispatch(...args: Parameters<T>): void;

        hasListener(callback: T): boolean;

        hasListeners(): boolean;

        constructor(target?: object);
    }


    // ====== methods ========
    private function generateId(): string;

    /** Post a message through the channel to host window. */
    export function postMessage<K extends keyof PostMessageMap>(action: K, data?: PostMessageMap[K]): void;

    /** Post a message through the channel to host window. */
    export function postMessage(action: string, data?: any): void;

    /** Post a message that support callback through the channel to host window. */
    export function postMessagePromise<M extends PostMessagePromiseMap, K extends keyof M>(action: K, data?: M[K]["data"]): Promise<M[K]["result"]>;

    /** Post a message that support callback through the channel to host window. */
    export function postMessagePromise(action: string, data?: any): Promise<any>;
    

    // ======= events ========
    /** Fires when a message is received from .Net/C# side. */
    export var onMessage: EventRegister<(message: Message) => void>;
}


declare namespace proton.window {

    type Message = { action: 'window.onWindowStateChange', data: { windowState: FormWindowState } };

    // enums
    export enum FormWindowState {
        Normal = 0,
        Minimized = 1,
        Maximized = 2,
    }
    export enum FormBorderStyle {
        None = 0,
        FixedSingle = 1,
        Fixed3D = 2,
        FixedDialog = 3,
        Sizable = 4,
        FixedToolWindow = 5,
        SizableToolWindow = 6,
    }

    // properties

    /** Gets or sets the window title. */
    export var text: string;
    /** Gets or sets a value that indicates whether form is minimized, maximized, or normal. */
    export var windowState: FormWindowState;
    /** Gets or sets the border style of the form. */
    export var borderStyle: FormBorderStyle;
    /** Gets or sets a value indicating whether the form can be resized from Webview. */
    export var allowResizable: boolean;    
    
    // ======= events ========
    export var onWindowStateChange: EventRegister<(this: proton.window) => void>;
}
declare namespace proton {
    interface PostMessageMap {
        "window.setText": string,
        "window.setWindowState": number,
        "window.setBorderStyle": number,
        "window.setAllowResizable": boolean;

        "window.startDrag": never;
        "window.startResize": number;
        "window.setCaptionRectangle": { x: number, y: number, width: number, height: number };
        "window.openContextMenu": never;
    }

    interface PostMessagePromiseMap {
        "window.getAll": {
            data: never;
            result: {
                text: string;
                windowState: proton.window.FormWindowState;
                borderStyle: number;
                allowResizable: boolean;
            };
        };
    }
}


