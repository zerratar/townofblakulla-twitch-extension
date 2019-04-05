export default class BlakullaServiceConnection {
    private connectionSuccess: any;
    private connectionFailed: any;
    private connectedToServer: boolean;
    private connectionPromise: Promise<any>;
    private socket: WebSocket = null;

    constructor() {
        // ready states:
        //  0: CONNECTING
        //  1: OPEN
        //  2: CLOSING
        //  3: CLOSED
        this.connectionSuccess = null;
        this.connectionFailed = null;
        this.connectedToServer = false;
        this.connectionPromise = new Promise<boolean>((resolve, reject) => {
            this.connectionSuccess = resolve;
            this.connectionFailed = reject;
        });
    }

    connectAsync(): Promise<void> {
        if (this.connectedToServer) {
            return;
        }
        this.socket = new WebSocket(`ws://${window.location.hostname}:${window.location.port}/ws`);
        this.socket.onopen = () => this.onConnectionOpen();
        this.socket.onclose = reason => this.onConnectionClose();
        this.socket.onmessage = msg => this.onMessageReceived(msg);
        this.socket.onerror = err => this.onError(err);
        return this.connectionPromise;
    }

    logout() {
        if (this.socket) {
            this.socket.close(1000, "logout");
            this.connectedToServer = false;
        }
    }

    onMessageReceived(msg: any) {
        console.log(msg);
    }

    onConnectionOpen() {
        console.log("connection open");
        this.connectionSuccess(true);
    }

    onConnectionClose() {
        console.log("connection closed");
        this.socket.onopen = undefined;
        this.socket.onclose = undefined;
        this.socket.onerror = undefined;
        this.socket.onmessage = undefined;
        this.socket = null;

        this.connectAsync();
    }

    onError(error: any) {
        console.error(error);
        this.connectionSuccess(false);
    }
    dispose() {
        this.logout();
    }

    get isConnected() {
        return this.socket && this.socket.readyState == WebSocket.OPEN;
    }
}
