import Authentication from "./authentication";
import TwitchService from "./twitch-service";
import BlakullaServiceConnection from "./blakulla-service-connection";

export default class BlakullaService {
    public readonly auth: Authentication = null;
    private readonly twitch: TwitchService = null;
    private readonly connection: BlakullaServiceConnection = null;

    constructor() {
        this.connection = new BlakullaServiceConnection();
        this.twitch = new TwitchService();
        this.auth = new Authentication(null, null);
    }

    async testAsync() {
        return await this.auth.apiGet("test");
    }

    async getGameStateAsync() {
        try {
            const result = await this.auth.apiGet("state");
            if (result && result.ok) {
                console.log("game state received: " + result.json());
                return result.json;
            }
        } catch (err) { }
        return null;
    }

    start(
        onAuth: (res: any) => void,
        onVisibilityChanged: (visibility: boolean) => void,
        onContextUpdate: (context: any, delta: any) => void) {

        this.twitch.onAuthorized(async (auth: any) => {
            this.auth.setToken(auth.token, auth.userId);

            console.log("Twitch Authorized");

            // await this.connectAsync();
            // console.log("WS Connected");


            // if (!this.state.finishedLoading) {
            //     this.service.testAsync().then(result => {
            //         this.setState(() => {
            //             return { test: result }
            //         })
            //     })
            //     this.twitch.actions.requestIdShare((e: any) => {
            //         console.log(e)
            //         this.setState(() => {
            //             return { username: e }
            //         })
            //     });
            //     // if the component hasn't finished loading (as in we've not set up after getting a token), let's set it up now.
            //     // now we've done the setup for the component, let's set the state to true to force a rerender with the correct data.
            // }


            onAuth(auth);
        });

        this.twitch.onVisibilityChanged((isVisible: boolean) => onVisibilityChanged(isVisible));

        this.twitch.onContext((context: any, delta: any) => onContextUpdate(context, delta));
    }

    dispose() {
        this.twitch.dispose();
    }

    get isReady(): boolean {
        return this.twitch && this.auth && this.auth.isAuthenticated();
    }
}

