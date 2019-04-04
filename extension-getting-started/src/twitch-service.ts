export default class TwitchService {
    private readonly twitch: any = null;
    constructor() {
        let win = window as any;
        this.twitch = win.Twitch ? win.Twitch.ext : null;
    }
    public onAuthorized(onAuth: (res: any) => void) {
        if (this.twitch) {
            this.twitch.onAuthorized((auth: any) => {
                onAuth(auth);
            });
            this.twitch.listen('broadcast', (target: any, contentType: any, body: any) => {
                this.twitch.rig.log(`New PubSub message!\n${target}\n${contentType}\n${body}`);
                // now that you've got a listener, do something with the result... 
                // do something...
            });
        }
    }
    public onContext(arg0: (context: any, delta: any) => void) {
        this.twitch.onContext((context: any, delta: any) => arg0(context, delta));
    }
    public onVisibilityChanged(arg0: (isVisible: boolean) => void) {
        this.twitch.onVisibilityChanged((isVisible: boolean) => arg0(isVisible));
    }
    public dispose() {
        if (this.twitch) {
            this.twitch.unlisten('broadcast', () => console.log('successfully unlistened'));
        }
    }
}
