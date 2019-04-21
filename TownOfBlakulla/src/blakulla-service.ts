import Authentication from "./authentication";
import TwitchService from "./twitch-service";
import BlakullaServiceConnection from "./blakulla-service-connection";
import { string } from "prop-types";

export default class BlakullaService {
    public readonly auth: Authentication = null;
    private readonly twitch: TwitchService = null;
    private readonly connection: BlakullaServiceConnection = null;
    private lastChatMessage: string = BlakullaService.utcNow();
    private stateRevision: number = 0;
    private loadingCounter: number = 0;

    public chatMessageEvent: Subject<ChatMessage> = new Subject<ChatMessage>();    
    public loadingEvent: Subject<boolean> = new Subject<boolean>();

    constructor() {
        this.connection = new BlakullaServiceConnection();
        this.twitch = new TwitchService();
        this.auth = new Authentication(null, null);
    }
    
    public isMafia(role:string): boolean {        
        return [
                "Janitor", "Godfather", "Blackmailer", "Mafioso",
                "Ambusher", "Consigliere", "Consort", "Hypnotist",
                "Framer", "Forger", "Disguiser"
            ].includes(role);
    }

    public isMedium(role:string): boolean {
        return role == "medium";
    }

    async getStateAsync() {
        try {
            const result = await this.auth.apiGet(`state/${this.stateRevision}`);
            if (result && result.ok) {
                const jsonResult = await result.json();

                if (jsonResult != null) {
                    this.stateRevision = jsonResult.revision;
                }
                
                return jsonResult;
            } else {
                console.error("getStateAsync failed, uknown reason.");
            }
        } catch (err) { 
            console.error(err);
        }
        return null;
    }

    async getChatMessagesAsync(channel: string) {        
        if (!channel) {
            return;
        }

        try {            
            const result = await this.auth.apiGet(`chat/${channel}/${this.lastChatMessage}`);
            if (result && result.ok) {
                const jsonResult = await result.json();

                if (jsonResult != null && jsonResult.length > 0) {
                    this.lastChatMessage = jsonResult[jsonResult.length - 1].timeSent;
                    for(const msg of jsonResult) {
                        this.chatMessageEvent.next(msg);
                    }
                }
                
                return jsonResult;
            } else {
                console.error("getChatMessages failed, uknown reason.");
            }
        } catch (err) { 
            console.error(err);
        }
        return null;
    }

    async sendChatMessageAsync(channel: string, message: string) : Promise<ChatMessage> {
        try {            
            this.beginLoading();
            const result = await this.auth.apiPost("chat", {channel, message});
            if (result && result.ok) {
                const value = await result.json();
                this.chatMessageEvent.next(value);
                return value;
            } else {
                console.error("sendChatMessageAsync failed, uknown reason.");
            }
        } catch (err) {
            console.error(err);
        } finally { 
            this.endLoading();
        }
        return null;   
    }

    async joinAsync(name: string) {
        try {            
            this.beginLoading();
            const result = await this.auth.apiPost("join", {name});
            if (result && result.ok) {
                return await result.json();
            } else {
                console.error("joinAsync failed, uknown reason.");
            }
        } catch (err) {
            console.error(err);
        } finally { 
            this.endLoading();
        }
        return null;
    }

    async voteAsync(value: string) {
        try {
            this.beginLoading();
            const result = await this.auth.apiPost("vote", {value});
            if (result && result.ok) {
                return await result.json();
            } else {
                console.error("voteAsync failed, uknown reason.");
            }
        } catch (err) {
            console.error(err);
        } finally { 
            this.endLoading();
        }
        return null;
    }
    
    async leaveAsync() {
        try {
            this.beginLoading();
            const result = await this.auth.apiPost("leave");
            if (result && result.ok) {
                return await result.json();
            } else {
                console.error("leaveAsync failed, uknown reason.");
            }
        } catch (err) {
            console.error(err);
        }  finally { 
            this.endLoading();
        }
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

    get isLoading(): boolean {
        return this.loadingCounter > 0;
    }

    static utcNow(): string {
        const date = new Date();
        const year = date.getUTCFullYear();
        const month = BlakullaService.getLeftPaddedNumber(date.getUTCMonth()+1);
        const day = BlakullaService.getLeftPaddedNumber(date.getUTCDate());
        const hour = BlakullaService.getLeftPaddedNumber(date.getUTCHours());
        const minutes =BlakullaService.getLeftPaddedNumber(date.getUTCMinutes());
        const seconds = BlakullaService.getLeftPaddedNumber(date.getUTCSeconds());
        return `${year}-${month}-${day}T${hour}:${minutes}:${seconds}Z`;
    } 

    private static getLeftPaddedNumber(value: number): string {
        return value < 10 ? `0${value}` : `${value}`
    }

    private beginLoading(): void {
        ++this.loadingCounter;
        this.loadingEvent.next(this.isLoading);
    }
    private endLoading(): void {
        --this.loadingCounter;
        this.loadingEvent.next(this.isLoading);
    }    
}

export class Subject<T> {
    private readonly subscriptions: Subscription<T>[] = [];

    constructor() {}

    subscribe(callback: (value:T)=>void): Subscription<T> {
        const sub = new Subscription<T>(this, callback);
        this.subscriptions.push(sub);
        return sub;
    }

    next(value: T) {
        this.subscriptions.forEach(x => x.invoke(value));
    }

    unsubscribe(sub:Subscription<T>) {
        const index = this.subscriptions.indexOf(sub);
        if (index >= 0) {}
            this.subscriptions.splice(index, 1);
    }
}

export class Subscription<T> {
    constructor(
        private readonly subject: Subject<T>,
        private readonly callback: (value: T)=>void) {
    }

    invoke(value: T) {
        if (this.callback) {
            this.callback(value);
        }
    }
    
    unsubscribe() {
        this.subject.unsubscribe(this);
    }
}

export class ChatMessage {
    constructor(
        public readonly timeSent: Date,
        public readonly sender: string,
        public readonly channel: string,
        public readonly message: string) {
    }
}

export enum GameStateType {
    JOINABLE = 0,
    STARTED = 1,
    NOT_STARTED = 2,
    INVALID = -1
}