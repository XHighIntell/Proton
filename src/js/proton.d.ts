
declare namespace Proton {
    interface Namespace {
        browserWindow: BrowserWindow.Namespace;

    }
}
declare namespace Proton.BrowserWindow {
    interface Namespace extends EventTarget {
        // properties
        windowState: number;
        messenger: Messenger;

        // methods
        generateId(): string;
        maximize(): void;
        minimize(): void;

        hitTest(x: number, y: number): number;
        /** Reserved */
        onmessage(message: Win32NotificationMessaage): void;

        /** Post message to browser window. */
        postMessage(message: object): void;

        /** Post message to browser window with promise. */
        postMessagePromise(message: object): Promise<any>;
    }

    type Win32NotificationMessaage =
        { action: "browserWindow.onWindowStateChange", windowState: number };

    interface Messenger {

        getAll(): Promise<GetAllResponse>;
        drag(): void;
        startResize(NCHITTEST: number): void;
        setCaptionRectangle(rect: object): void;
        openContextMenu(): void;
        setWindowState(windowState: number): void
        releaseCapture(): void;
    }

    interface GetAllResponse {
        windowState: number;
        borderStyle: number;
        allowResizable: boolean;
    }


}


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
        get: (this: T) => any;
        set: (this: T, newValue: T[K]) => void;
    }
}




declare var proton: Proton.Namespace;